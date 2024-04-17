using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Gamification;
using HomeChoreTracker.Api.Contracts.HomeChore;
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
	public class HomeChoreControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "HomeChore";

		public HomeChoreControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetHomeChoreTasks_ValidCredentials_RetursHomeChoreTasksResponse()
		{
			// Arrange
			int id = 1;
			string uri = baseUri + $"/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<HomeChoreTask>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(2);
		}

		[Fact]
		public async Task GetHomeChore_ValidCredentials_RetursHomeChoreResponse()
		{
			// Arrange
			int id = 1;
			string uri = baseUri + $"/Chore/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<HomeChoreTask>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Id.Should().Be(1);
			response.Data.Name.Should().Be("Vacuuming");
		}

		[Fact]
		public async Task GetHomeChoresToCalendar_ValidCredentials_RetursHomeChoresToCalendarResponse()
		{
			// Arrange
			int id = 1;
			string uri = baseUri + $"/Chore/Calendar/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<TaskAssignmentResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(101);
		}
	}
}
