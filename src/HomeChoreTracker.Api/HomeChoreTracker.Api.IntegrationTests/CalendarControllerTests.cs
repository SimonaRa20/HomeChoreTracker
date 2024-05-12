using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Home;
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
	public class CalendarControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Calendar";

		public CalendarControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetEvents_ValidCredentials_RetursEventsResponse()
		{
			// Arrange
			string uri = baseUri;
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<Event>>(uri);

			// Assert
			response.Data.Count.Should().Be(246);
		}

		[Fact]
		public async Task GetHomeChoresToCalendar_ValidCredentials_RetursHomeChoresToCalendarResponse()
		{
			// Arrange
			string uri = baseUri + "/Chores";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<TaskAssignmentResponse>>(uri);

			// Assert
			response.Data.Count.Should().Be(5);
		}

		[Fact]
		public async Task GetHomeChoreFromCalendar_ValidCredentials_RetursHomeChoreFromCalendarResponse()
		{
			// Arrange
			int id = 294;
			string uri = baseUri + $"/Chore/{id}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<HomeChoreEventResponse>(uri);

			// Assert
			response.Data.Name.Should().Be("Vacuuming");
		}


		[Fact]
		public async Task HomeChoreCalendarFile_ValidCredentials_RetursHomeChoreCalendarFileResponse()
		{
			// Arrange
			var client = _factory.GetCustomHttpClient();
			string uri = baseUri + $"/Chores/File";
			await client.LoginAsUser();

			// Act
			var response = await client.Get<byte[]>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}
