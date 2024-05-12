using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Identity.Data;
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
	public class AuthControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Auth";

		public AuthControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task LoginJwt_ValidCredentials_RetursAccessAndRefreshToken()
		{
			// Arrange
			string uri = baseUri + "/Login";
			var client = _factory.GetCustomHttpClient();

			var loginRequest = new UserLoginRequest
			{
				Email = "user1@gmail.com",
				Password = "simona123"
			};

			// Act
			var response = await client.Post<UserLoginRequest, UserLoginResponse>(uri, loginRequest);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Token.Should().NotBeNullOrEmpty();
			response.Data.UserName.Should().Be("Jonas Jonaitis");
		}
	}
}
