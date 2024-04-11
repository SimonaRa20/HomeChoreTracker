using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.IntegrationTests.Shared;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.IntegrationTests.Extensions
{
	public static class ClientExtensions
	{
		public static async Task LoginAsUser(this CustomHttpClient client)
		{
			await client.AddJwtToken("user1@gmail.com", "simona123");
		}

		public static async Task LoginAsAdministrator(this CustomHttpClient client)
		{
			await client.AddJwtToken("admin@gmail.com", "simona123");
		}

		public static async Task AddJwtToken(this CustomHttpClient client, string email, string password)
		{
			string uri = "Auth/Login";

			var loginRequest = new UserLoginRequest
			{
				Email = email,
				Password = password
			};

			var response = await client.Post<UserLoginRequest, UserLoginResponse>(uri, loginRequest);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			client.AddBearerToken(response.Data.Token);
		}

		public static CustomHttpClient GetCustomHttpClient(this WebApplicationFactory<Program> factory)
		{
			return new CustomHttpClient(factory.CreateClient());
		}
	}
}
