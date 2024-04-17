using FluentAssertions;
using HomeChoreTracker.Api.Contracts.Notification;
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
	public class NotificationControllerTests
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly string baseUri = "Notification";

		public NotificationControllerTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetNotifications_ValidCredentials_ReturnsNotificationsResponse()
		{
			// Arrange
			string uri = baseUri;
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<NotificationResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(9);
		}

		[Fact]
		public async Task GetAllNotifications_ValidCredentials_ReturnsAllNotificationsResponse()
		{
			// Arrange
			string uri = baseUri + "/all";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<NotificationResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(22);
		}

		[Fact]
		public async Task GetAllNotification_ValidCredentials_ReturnsAllNotificationResponse()
		{
			// Arrange
			int skip = 0;
			int take = 10;
			string uri = baseUri + $"/all/skip{skip}/take{take}";
			var client = _factory.GetCustomHttpClient();
			await client.LoginAsUser();

			// Act
			var response = await client.Get<List<NotificationResponse>>(uri);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Data.Count.Should().Be(10);
		}
	}
}
