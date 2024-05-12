using FluentAssertions;
using HomeChoreTracker.Api.Contracts.User;
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
	public class HomeChoreBaseControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "HomeChoreBase";

		public HomeChoreBaseControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetHomeChoresBase_ValidCredentials_RetursHomeChoresBaseResponse()
		{
			// Arrange
			int skip = 0;
			int take = 10;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/skip{skip}/take{take}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<HomeChoreBase>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(9);
		}

		[Fact]
		public async Task GetHomeChoresBases_ValidCredentials_RetursHomeChoresBasesResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri;
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<HomeChoreBase>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(9);
		}

		[Fact]
		public async Task GetHomeChoreBase_ValidCredentials_RetursHomeChoreBaseResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<HomeChoreBase>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Name.Should().Be("Vacuuming");
		}
	}
}
