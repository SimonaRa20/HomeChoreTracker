using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class FinanceControllerTests
	{
		private readonly FinanceController _financeController;
		private readonly Mock<IIncomeRepository> _incomeRepositoryMock;
		private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
		private readonly Mock<IGamificationRepository> _gamificationRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<INotificationRepository> _notificationRepositoryMock;
		private readonly Mock<IHomeRepository> _homeRepositoryMock;

		public FinanceControllerTests()
		{
			_incomeRepositoryMock = new Mock<IIncomeRepository>();
			_expenseRepositoryMock = new Mock<IExpenseRepository>();
			_gamificationRepositoryMock = new Mock<IGamificationRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_homeRepositoryMock = new Mock<IHomeRepository>();

			_financeController = new FinanceController(
				_incomeRepositoryMock.Object,
				_expenseRepositoryMock.Object,
				_homeRepositoryMock.Object,
				_gamificationRepositoryMock.Object,
				_userRepositoryMock.Object,
				_notificationRepositoryMock.Object
			);
		}

		[Fact]
		public async Task GetCurrentMonthTotalIncome_Returns_OkResult_With_TotalIncome()
		{
			// Arrange
			var userId = 1;
			var expectedTotalIncome = 1000.0m;
			_incomeRepositoryMock.Setup(repo => repo.GetCurrentMonthTotalIncome(userId))
				.ReturnsAsync(expectedTotalIncome);

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.GetCurrentMonthTotalIncome();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedTotalIncome, okResult.Value);
		}

		[Fact]
		public async Task GetCurrentMonthTotalExpense_Returns_OkResult_With_TotalExpense()
		{
			// Arrange
			var userId = 1;
			var expectedTotalExpense = 0m;
			_expenseRepositoryMock.Setup(repo => repo.GetCurrentMonthTotalExpense(userId))
				.ReturnsAsync(expectedTotalExpense);

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.GetCurrentMonthTotalIncome();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedTotalExpense, okResult.Value);
		}

		[Fact]
		public async Task AddIncome_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var category = new FinancialCategory
			{
				Id = 1,
				Name = "Test Category",
				Type = FinancialType.Income
			};
			var badgeWallet = new BadgeWallet();
			_incomeRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync(category);
			_incomeRepositoryMock.Setup(repo => repo.AddIncome(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			var incomeRequest = new IncomeRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = category.Id,
				HomeId = 1
			};

			var expectedMessage = "Income added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddIncome(incomeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task AddExpense_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var category = new FinancialCategory
			{
				Id = 1,
				Name = "Test Category",
				Type = FinancialType.Expense
			};
			var badgeWallet = new BadgeWallet();
			_expenseRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync(category);
			_expenseRepositoryMock.Setup(repo => repo.AddExpense(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			var expenseRequest = new ExpenseRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = category.Id,
				HomeId = 1
			};

			var expectedMessage = "Expense added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddExpense(expenseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task AddIncomeCategoryZero_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var badgeWallet = new BadgeWallet();
			_incomeRepositoryMock.Setup(repo => repo.AddIncome(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			_incomeRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>()))
				.ReturnsAsync((FinancialCategory)null);

			_incomeRepositoryMock.Setup(repo => repo.AddCategory(It.IsAny<FinancialCategory>()))
				.ReturnsAsync(new FinancialCategory { Id = 1 });

			var incomeRequest = new IncomeRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "Birthday"
			};

			var expectedMessage = "Income added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddIncome(incomeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task AddExpenseCategoryZero_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var badgeWallet = new BadgeWallet();
			_expenseRepositoryMock.Setup(repo => repo.AddExpense(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			_expenseRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>()))
				.ReturnsAsync((FinancialCategory)null);

			_incomeRepositoryMock.Setup(repo => repo.AddCategory(It.IsAny<FinancialCategory>()))
				.ReturnsAsync(new FinancialCategory { Id = 1 });

			var expenseRequest = new ExpenseRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "Birthday"
			};

			var expectedMessage = "Expense added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddExpense(expenseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task AddIncomeNewCategory_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var category = new FinancialCategory
			{
				Id = 1, 
				Name = "Test Category",
				Type = FinancialType.Income
			};
			var badgeWallet = new BadgeWallet();
			_incomeRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync(category);
			_incomeRepositoryMock.Setup(repo => repo.AddIncome(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			var incomeRequest = new IncomeRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "hihi"
			};

			var expectedMessage = "Income added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddIncome(incomeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task AddExpenseNewCategory_Returns_OkResult_When_Successfully_Added()
		{
			// Arrange
			var userId = 1;
			var category = new FinancialCategory
			{
				Id = 1,
				Name = "Test Category",
				Type = FinancialType.Expense
			};
			var badgeWallet = new BadgeWallet();
			_expenseRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync(category);
			_incomeRepositoryMock.Setup(repo => repo.AddIncome(It.IsAny<FinancialRecord>()));
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(badgeWallet);

			var expenseRequest = new ExpenseRequest
			{
				Title = "Salary",
				Amount = 2000.0m,
				Description = "Monthly salary",
				Time = DateTime.Now,
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "hihi"
			};

			var expectedMessage = "Expense added successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.AddExpense(expenseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task UpdateIncome_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var userId = 1;
			var categoryId = 1;
			var incomeId = 123;
			var updatedIncome = new FinancialRecord
			{
				Id = incomeId,
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = categoryId,
				HomeId = 1,
				UserId = userId
			};

			var updateIncome = new IncomeResponse
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = categoryId,
				HomeId = 1,
			};

			_incomeRepositoryMock.Setup(repo => repo.GetIncomeById(incomeId)).ReturnsAsync(updateIncome);
			_incomeRepositoryMock.Setup(repo => repo.Update(updatedIncome));

			var incomeRequest = new IncomeRequest
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now.AddDays(-5),
				FinancialCategoryId = categoryId,
				HomeId = 1
			};

			var expectedMessage = "Income updated successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.UpdateIncome(incomeId, incomeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task UpdateExpense_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var userId = 1;
			var categoryId = 1;
			var expenseId = 123;
			var updatedExpense = new FinancialRecord
			{
				Id = expenseId,
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Expense,
				FinancialCategoryId = categoryId,
				HomeId = 1,
				UserId = userId
			};

			var updateExpense = new ExpenseResponse
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Expense,
				FinancialCategoryId = categoryId,
				HomeId = 1,
			};

			_expenseRepositoryMock.Setup(repo => repo.GetExpenseById(expenseId)).ReturnsAsync(updateExpense);
			_expenseRepositoryMock.Setup(repo => repo.Update(updatedExpense));

			var expenseRequest = new ExpenseRequest
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now.AddDays(-5),
				FinancialCategoryId = categoryId,
				HomeId = 1
			};

			var expectedMessage = "Expense updated successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.UpdateExpense(expenseId, expenseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task UpdateIncomeCategoryZero_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var userId = 1;
			var categoryId = 1;
			var incomeId = 123;
			var updateIncome = new IncomeResponse
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Income,
				FinancialCategoryId = categoryId,
				HomeId = 1,
			};

			_incomeRepositoryMock.Setup(repo => repo.GetIncomeById(incomeId)).ReturnsAsync(updateIncome);
			_incomeRepositoryMock.Setup(repo => repo.Update(It.IsAny<FinancialRecord>()));
			_incomeRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync((FinancialCategory)null);
			_incomeRepositoryMock.Setup(repo => repo.AddCategory(It.IsAny<FinancialCategory>()))
				.ReturnsAsync(new FinancialCategory { Id = 2 });

			var incomeRequest = new IncomeRequest
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now.AddDays(-5),
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "New Category"
			};

			var expectedMessage = "Income updated successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.UpdateIncome(incomeId, incomeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task UpdateExpenseCategoryZero_Returns_OkResult_When_Successfully_Updated()
		{
			// Arrange
			var userId = 1;
			var categoryId = 1;
			var expenseId = 123;
			var updateExpense = new ExpenseResponse
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now,
				Type = FinancialType.Expense,
				FinancialCategoryId = categoryId,
				HomeId = 1,
			};

			_expenseRepositoryMock.Setup(repo => repo.GetExpenseById(expenseId)).ReturnsAsync(updateExpense);
			_expenseRepositoryMock.Setup(repo => repo.Update(It.IsAny<FinancialRecord>()));
			_expenseRepositoryMock.Setup(repo => repo.CheckCategory(It.IsAny<string>())).ReturnsAsync((FinancialCategory)null);
			_incomeRepositoryMock.Setup(repo => repo.AddCategory(It.IsAny<FinancialCategory>()))
				.ReturnsAsync(new FinancialCategory { Id = 2 });

			var expenseRequest = new ExpenseRequest
			{
				Title = "Updated Salary",
				Amount = 2500.0m,
				Description = "Updated monthly salary",
				Time = DateTime.Now.AddDays(-5),
				FinancialCategoryId = 0,
				HomeId = 1,
				NewFinancialCategory = "New Category"
			};

			var expectedMessage = "Expense updated successfully";

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.UpdateExpense(expenseId, expenseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(expectedMessage, okResult.Value);
		}

		[Fact]
		public async Task DeleteIncomeById_Returns_OkResult_When_Successfully_Deleted()
		{
			// Arrange
			var userId = 1;
			var incomeId = 123;

			_incomeRepositoryMock.Setup(repo => repo.Delete(incomeId));

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.DeleteFinanceById(incomeId, "income");

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Income with ID {incomeId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task DeleteByIdExpense_Returns_OkResult_When_Successfully_Deleted()
		{
			// Arrange
			var userId = 1;
			var expenseId = 123;

			_expenseRepositoryMock.Setup(repo => repo.Delete(expenseId));

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.DeleteExpenseById(expenseId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Expense with ID {expenseId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task DeleteByIdIncome_Returns_OkResult_When_Successfully_Deleted()
		{
			// Arrange
			var userId = 1;
			var incomeId = 123;

			_incomeRepositoryMock.Setup(repo => repo.Delete(incomeId));

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.DeleteIncomeById(incomeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Income with ID {incomeId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task DeleteExpenseById_Returns_OkResult_When_Successfully_Deleted()
		{
			// Arrange
			var userId = 1;
			var expenseId = 123;

			_expenseRepositoryMock.Setup(repo => repo.Delete(expenseId));

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.DeleteFinanceById(expenseId, "expense");

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal($"Expense with ID {expenseId} deleted successfully", okResult.Value);
		}

		[Fact]
		public async Task GetTransferHistory_Returns_OkResult_With_TransferHistory()
		{
			// Arrange
			var userId = 1;
			var homeId = 123;
			var income1 = new FinancialRecord
			{
				Id = 1,
				Title = "Income 1",
				Amount = 100.0m,
				Description = "Income description 1",
				Time = DateTime.Now.AddDays(-1),
				FinancialCategoryId = 1,
				Type = FinancialType.Income,
				UserId = userId
			};
			var income2 = new FinancialRecord
			{
				Id = 2,
				Title = "Income 2",
				Amount = 200.0m,
				Description = "Income description 2",
				Time = DateTime.Now.AddDays(-2),
				FinancialCategoryId = 2,
				Type = FinancialType.Income,
				UserId = userId
			};
			var expense1 = new FinancialRecord
			{
				Id = 1,
				Title = "Expense 1",
				Amount = 50.0m,
				Description = "Expense description 1",
				Time = DateTime.Now.AddDays(-3),
				FinancialCategoryId = 1,
				Type = FinancialType.Expense,
				UserId = userId
			};
			var expense2 = new FinancialRecord
			{
				Id = 2,
				Title = "Expense 2",
				Amount = 75.0m,
				Description = "Expense description 2",
				Time = DateTime.Now.AddDays(-4),
				FinancialCategoryId = 2,
				Type = FinancialType.Expense,
				UserId = userId
			};

			var incomes = new List<FinancialRecord> { income1, income2 };
			var expenses = new List<FinancialRecord> { expense1, expense2 };

			_incomeRepositoryMock.Setup(repo => repo.GetHomeAll(homeId)).ReturnsAsync(incomes);
			_expenseRepositoryMock.Setup(repo => repo.GetHomeAll(homeId)).ReturnsAsync(expenses);

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.GetTransferHistory(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var transferHistory = Assert.IsType<List<TransferHistoryItem>>(okResult.Value);
			Assert.Equal(4, transferHistory.Count); 
		}

		[Fact]
		public async Task GetExpenseById_Returns_OkResult_With_Expense()
		{
			// Arrange
			var userId = 1;
			var expenseId = 123;
			var expectedExpense = new ExpenseResponse
			{
				Title = "Test Expense",
				Amount = 50.0m,
				Description = "Test expense description",
				Time = DateTime.Now,
				Type = FinancialType.Expense,
				FinancialCategoryId = 1,
			};

			_expenseRepositoryMock.Setup(repo => repo.GetExpenseById(expenseId)).ReturnsAsync(expectedExpense);

			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _financeController.GetExpenseById(expenseId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var expense = Assert.IsType<ExpenseResponse>(okResult.Value);
			Assert.Equal(expectedExpense.Title, expense.Title);
			Assert.Equal(expectedExpense.Amount, expense.Amount);
			Assert.Equal(expectedExpense.Description, expense.Description);
			Assert.Equal(expectedExpense.Time, expense.Time);
			Assert.Equal(expectedExpense.Type, expense.Type);
			Assert.Equal(expectedExpense.FinancialCategoryId, expense.FinancialCategoryId);
		}
		[Fact]
		public async Task GetMonthlySummary_Returns_OkResult_With_MonthlySummaries_ForCurrentUser()
		{
			// Arrange
			int userId = 1;
			var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};

			_incomeRepositoryMock.Setup(repo => repo.GetTotalIncomeForMonth(It.IsAny<DateTime>(), userId))
				.ReturnsAsync(1000.0m);
			_expenseRepositoryMock.Setup(repo => repo.GetTotalExpenseForMonth(It.IsAny<DateTime>(), userId))
				.ReturnsAsync(500.0m);

			// Act
			var result = await _financeController.GetMonthlySummary();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var monthlySummaries = Assert.IsType<List<MonthlySummary>>(okResult.Value);
			Assert.Equal(13, monthlySummaries.Count);
			foreach (var summary in monthlySummaries)
			{
				Assert.Equal(1000.0m, summary.TotalIncome);
				Assert.Equal(500.0m, summary.TotalExpense);
			}
		}

		[Fact]
		public async Task GetMonthlySummary_Returns_OkResult_With_MonthlySummaries_ForSpecifiedHome()
		{
			// Arrange
			int userId = 1;
			int homeId = 123;
			var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};

			_homeRepositoryMock.Setup(repo => repo.OrHomeMember(homeId, userId)).ReturnsAsync(true);
			_incomeRepositoryMock.Setup(repo => repo.GetTotalHomeIncomeForMonth(It.IsAny<DateTime>(), homeId))
				.ReturnsAsync(1500.0m);
			_expenseRepositoryMock.Setup(repo => repo.GetTotalHomeExpenseForMonth(It.IsAny<DateTime>(), homeId))
				.ReturnsAsync(800.0m);

			// Act
			var result = await _financeController.GetMonthlySummary(homeId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var monthlySummaries = Assert.IsType<List<MonthlySummary>>(okResult.Value);
			Assert.Equal(13, monthlySummaries.Count);
			foreach (var summary in monthlySummaries)
			{
				Assert.Equal(1500.0m, summary.TotalIncome);
				Assert.Equal(800.0m, summary.TotalExpense);
			}
		}

		[Fact]
		public async Task GetIncomeById_Returns_NotFound_When_Income_Not_Found()
		{
			// Arrange
			var incomeId = 123;
			_incomeRepositoryMock.Setup(repo => repo.GetIncomeById(incomeId)).ReturnsAsync((IncomeResponse)null);

			// Act
			var result = await _financeController.GetIncomeById(incomeId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Income with ID {incomeId} not found", notFoundResult.Value);
		}

		[Fact]
		public async Task GetExpenseById_Returns_NotFound_When_Expense_Not_Found()
		{
			// Arrange
			var expenseId = 123;
			_expenseRepositoryMock.Setup(repo => repo.GetExpenseById(expenseId)).ReturnsAsync((ExpenseResponse)null);

			// Act
			var result = await _financeController.GetExpenseById(expenseId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal($"Expense with ID {expenseId} not found", notFoundResult.Value);
		}

		[Fact]
		public async Task GetCurrentMonthTotalBalance_Forbidden_When_User_Not_Home_Member()
		{
			// Arrange
			int userId = 1;
			int homeId = 123;
			var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
			_financeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};

			_homeRepositoryMock.Setup(repo => repo.OrHomeMember(homeId, userId)).ReturnsAsync(false);

			// Act
			var result = await _financeController.GetCurrentMonthTotalBalance(homeId);

			// Assert
			var forbiddenResult = Assert.IsType<ForbidResult>(result);
			Assert.NotNull(forbiddenResult);
		}

	}
}
