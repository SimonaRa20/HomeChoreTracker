using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.IntegrationTests.Extensions;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.IntegrationTests
{
	[Collection("Sequential")]
	public class FinanceControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Finance";

		public FinanceControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetCurrentMonthTotalIncome_ValidCredentials_RetursCurrentMonthTotalIncomeResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalIncome";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(800.00M);
		}

		[Fact]
		public async Task GetHomeCurrentMonthTotalIncome_ValidCredentials_RetursHomeCurrentMonthTotalIncomeResponse()
		{
			// Arrange
			int homeId = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalIncome/{homeId}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(1800.00M);
		}

		[Fact]
		public async Task GetIncomeById_ValidCredentials_RetursIncomeByIdResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/income/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<IncomeResponse>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Title.Should().Be("For birthday");
		}

		[Fact]
		public async Task GetCurrentMonthTotalExpense_ValidCredentials_RetursCurrentMonthTotalExpenseResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalExpense";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(124.00M);
		}

		[Fact]
		public async Task GetHomeCurrentMonthTotalExpense_ValidCredentials_RetursHomeCurrentMonthTotalExpenseResponse()
		{
			// Arrange
			int homeId = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalExpense/{homeId}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(544.00M);
		}

		[Fact]
		public async Task GetTransferHistory_ValidCredentials_RetursTransferHistoryResponse()
		{
			// Arrange
			int skip = 0;
			int take = 10;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/transferHistory/skip{skip}/take{take}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<TransferHistoryItem>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(10);
		}

		[Fact]
		public async Task GetTransfersHistory_ValidCredentials_RetursTransfersHistoryResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/transferHistory";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<TransferHistoryItem>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(13);
		}

		[Fact]
		public async Task GetExpenseById_ValidCredentials_RetursExpenseByIdResponse()
		{
			// Arrange
			int id = 2;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/expense/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<ExpenseResponse>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Title.Should().Be("New jacket");
		}

		[Fact]
		public async Task GetCurrentMonthTotalBalance_ValidCredentials_RetursCurrentMonthTotalBalanceResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalBalance";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(676.00M);
		}

		[Fact]
		public async Task GetHomeCurrentMonthTotalBalance_ValidCredentials_RetursHomeCurrentMonthTotalBalanceResponse()
		{
			// Arrange
			int homeId = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/totalBalance/{homeId}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<decimal>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(1256.00M);
		}

		[Fact]
		public async Task GetExpenseCategories_ValidCredentials_RetursExpenseCategoriesResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/expenseCategories";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Dictionary<string, int>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(4);
		}

		[Fact]
		public async Task GetHomeExpenseCategories_ValidCredentials_RetursHomeExpenseCategoriesResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/expenseCategories/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Dictionary<string, int>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(3);
		}

		[Fact]
		public async Task GetHomeIncomeCategories_ValidCredentials_RetursHomeIncomeCategoriesResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/incomeCategories/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Dictionary<string, int>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(2);
		}

		[Fact]
		public async Task GetIncomeCategories_ValidCredentials_RetursIncomeCategoriesResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/incomeCategories";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Dictionary<string, int>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(2);
		}

		[Fact]
		public async Task GetCategoriesIncome_ValidCredentials_RetursCategoriesIncomeResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/CategoriesIncome";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<FinancialCategory>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(2);
		}

		[Fact]
		public async Task GetCategoriesExpense_ValidCredentials_RetursCategoriesExpenseResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/CategoriesExpense";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<FinancialCategory>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(4);
		}

		[Fact]
		public async Task GenerateFinanceReport_ValidCredentials_RetursGenerateFinanceReportResponse()
		{
			// Arrange
			DateTime startDate = DateTime.Now.AddMonths(-3);
			DateTime endDate = DateTime.Now;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/generateReport?startDate={startDate}&endDate={endDate}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<byte[]>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}
