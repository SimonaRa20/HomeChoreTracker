using AutoMapper;
using HomeChoreTracker.Api.Contracts.Auth;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Controllers;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HomeChoreTracker.Api.Tests
{
	public class AuthControllerTests
	{
		private readonly AuthController _authController;
		private readonly Mock<IAuthRepository> _authRepositoryMock;
		private readonly Mock<IConfiguration> _config;
		private readonly Mock<IMapper> _mapperMock;

		public AuthControllerTests()
		{
			_mapperMock = new Mock<IMapper>();
			_config = new Mock<IConfiguration>();
			_authRepositoryMock = new Mock<IAuthRepository>();
			_authController = new AuthController(_config.Object, _authRepositoryMock.Object, _mapperMock.Object);
		}

		[Fact]
		public async Task Register_ValidUser_ReturnsCreatedResult()
		{
			// Arrange
			var userRequest = new UserRegisterRequest
			{
				UserName = "test",
				Email = "test@example.com",
				Password = "password"
			};
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
			_authRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<User>())).Returns(Task.CompletedTask);
			_authRepositoryMock.Setup(repo => repo.Save()).Returns(Task.CompletedTask);

			// Act
			var result = await _authController.Register(userRequest) as CreatedResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
		}

		[Fact]
		public async Task Register_ExistingUser_ReturnsUnprocessableEntityResult()
		{
			// Arrange
			var userRequest = new UserRegisterRequest
			{
				Email = "test@example.com",
				Password = "password"
			};
			var existingUser = new User { Email = "test@example.com" };
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(existingUser);

			// Act
			var result = await _authController.Register(userRequest) as ObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal((int)HttpStatusCode.UnprocessableEntity, result.StatusCode);
		}

		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkResult()
		{
			// Arrange
			var userRequest = new UserRegisterRequest
			{
				UserName = "test",
				Email = "test@example.com",
				Password = "password"
			};

			// Simulate user registration
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(userRequest.Email)).ReturnsAsync((User)null);
			_authRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<User>())).Returns(Task.CompletedTask);
			_authRepositoryMock.Setup(repo => repo.Save()).Returns(Task.CompletedTask);

			// Register the user
			var resultRegister = await _authController.Register(userRequest) as CreatedResult;

			// Now set up the mock repository to return the registered user
			var registeredUser = new User
			{
				Email = userRequest.Email,
				Password = "password" // Assuming this is the hashed password
			};
			var saltValue = "dsdjiajeefiajofijaoifjoaijfoiajgorjag";
			byte[] salt = Encoding.ASCII.GetBytes(saltValue);
			string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: registeredUser.Password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA512,
				iterationCount: 10000,
				numBytesRequested: 256 / 8));

			var userLoginRequest = new UserLoginRequest
			{
				Email = userRequest.Email,
				Password = hashedPassword,
			};
			_authController.ControllerContext = new ControllerContext
			{
				HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
			};

			// Act
			var result = await _authController.LoginAsync(userLoginRequest) as OkObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal((int)System.Net.HttpStatusCode.OK, result.StatusCode);
		}



		[Fact]
		public async Task Login_InValidCredentials_ReturnsNotFoundResult()
		{
			// Arrange
			var userLoginRequest = new UserLoginRequest
			{
				Email = "test@example.com",
				Password = "password"
			};
			var user = new User { Email = "test@example.com", Password = "hashed_password" };
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(userLoginRequest.Email)).ReturnsAsync(user);
			_authController.ControllerContext = new ControllerContext
			{
				HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
			};

			// Act
			var result = await _authController.LoginAsync(userLoginRequest) as NotFoundObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal((int)System.Net.HttpStatusCode.NotFound, result.StatusCode);
		}

		[Fact]
		public async Task RestorePassword_ExistingUser_ReturnsOkResult()
		{
			// Arrange
			var userEmail = "test@example.com";
			var user = new User { Email = userEmail };
			_authRepositoryMock.Setup(repo => repo.GetUserByEmail(userEmail)).ReturnsAsync(user);

			// Act
			var result = await _authController.RestorePassword(userEmail) as OkObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal((int)System.Net.HttpStatusCode.OK, result.StatusCode);
		}
	}
}
