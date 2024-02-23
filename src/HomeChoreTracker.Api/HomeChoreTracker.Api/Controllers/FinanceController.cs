using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace HomeChoreTracker.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class FinanceController : ControllerBase
	{
		private readonly IIncomeRepository _incomeRepository;
		private readonly IExpenseRepository _expenseRepository;

		public FinanceController(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository)
		{
			_incomeRepository = incomeRepository;
			_expenseRepository = expenseRepository;
		}

		[HttpGet("totalIncome")]
		[Authorize]
		public async Task<IActionResult> GetCurrentMonthTotalIncome()
		{
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalIncome(userId);
			return Ok(totalIncome);
		}

		[HttpPost("income")]
		[Authorize]
		public async Task<IActionResult> AddIncome(IncomeRequest incomeRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var income = new Income
			{
				Title = incomeRequest.Title,
				Amount = incomeRequest.Amount,
				Description = incomeRequest.Description,
				Time = incomeRequest.Time,
				Type = incomeRequest.Type,
				HomeId = incomeRequest.HomeId,
				UserId = userId,
			};

			await _incomeRepository.AddIncome(income);
			return Ok("Income added successfully");
		}

		[HttpPut("income/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateIncome(int id, IncomeRequest incomeRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var income = new Income
			{
				Id = id,
				Title = incomeRequest.Title,
				Amount = incomeRequest.Amount,
				Description = incomeRequest.Description,
				Time = incomeRequest.Time,
				Type = incomeRequest.Type,
				HomeId = incomeRequest?.HomeId,
				UserId = userId
			};

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
                        UserId = income.UserId
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
                        SubscriptionDuration = expense.SubscriptionDuration,
                        UserId = expense.UserId
                    }
                });
            }

            // Order transfer history by time in ascending order
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
                        Type = (int)income.Type,
                        UserId = income.UserId
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
                        SubscriptionDuration = expense.SubscriptionDuration,
                        UserId = expense.UserId
                    }
                });
            }

            // Order transfer history by time in ascending order
            transferHistory = transferHistory.OrderByDescending(item => item.Data.Time).ToList();

            return Ok(transferHistory);
        }


        [HttpPost("expense")]
		[Authorize]
		public async Task<IActionResult> AddExpense(ExpenseRequest expenseRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var expense = new Expense
			{
				Title = expenseRequest.Title,
				Amount = expenseRequest.Amount,
				Description = expenseRequest.Description,
				Time = expenseRequest.Time,
				Type = expenseRequest.Type,
				SubscriptionDuration = expenseRequest.SubscriptionDuration,
				HomeId = expenseRequest.HomeId,
				UserId = userId,
			};

			await _expenseRepository.AddExpense(expense);
			return Ok("Expense added successfully");
		}


		[HttpPut("expense/{id}")]
		[Authorize]
		public async Task<IActionResult> UpdateExpense(int id, ExpenseRequest expenseRequest)
		{
			int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			var expense = new Expense
			{
				Id = id,
				Title = expenseRequest.Title,
				Amount = expenseRequest.Amount,
				Description = expenseRequest.Description,
				Time = expenseRequest.Time,
				Type = expenseRequest.Type,
				SubscriptionDuration = expenseRequest.SubscriptionDuration,
				HomeId = expenseRequest.HomeId,
				UserId = userId,
			};

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

        [HttpGet("expenseCategories")]
        [Authorize]
        public async Task<IActionResult> GetExpenseCategories()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var categories = Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>();
            var categoryCounts = new Dictionary<ExpenseType, int>();

            foreach (var category in categories)
            {
                var count = await _expenseRepository.GetExpenseCountByCategory(category, userId);
                categoryCounts.Add(category, count);
            }

            return Ok(categoryCounts);
        }

        [HttpGet("incomeCategories")]
        [Authorize]
        public async Task<IActionResult> GetIncomeCategories()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var categories = Enum.GetValues(typeof(IncomeType)).Cast<IncomeType>();
            var categoryCounts = new Dictionary<IncomeType, int>();

            foreach (var category in categories)
            {
                var count = await _incomeRepository.GetIncomeCountByCategory(category, userId);
                categoryCounts.Add(category, count);
            }

            return Ok(categoryCounts);
        }

        [HttpGet("generateReport")]
        public async Task<IActionResult> GenerateFinanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            // Retrieve data for the report (in your case, incomes and expenses)
            var incomes = await _incomeRepository.GetIncomesByDateRange(startDate, endDate, userId);
            var expenses = await _expenseRepository.GetExpensesByDateRange(startDate, endDate, userId);

            // Create a memory stream to store the generated PDF
            using (var stream = new MemoryStream())
            {
                // Initialize PDF writer and document
                PdfWriter writer = new(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Add title to the document
                var title = new Paragraph("Financial Report")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20);
                document.Add(title);

                // Add date range to the document
                var dateRange = new Paragraph($"Date Range: {startDate.ToString("MM/dd/yyyy")} - {endDate.ToString("MM/dd/yyyy")}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(14);
                document.Add(dateRange);

                // Add incomes section to the document
                var incomesSection = new Paragraph("Incomes");
                document.Add(incomesSection);

                if (incomes.Count != 0)
                {
                    // Add a table to display income details
                    Table incomeTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                    // Add table header
                    incomeTable.AddHeaderCell("Title");
                    incomeTable.AddHeaderCell("Amount");
                    incomeTable.AddHeaderCell("Date");

                    // Add income details to the table
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

                // Add expenses section to the document
                var expensesSection = new Paragraph("Expenses");
                document.Add(expensesSection);

                if (expenses.Count != 0)
                {
                    // Add a table to display expense details
                    Table expenseTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                    // Add table header
                    expenseTable.AddHeaderCell("Title");
                    expenseTable.AddHeaderCell("Amount");
                    expenseTable.AddHeaderCell("Date");

                    // Add expense details to the table
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

                // Add total income and total expense summary
                var totalIncome = incomes.Sum(x => x.Amount);
                var totalExpense = expenses.Sum(x => x.Amount);

                var summary = new Paragraph($"Total Income: {totalIncome:C} | Total Expense: {totalExpense:C}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(14);
                document.Add(summary);

                // Close the document
                document.Close();

                // Get the bytes of the generated PDF
                var pdfBytes = stream.ToArray();

                // Return the PDF file
                return File(pdfBytes, "application/pdf", "financial_report.pdf");
            }
        }

    }
}
