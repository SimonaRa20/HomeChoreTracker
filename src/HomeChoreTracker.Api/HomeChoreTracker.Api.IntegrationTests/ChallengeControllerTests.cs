using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Challenge;
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
	public class ChallengeControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Challenge";

		public ChallengeControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetUsers_ValidCredentials_RetursUsersResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/OpponentsUsers";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<User>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(3);
		}

		[Fact]
		public async Task GetUserHomes_ValidCredentials_RetursUserHomesResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/UserHomes";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<Home>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(3);
		}

		[Fact]
		public async Task GetOpponentsHomes_ValidCredentials_RetursOpponentsHomesResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/OpponentsHomes";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<Home>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(1);
		}

		[Fact]
		public async Task GetReceivedChallenges_ValidCredentials_RetursReceivedChallengesResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/ReceivedChallenges";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<ReceivedChallengeResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(1);
		}

		[Fact]
		public async Task GetCurrentChallenges_ValidCredentials_RetursCurrentChallengesResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/CurrentChallenges";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<CurrentChallengeResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(1);
		}

		[Fact]
		public async Task GetHistoryChallenges_ValidCredentials_RetursHistoryChallengesResponse()
		{
			// Arrange
			int id = 2;
			string uri = baseUri + "/HistoryChallenges";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<HistoryChallengeResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(1);
		}
	}
}
