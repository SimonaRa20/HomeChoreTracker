using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Forum;
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
	public class ForumControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Forum";

		public ForumControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetAdvice_ValidCredentials_RetursAdviceResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/{id}";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<Advice>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Title.Should().Be("Efficient Laundry Sorting");
		}

		[Fact]
		public async Task GetAdvices_ValidCredentials_RetursAdvicesResponse()
		{
			// Arrange
			int id = 1;
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri;
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<AdviceResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(7);
		}
	}
}
