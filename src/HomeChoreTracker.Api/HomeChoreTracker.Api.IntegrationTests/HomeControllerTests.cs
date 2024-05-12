using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Contracts.User;
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
	public class HomeControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Home";

		public HomeControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetHome_ValidCredentials_RetursHomeResponse()
		{
			// Arrange
			int id = 1;
			string uri = baseUri + $"/Level/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<LevelRequest>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.GamificationLevelId.Should().Be(2);
		}

		[Fact]
		public async Task GetHomes_ValidCredentials_RetursHomesResponse()
		{
			// Arrange
			string uri = baseUri;
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<Home>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(3);
		}

		[Fact]
		public async Task GetMaxLevel_ValidCredentials_RetursMaxLevelResponse()
		{
			// Arrange
			string uri = baseUri + "/Level";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<int>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(8);
		}

		[Fact]
		public async Task GetHomePlant_ValidCredentials_RetursHomePlantResponse()
		{
			// Arrange
			int homeId = 1;
			string uri = baseUri + $"/Level/{homeId}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<LevelRequest>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.GamificationLevelId.Should().Be(2);
		}

		[Fact]
		public async Task GetPointsHistory_ValidCredentials_RetursPointsHistoryResponse()
		{
			// Arrange
			int homeId = 1;
			int skip = 0;
			int take = 10;
			string uri = baseUri + $"/{homeId}/skip{skip}/take{take}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<PointsResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(10);
		}

		[Fact]
		public async Task GetHomeMembers_ValidCredentials_RetursHomeMembersResponse()
		{
			// Arrange
			int homeId = 1;
			string uri = baseUri + $"/HomeMembers?homeId={homeId}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<UserGetResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(2);
		}
	}
}
