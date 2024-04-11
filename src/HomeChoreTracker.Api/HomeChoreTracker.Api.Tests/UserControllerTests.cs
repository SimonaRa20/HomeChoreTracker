using AutoMapper;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests
{
	public class UserControllerTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly UserController _userController;

		public UserControllerTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_authRepositoryMock = new Mock<IAuthRepository>();
			_mapperMock = new Mock<IMapper>();
			_userController = new UserController(_userRepositoryMock.Object, _authRepositoryMock.Object, _mapperMock.Object);
		}

		[Fact]
		public async Task GetUserData_ExistingUser_ReturnsOkResult()
		{
			// Arrange
			var userId = 1;
			var user = new User { Id = userId };
			var userGetResponse = new UserGetResponse(); // Modify this line
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
			_mapperMock.Setup(mapper => mapper.Map<UserGetResponse>(user)).Returns(userGetResponse);
			var userClaims = new List<Claim> { new Claim(ClaimTypes.Name, userId.ToString()) };
			var identity = new ClaimsIdentity(userClaims, "Test");
			var userPrincipal = new ClaimsPrincipal(identity);
			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = userPrincipal }
			};

			// Act
			var result = await _userController.GetUserData();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal((int)System.Net.HttpStatusCode.OK, okResult.StatusCode); // Modify this line
		}

		[Fact]
		public async Task GetUserData_NonExistingUser_ReturnsNotFoundResult()
		{
			// Arrange
			var userId = 1;
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);
			var userClaims = new List<Claim> { new Claim(ClaimTypes.Name, userId.ToString()) };
			var identity = new ClaimsIdentity(userClaims, "Test");
			var userPrincipal = new ClaimsPrincipal(identity);
			_userController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = userPrincipal }
			};

			// Act
			var result = await _userController.GetUserData();

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
	}
}
