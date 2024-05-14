using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Avatar;
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
	public class AvatarControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Avatar";

		public AvatarControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetAvatars_ValidCredentials_RetursAvatarsResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri;
			await client.LoginAsAdministrator();

			// Act
			var response = await client.Get<List<AvatarResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(19);
		}

		[Fact]
		public async Task GetToUserAvatars_ValidCredentials_RetursAvatarsResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + "/GetAvatars";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<AvatarSelectionResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(19);
		}

		[Fact]
		public async Task GetUserAvatar_ValidCredentials_RetursAvatarResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + "/GetUserAvatar";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Avatar>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Id.Should().Be(2);
		}

		[Fact]
		public async Task GetUserPoints_ValidCredentials_RetursPoints()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + "/UserPoints";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<int>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Should().Be(178);
		}

		[Fact]
		public async Task GetAvatar_ValidCredentials_RetursAvatar()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + "/1";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Avatar>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Id.Should().Be(1);
		}
	}
}
