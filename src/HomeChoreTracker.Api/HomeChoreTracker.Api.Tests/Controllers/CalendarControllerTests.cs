using AutoFixture;
using AutoMapper;
using HomeChoreTracker.Api.Contracts.Calendar;
using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class CalendarControllerTests
	{
		private readonly Fixture _fixture;
		private readonly CalendarController _calendarController;
		private readonly Mock<ICalendarRepository> _calendarRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IHomeChoreRepository> _homeChoreRepositoryMock;
		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;

		public CalendarControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_authRepositoryMock = new Mock<IAuthRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_calendarRepositoryMock = new Mock<ICalendarRepository>();
			_homeChoreRepositoryMock = new Mock<IHomeChoreRepository>();
			_purchaseRepositoryMock = new Mock<IPurchaseRepository>();
			_mapperMock = new Mock<IMapper>();
			_calendarController = new CalendarController(_calendarRepositoryMock.Object, _homeChoreRepositoryMock.Object, _userRepositoryMock.Object,_purchaseRepositoryMock.Object);
		}

		[Fact]
		public async Task AddBusyInterval_Returns_BadRequestObjectResult_When_Null_Added()
		{
			// Arrange
			var userId = 1;
			var busyIntervalRequest = _fixture.Create<BusyIntervalRequest>();
			var user = _fixture.Create<User>();
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

			_calendarController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			// Act
			var result = await _calendarController.AssignTasksToMembers(1,true, true,true);

			// Assert
			var okResult = Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}
