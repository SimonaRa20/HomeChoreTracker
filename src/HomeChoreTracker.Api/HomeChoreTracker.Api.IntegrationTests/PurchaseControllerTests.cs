using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.IntegrationTests.Extensions;
using HomeChoreTracker.Api.Models;
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
	public class PurchaseControllerTests
    {
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Purchase";

		public PurchaseControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetPurchases_ValidCredentials_ReturnsPurchasesResponse()
		{
			// Arrange
			int homeId = 1;
			string uri = baseUri + $"/{homeId}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<Purchase>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(6);
		}

		[Fact]
		public async Task GetPurchase_ValidCredentials_ReturnsPurchaseResponse()
		{
			// Arrange
			int purchaseId = 1;
			string uri = baseUri + $"/purchase/{purchaseId}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Purchase>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Items.Count.Should().Be(3);
		}
	}
}
