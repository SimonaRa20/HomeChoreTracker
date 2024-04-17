using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Node;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using Table = iText.Layout.Element.Table;


namespace HomeChoreTracker.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class FinanceController : Controller
    {
		private readonly IIncomeRepository _incomeRepository;
		private readonly IExpenseRepository _expenseRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHomeRepository _homeRepository;

		public FinanceController(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository, IHomeRepository homeRepository, IGamificationRepository gamificationRepository, IUserRepository userRepository, INotificationRepository notificationRepository)
		{
			_incomeRepository = incomeRepository;
			_expenseRepository = expenseRepository;
            _gamificationRepository = gamificationRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _homeRepository = homeRepository;
		}

		[HttpGet("totalIncome")]
		[Authorize]
		public async Task<IActionResult> GetCurrentMonthTotalIncome()
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalIncome(userId);
			return Ok(totalIncome);
		}

        [HttpGet("totalIncome/{homeId}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentMonthTotalIncome(int homeId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(homeId, userId);

            if (!isMember)
            {
                return Forbid();
            }
            decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalHomeIncome(homeId);
            return Ok(totalIncome);
        }

        [HttpPost("income")]
		[Authorize]
		public async Task<IActionResult> AddIncome(IncomeRequest incomeRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userRepository.GetUserById(userId);
            var income = new FinancialRecord();

			if (incomeRequest.FinancialCategoryId == 0)
            {
                FinancialCategory category = await _incomeRepository.CheckCategory(incomeRequest.NewFinancialCategory);

                FinancialCategory addedCategory = new FinancialCategory();


				if (category == null)
                {
					FinancialCategory newfinancialCategory = new FinancialCategory
					{
						Name = incomeRequest.NewFinancialCategory,
						Type = FinancialType.Income,
						UserId = userId,
						HomeId = incomeRequest.HomeId
					};

					addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
				}
                else
                {
                    addedCategory = category;
				}

                income = new FinancialRecord
                {
                    Title = incomeRequest.Title,
                    Amount = incomeRequest.Amount,
                    Description = incomeRequest.Description,
                    Time = incomeRequest.Time,
                    Type = FinancialType.Income,
					FinancialCategoryId = addedCategory.Id,
                    HomeId = incomeRequest.HomeId,
                    UserId = userId,
                };
            }
            else
            {
                income = new FinancialRecord
                {
                    Title = incomeRequest.Title,
                    Amount = incomeRequest.Amount,
                    Description = incomeRequest.Description,
                    Time = incomeRequest.Time,
                    Type = FinancialType.Income,
					FinancialCategoryId = incomeRequest.FinancialCategoryId,
                    HomeId = incomeRequest.HomeId,
                    UserId = userId,
                };
            }

			await _incomeRepository.AddIncome(income);

            var hasBadge = await _gamificationRepository.UserHasCreateFirstIncomeBadge(userId);
            if(!hasBadge)
            {
                BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(userId);
                wallet.CreateFirstIncome = true;
                await _gamificationRepository.UpdateBadgeWallet(wallet);

                Notification notification = new Notification
                {
                    Title = $"You earned badge 'Create first income'",
                    IsRead = false,
                    Time = DateTime.Now,
                    UserId = (int)userId,
                    User = user,
                };

                await _notificationRepository.CreateNotification(notification);
            }

            return Ok("Income added successfully");
		}

		[HttpPut("income/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateIncome(int id, IncomeRequest incomeRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var income = new FinancialRecord();
            if (incomeRequest.FinancialCategoryId == 0)
            {
				FinancialCategory category = await _incomeRepository.CheckCategory(incomeRequest.NewFinancialCategory);

				FinancialCategory addedCategory = new FinancialCategory();


				if (category == null)
				{
					FinancialCategory newfinancialCategory = new FinancialCategory
					{
						Name = incomeRequest.NewFinancialCategory,
						Type = FinancialType.Income,
						UserId = userId,
						HomeId = incomeRequest.HomeId
					};

					addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
				}
				else
				{
					addedCategory = category;
				}


                income = new FinancialRecord
                {
                    Id = id,
                    Title = incomeRequest.Title,
                    Amount = incomeRequest.Amount,
                    Description = incomeRequest.Description,
                    Time = incomeRequest.Time,
                    Type = FinancialType.Income,
                    FinancialCategoryId = addedCategory.Id,
                    HomeId = incomeRequest.HomeId,
                    UserId = userId,
                };
            }
            else
            {
                income = new FinancialRecord
                {
                    Id = id,
                    Title = incomeRequest.Title,
                    Amount = incomeRequest.Amount,
                    Description = incomeRequest.Description,
                    Time = incomeRequest.Time,
                    Type = FinancialType.Income,
                    FinancialCategoryId = incomeRequest.FinancialCategoryId,
                    HomeId = incomeRequest.HomeId,
                    UserId = userId,
                };
            }

            await _incomeRepository.Update(income);
			return Ok("Income updated successfully");
		}

		[HttpGet("income/{id}")]
		[Authorize]
		public async Task<IActionResult> GetIncomeById(int id)
		{
			var income = await _incomeRepository.GetIncomeById(id);
			if (income == null)
			{
				return NotFound($"Income with ID {id} not found");
			}
			return Ok(income);
		}

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFinanceById(int id, string type)
        {
			if(type.Equals("income"))
			{
                await _incomeRepository.Delete(id);
                return Ok($"Expense with ID {id} deleted successfully");
            }
			else
			{
                await _expenseRepository.Delete(id);
                return Ok($"Expense with ID {id} deleted successfully");
            }
        }


        [HttpDelete("income/{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteIncomeById(int id)
		{
			await _incomeRepository.Delete(id);
			return Ok($"Income with ID {id} deleted successfully");
		}

		[HttpGet("totalExpense")]
		[Authorize]
		public async Task<IActionResult> GetCurrentMonthTotalExpense()
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalExpense(userId);
			return Ok(totalExpense);
		}

        [HttpGet("totalExpense/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentMonthTotalExpense(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(id, userId);

            if (!isMember)
            {
                return Forbid();
            }
            decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalHomeExpense(id);
            return Ok(totalExpense);
        }

        [HttpGet("transferHistory/skip{skip}/take{take}")]
        [Authorize]
        public async Task<IActionResult> GetTransferHistory(int skip = 0, int take = 5)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var incomes = await _incomeRepository.GetAll(userId);
            var expenses = await _expenseRepository.GetAll(userId);

            var transferHistory = new List<TransferHistoryItem>();

            foreach (var income in incomes)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Income",
                    Data = new TransferData
                    {
                        Id = income.Id,
                        Title = income.Title,
                        Amount = income.Amount,
                        Description = income.Description,
                        Time = income.Time,
                        Type = (int)income.Type,
						Category = await _incomeRepository.GetIncomeCategory((int)income.FinancialCategoryId),
						UserId = income.UserId,
                    }
                });
            }

            foreach (var expense in expenses)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Expense",
                    Data = new TransferData
                    {
                        Id = expense.Id,
                        Title = expense.Title,
                        Amount = expense.Amount,
                        Description = expense.Description,
                        Time = expense.Time,
                        Type = (int)expense.Type,
						Category = await _expenseRepository.GetExpenseCategory((int)expense.FinancialCategoryId),
						UserId = expense.UserId,
                    }
                });
            }

            transferHistory = transferHistory.OrderByDescending(item => item.Data.Time).ToList();

			transferHistory = transferHistory.Skip(skip).Take(take).ToList();

            return Ok(transferHistory);
        }

        [HttpGet("transferHistory")]
        [Authorize]
        public async Task<IActionResult> GetTransferHistory()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var incomes = await _incomeRepository.GetAll(userId);
            var expenses = await _expenseRepository.GetAll(userId);

            var transferHistory = new List<TransferHistoryItem>();

            foreach (var income in incomes)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Income",
                    Data = new TransferData
                    {
                        Id = income.Id,
                        Title = income.Title,
                        Amount = income.Amount,
                        Description = income.Description,
                        Time = income.Time,
						Category = await _incomeRepository.GetIncomeCategory((int)income.FinancialCategoryId),
						Type = (int)income.Type,
                        UserId = income.UserId,
                    }
                });
            }

            foreach (var expense in expenses)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Expense",
                    Data = new TransferData
                    {
                        Id = expense.Id,
                        Title = expense.Title,
                        Amount = expense.Amount,
                        Description = expense.Description,
                        Time = expense.Time,
                        Type = (int)expense.Type,
						Category = await _expenseRepository.GetExpenseCategory((int)expense.FinancialCategoryId),
						UserId = expense.UserId,
                    }
                });
            }

            transferHistory = transferHistory.OrderByDescending(item => item.Data.Time).ToList();

            return Ok(transferHistory);
        }

        [HttpGet("transferHistory/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTransferHistory(int id)
        {
            var incomes = await _incomeRepository.GetHomeAll(id);
            var expenses = await _expenseRepository.GetHomeAll(id);

            var transferHistory = new List<TransferHistoryItem>();

            foreach (var income in incomes)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Income",
                    Data = new TransferData
                    {
                        Id = income.Id,
                        Title = income.Title,
                        Amount = income.Amount,
                        Description = income.Description,
                        Time = income.Time,
						Category = await _incomeRepository.GetIncomeCategory((int)income.FinancialCategoryId),
						Type = (int)income.Type,
                        UserId = income.UserId,
                    }
                });
            }

            foreach (var expense in expenses)
            {
                transferHistory.Add(new TransferHistoryItem
                {
                    Type = "Expense",
                    Data = new TransferData
                    {
                        Id = expense.Id,
                        Title = expense.Title,
                        Amount = expense.Amount,
                        Description = expense.Description,
                        Time = expense.Time,
                        Type = (int)expense.Type,
                        Category = await _expenseRepository.GetExpenseCategory((int)expense.FinancialCategoryId),
						UserId = expense.UserId,
                    }
                });
            }

            transferHistory = transferHistory.OrderByDescending(item => item.Data.Time).ToList();

            return Ok(transferHistory);
        }

        [HttpPost("expenseimage")]
		[Authorize]
		public async Task<IActionResult> AddExpenseFromImage([FromForm]ExpenseImageRequest expenseRequest)
		{
			try
			{
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var user = await _userRepository.GetUserById(userId);
                if (expenseRequest.ExpenseImage == null)
				{
					return new ObjectResult("Upload image is required")
					{
						StatusCode = (int)HttpStatusCode.UnprocessableEntity
					};
				}

				using (var memoryStream = new MemoryStream())
				{
					await expenseRequest.ExpenseImage.CopyToAsync(memoryStream);

					string result = await ExtractTextFromImage(memoryStream.ToArray(), "eng", "helloworld");

					Rootobject parsedResult = JsonConvert.DeserializeObject<Rootobject>(result);

					if (parsedResult.OCRExitCode != 1)
					{
						return BadRequest(parsedResult.ErrorMessage.First());
					}


					string extractedText = parsedResult.ParsedResults[0].ParsedText;

					string title = ParseTitle(extractedText);
					decimal amount = ParseAmount(extractedText);

                    var expense = new FinancialRecord();
                    if (expenseRequest.FinancialCategoryId == 0)
                    {
						FinancialCategory category = await _expenseRepository.CheckCategory(expenseRequest.NewFinancialCategory);

						FinancialCategory addedCategory = new FinancialCategory();
						if (category == null)
						{
							FinancialCategory newfinancialCategory = new FinancialCategory
							{
								Name = expenseRequest.NewFinancialCategory,
								Type = FinancialType.Income,
								UserId = userId,
								HomeId = expenseRequest.HomeId
							};

							addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
						}
						else
						{
							addedCategory = category;
						}

                        expense = new FinancialRecord
                        {
                            Title = title,
                            Amount = amount,
                            Description = extractedText,
                            Time = DateTime.Now,
                            Type = FinancialType.Expense,
                            FinancialCategoryId = addedCategory.Id,
                            HomeId = expenseRequest.HomeId,
                            UserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value),
                        };
                    }
                    else
                    {
                        expense = new FinancialRecord
                        {
                            Title = title,
                            Amount = amount,
                            Description = extractedText,
                            Time = DateTime.Now,
                            Type = FinancialType.Expense,
                            FinancialCategoryId = expenseRequest.FinancialCategoryId,
                            HomeId = expenseRequest.HomeId,
                            UserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value),
                        };
                    }

					await _expenseRepository.AddExpense(expense);

                    var hasBadge = await _gamificationRepository.UserHasCreateFirstExpenseBadge(userId);
                    if (!hasBadge)
                    {
                        BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(userId);
                        wallet.CreateFirstExpense = true;
                        await _gamificationRepository.UpdateBadgeWallet(wallet);

                        Notification notification = new Notification
                        {
                            Title = $"You earned badge 'Create first expense'",
                            IsRead = false,
                            Time = DateTime.Now,
                            UserId = (int)userId,
                            User = user,
                        };

                        await _notificationRepository.CreateNotification(notification);
                    }

                    return Ok("Expense added successfully");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		private string ParseTitle(string extractedText)
		{
			string[] lines = extractedText.Split('\n');
			string shopTitle = lines[0].Trim();
			return shopTitle;
		}

		private decimal ParseAmount(string extractedText)
		{
			string[] lines = extractedText.Split('\n');
			foreach (string line in lines)
			{
				if (line.Trim().StartsWith("MOKETI"))
				{
					string[] tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string token in tokens)
					{
						string cleanedToken = token.Replace(",", ".").Trim();
						if (decimal.TryParse(cleanedToken, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
						{
							return value;
						}
					}
				}
				if (line.Trim().StartsWith("SUMA"))
				{
					string[] tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string token in tokens)
					{
						string cleanedToken = token.Replace(",", ".").Trim();
						if (decimal.TryParse(cleanedToken, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
						{
							return value;
						}
					}
				}
				if (line.Trim().StartsWith("Mokéti"))
				{
					string[] tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string token in tokens)
					{
						string cleanedToken = token.Replace(",", ".").Trim();
						if (decimal.TryParse(cleanedToken, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
						{
							return value;
						}
					}
				}
			}
			return 0;
		}


		private async Task<string> ExtractTextFromImage(byte[] imageData, string language, string apiKey)
		{
			using (var httpClient = new HttpClient())
			{
				using (var form = new MultipartFormDataContent())
				{
					form.Add(new StringContent(apiKey), "apikey");
					form.Add(new StringContent(language), "language");
					form.Add(new StringContent("2"), "ocrengine");
					form.Add(new StringContent("true"), "scale");
					form.Add(new StringContent("true"), "istable");

					form.Add(new ByteArrayContent(imageData), "image", "image.jpg");

					var response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

					response.EnsureSuccessStatusCode();

					return response.Content.ReadAsStringAsync().Result;
				}
			}
		}


		[HttpPost("expense")]
		[Authorize]
		public async Task<IActionResult> AddExpense(ExpenseRequest expenseRequest)
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userRepository.GetUserById(userId);
            var expense = new FinancialRecord();
            if (expenseRequest.FinancialCategoryId == 0)
            {
				FinancialCategory category = await _expenseRepository.CheckCategory(expenseRequest.NewFinancialCategory);

				FinancialCategory addedCategory = new FinancialCategory();
				if (category == null)
				{
					FinancialCategory newfinancialCategory = new FinancialCategory
					{
						Name = expenseRequest.NewFinancialCategory,
						Type = FinancialType.Income,
						UserId = userId,
						HomeId = expenseRequest.HomeId
					};

					addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
				}
				else
				{
					addedCategory = category;
				}

				expense = new FinancialRecord
                {
                    Title = expenseRequest.Title,
                    Amount = expenseRequest.Amount,
                    Description = expenseRequest.Description,
                    Time = expenseRequest.Time,
                    Type = FinancialType.Expense,
					FinancialCategoryId = addedCategory.Id,
                    HomeId = expenseRequest.HomeId,
                    UserId = userId,
                };
            }
            else
            {
                expense = new FinancialRecord
                {
                    Title = expenseRequest.Title,
                    Amount = expenseRequest.Amount,
                    Description = expenseRequest.Description,
                    Time = expenseRequest.Time,
                    Type = FinancialType.Expense,
					FinancialCategoryId = expenseRequest.FinancialCategoryId,
                    HomeId = expenseRequest.HomeId,
                    UserId = userId,
                };
            }

            var hasBadge = await _gamificationRepository.UserHasCreateFirstExpenseBadge(userId);
            if (!hasBadge)
            {
                BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(userId);
                wallet.CreateFirstExpense = true;
                await _gamificationRepository.UpdateBadgeWallet(wallet);


                Notification notification = new Notification
                {
                    Title = $"You earned badge 'Create first expense'",
                    IsRead = false,
                    Time = DateTime.Now,
                    UserId = (int)userId,
                    User = user,
                };

                await _notificationRepository.CreateNotification(notification);
            }

            await _expenseRepository.AddExpense(expense);
			return Ok("Expense added successfully");
		}


		[HttpPut("expense/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateExpense(int id, ExpenseRequest expenseRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var expense = new FinancialRecord();

            if (expenseRequest.FinancialCategoryId == 0)
            {
				FinancialCategory category = await _expenseRepository.CheckCategory(expenseRequest.NewFinancialCategory);

				FinancialCategory addedCategory = new FinancialCategory();
				if (category == null)
				{
					FinancialCategory newfinancialCategory = new FinancialCategory
					{
						Name = expenseRequest.NewFinancialCategory,
						Type = FinancialType.Income,
						UserId = userId,
						HomeId = expenseRequest.HomeId
					};

					addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
				}
				else
				{
					addedCategory = category;
				}

				expense = new FinancialRecord
                {
                    Id = id,
                    Title = expenseRequest.Title,
                    Amount = expenseRequest.Amount,
                    Description = expenseRequest.Description,
                    Time = expenseRequest.Time,
                    Type = FinancialType.Expense,
                    FinancialCategoryId = addedCategory.Id,
                    HomeId = expenseRequest.HomeId,
                    UserId = userId,
                };
            }
            else
            {
                expense = new FinancialRecord
                {
                    Id = id,
                    Title = expenseRequest.Title,
                    Amount = expenseRequest.Amount,
                    Description = expenseRequest.Description,
                    Time = expenseRequest.Time,
                    Type = FinancialType.Expense,
					FinancialCategoryId = expenseRequest.FinancialCategoryId,
                    HomeId = expenseRequest.HomeId,
                    UserId = userId,
                };
            }

            await _expenseRepository.Update(expense);
			return Ok("Expense updated successfully");
		}

		[HttpGet("expense/{id}")]
		[Authorize]
		public async Task<IActionResult> GetExpenseById(int id)
		{
			var expense = await _expenseRepository.GetExpenseById(id);
			if (expense == null)
			{
				return NotFound($"Expense with ID {id} not found");
			}

			return Ok(expense);
		}

		[HttpDelete("expense/{id}")]
		[Authorize]
		public async Task<IActionResult> DeleteExpenseById(int id)
		{
			await _expenseRepository.Delete(id);
			return Ok($"Expense with ID {id} deleted successfully");
		}

		[HttpGet("totalBalance")]
		[Authorize]
		public async Task<IActionResult> GetCurrentMonthTotalBalance()
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalIncome(userId);
			decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalExpense(userId);
			decimal totalBalance = totalIncome - totalExpense;

			return Ok(totalBalance);
		}

        [HttpGet("totalBalance/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentMonthTotalBalance(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(id, userId);

            if (!isMember)
            {
                return Forbid();
            }
            decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalHomeIncome(id);
            decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalHomeExpense(id);
            decimal totalBalance = totalIncome - totalExpense;

            return Ok(totalBalance);
        }

        [HttpGet("monthlySummary")]
		[Authorize]
		public async Task<IActionResult> GetMonthlySummary()
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            DateTime currentDate = DateTime.UtcNow;
			DateTime startDate = currentDate.AddMonths(-12).Date;
			List<MonthlySummary> monthlySummaries = new List<MonthlySummary>();

			while (startDate < currentDate)
			{
				DateTime endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month)).Date;
				decimal totalIncome = await _incomeRepository.GetTotalIncomeForMonth(startDate, userId);
				decimal totalExpense = await _expenseRepository.GetTotalExpenseForMonth(startDate, userId);
				MonthlySummary summary = new MonthlySummary
				{
					MonthYear = startDate.ToString("yyyy-MM"),
					TotalIncome = totalIncome,
					TotalExpense = totalExpense
				};

				monthlySummaries.Add(summary);
				startDate = startDate.AddMonths(1);
			}

			return Ok(monthlySummaries);
		}

        [HttpGet("monthlySummary/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMonthlySummary(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(id, userId);

            if (!isMember)
            {
                return Forbid();
            }
            DateTime currentDate = DateTime.UtcNow;
            DateTime startDate = currentDate.AddMonths(-12).Date;
            List<MonthlySummary> monthlySummaries = new List<MonthlySummary>();

            while (startDate < currentDate)
            {
                DateTime endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month)).Date;
                decimal totalIncome = await _incomeRepository.GetTotalHomeIncomeForMonth(startDate, id);
                decimal totalExpense = await _expenseRepository.GetTotalHomeExpenseForMonth(startDate, id);
                MonthlySummary summary = new MonthlySummary
                {
                    MonthYear = startDate.ToString("yyyy-MM"),
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense
                };

                monthlySummaries.Add(summary);
                startDate = startDate.AddMonths(1);
            }

            return Ok(monthlySummaries);
        }

        [HttpGet("expenseCategories")]
        [Authorize]
        public async Task<IActionResult> GetExpenseCategories()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var categories = await _expenseRepository.GetExpenseCategories();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var count = await _expenseRepository.GetExpenseCountByCategory(category.Id, userId);
                if(count > 0)
                {
                    categoryCounts.Add(category.Name, count);
                }
            }

            return Ok(categoryCounts);
        }

        [HttpGet("expenseCategories/{id}")]
        [Authorize]
        public async Task<IActionResult> GetExpenseCategories(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(id, userId);

            if (!isMember)
            {
                return Forbid();
            }
            var categories = await _expenseRepository.GetExpenseCategories();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var count = await _expenseRepository.GetHomeExpenseCountByCategory(category.Id, id);
                if (count > 0)
                {
                    categoryCounts.Add(category.Name, count);
                }
            }

            return Ok(categoryCounts);
        }

        [HttpGet("incomeCategories/{id}")]
        [Authorize]
        public async Task<IActionResult> GetIncomeCategories(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            bool isMember = await _homeRepository.OrHomeMember(id, userId);

            if (!isMember)
            {
                return Forbid();
            }
            var categories = await _incomeRepository.GetIncomeCategories();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var count = await _incomeRepository.GetHomeIncomeCountByCategory(category.Id, id);
                if (count > 0)
                {
                    categoryCounts.Add(category.Name, count);
                }
            }

            return Ok(categoryCounts);
        }

        [HttpGet("incomeCategories")]
        [Authorize]
        public async Task<IActionResult> GetIncomeCategories()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var categories = await _incomeRepository.GetIncomeCategories();
            var categoryCounts = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var count = await _incomeRepository.GetIncomeCountByCategory(category.Id, userId);
				if (count > 0)
				{
					categoryCounts.Add(category.Name, count);
				}
			}

            return Ok(categoryCounts);
        }

        [HttpGet("CategoriesIncome")]
        [Authorize]
        public async Task<IActionResult> GetCategoriesIncome()
        {
            var categories = await _incomeRepository.GetIncomeCategories();

            return Ok(categories);
        }
        [HttpGet("CategoriesExpense")]
        [Authorize]
        public async Task<IActionResult> GetCategoriesExpense()
        {
            var categories = await _expenseRepository.GetExpenseCategories();

            return Ok(categories);
        }
        [HttpGet("generateReport")]
        public async Task<IActionResult> GenerateFinanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var incomes = await _incomeRepository.GetIncomesByDateRange(startDate, endDate, userId);
            var expenses = await _expenseRepository.GetExpensesByDateRange(startDate, endDate, userId);

            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                var title = new Paragraph("Financial Report")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20);
                document.Add(title);

                var dateRange = new Paragraph($"Date Range: {startDate.ToString("MM/dd/yyyy")} - {endDate.ToString("MM/dd/yyyy")}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(14);
                document.Add(dateRange);

                var incomesSection = new Paragraph("Incomes");
                document.Add(incomesSection);

                if (incomes.Count != 0)
                {
                    Table incomeTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                    incomeTable.AddHeaderCell("Title");
                    incomeTable.AddHeaderCell("Amount");
                    incomeTable.AddHeaderCell("Date");

                    foreach (var income in incomes)
                    {
                        incomeTable.AddCell(income.Title);
                        incomeTable.AddCell(income.Amount.ToString("C"));
                        incomeTable.AddCell(income.Time.ToString("MM/dd/yyyy"));
                    }

                    document.Add(incomeTable);
                }
                else
                {
                    var noIncomeItem = new Paragraph("No income data available")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .SetFontSize(12);
                    document.Add(noIncomeItem);
                }

                var expensesSection = new Paragraph("Expenses");
                document.Add(expensesSection);

                if (expenses.Count != 0)
                {
                    Table expenseTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                    expenseTable.AddHeaderCell("Title");
                    expenseTable.AddHeaderCell("Amount");
                    expenseTable.AddHeaderCell("Date");

                    foreach (var expense in expenses)
                    {
                        expenseTable.AddCell(expense.Title);
                        expenseTable.AddCell(expense.Amount.ToString("C"));
                        expenseTable.AddCell(expense.Time.ToString("MM/dd/yyyy"));
                    }

                    document.Add(expenseTable);
                }
                else
                {
                    var noExpenseItem = new Paragraph("No expense data available")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .SetFontSize(12);
                    document.Add(noExpenseItem);
                }

                var totalIncome = incomes.Sum(x => x.Amount);
                var totalExpense = expenses.Sum(x => x.Amount);

                var summary = new Paragraph($"Total Income: {totalIncome:C} | Total Expense: {totalExpense:C}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(14);
                document.Add(summary);


                document.Close();

                var pdfBytes = stream.ToArray();

                return File(pdfBytes, "application/pdf", "financial_report.pdf");
            }
        }

    }
}
