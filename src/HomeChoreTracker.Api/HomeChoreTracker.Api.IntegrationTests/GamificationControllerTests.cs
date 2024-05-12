using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace HomeChoreTracker.Api.IntegrationTests
{
	[Collection("Sequential")]
	public class GamificationControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Gamification";

		public GamificationControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetGameLevels_ValidCredentials_RetursGamificationLevelsResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri;
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsAdministrator();

			// Act
			var response = await client.Get<List<GamificationLevelResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(8);
		}

		[Fact]
		public async Task GetGameLevel_ValidCredentials_RetursGamificationLevelResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + $"/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsAdministrator();

			// Act
			var response = await client.Get<GamificationLevelResponse>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Id.Should().Be(2);
			response.Data.PointsRequired.Should().Be(50);
		}

		[Fact]
		public async Task GetBadgeWallet_ValidCredentials_RetursBadgeWalletResponse()
		{
			// Arrange
			string uri = baseUri + "/BadgeWallet";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<BadgeWalletResponse>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Id.Should().Be(2);
		}

		[Fact]
		public async Task GetRatingsByPoints_ValidCredentials_RetursRatingsByPointsResponse()
		{
			// Arrange
			string uri = baseUri + "/RatingsByPoints";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<RatingsResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(8);
		}

		[Fact]
		public async Task GetRatingsByBadges_ValidCredentials_RetursRatingsByBadgesResponse()
		{
			// Arrange
			string uri = baseUri + "/RatingsByBadges";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<RatingsResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(8);
		}
	}
}