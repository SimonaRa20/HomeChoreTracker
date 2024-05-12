using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.IntegrationTests.Extensions;
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
	public class UserControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "User";

		public UserControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetUserData_ValidCredentials_RetursUserProfileResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<UserGetResponse>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Email.Should().Be("user1@gmail.com");
			response.Data.UserName.Should().Be("Jonas Jonaitis");
		}

		[Fact]
		public async Task GetUserBusyItervals_ValidCredentials_RetursUserBusyItervalsResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/BusyIntervals";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<BusyIntervalResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(7);
		}
	}
}
