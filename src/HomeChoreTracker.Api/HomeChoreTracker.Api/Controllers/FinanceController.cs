using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
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
			decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalIncome();
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
			decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalExpense();
			return Ok(totalExpense);
		}

        [HttpGet("transferHistory/skip{skip}/take{take}")]
        [Authorize]
        public async Task<IActionResult> GetTransferHistory(int skip = 0, int take = 5)
        {
            var incomes = await _incomeRepository.GetAll();
            var expenses = await _expenseRepository.GetAll();

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
            var incomes = await _incomeRepository.GetAll();
            var expenses = await _expenseRepository.GetAll();

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
			decimal totalIncome = await _incomeRepository.GetCurrentMonthTotalIncome();
			decimal totalExpense = await _expenseRepository.GetCurrentMonthTotalExpense();
			decimal totalBalance = totalIncome - totalExpense;

			return Ok(totalBalance);
		}
	}
}
