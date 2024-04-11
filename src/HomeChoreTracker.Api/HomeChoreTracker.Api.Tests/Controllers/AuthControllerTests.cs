using AutoFixture;
using AutoMapper;
using HomeChoreTracker.Api.Constants;
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

namespace HomeChoreTracker.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _fixture = new Fixture();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _mapperMock = new Mock<IMapper>();
            _authController = new AuthController(null, _authRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Register_Returns_CreatedResult_When_ValidInput()
        {
            // Arrange
            var userRegisterRequest = _fixture.Create<UserRegisterRequest>();
            userRegisterRequest.Email = "test@gmail.com";
            userRegisterRequest.Password = "short123456";

            // Act
            var result = await _authController.Register(userRegisterRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, createdResult.StatusCode);

            var responseData = Assert.IsAssignableFrom<UserRegisterResponse>(createdResult.Value);
            Assert.Equal(responseData.Id, responseData.Id);
            Assert.Equal(responseData.UserName, responseData.UserName);
            Assert.Equal(responseData.Email, responseData.Email);
        }


        [Fact]
        public async Task Register_Returns_UnprocessableEntityResult_When_InvalidEmail()
        {
            // Arrange
            var userRegisterRequest = _fixture.Create<UserRegisterRequest>();
            userRegisterRequest.Email = "invalidemail";

            // Act
            var result = await _authController.Register(userRegisterRequest);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
            Assert.Equal("Invalid email format.", objectResult.Value);
        }

        [Fact]
        public async Task Register_Returns_UnprocessableEntityResult_When_PasswordIsTooShort()
        {
            // Arrange
            var userRegisterRequest = _fixture.Create<UserRegisterRequest>();
            userRegisterRequest.Email = "test@gmail.com";
            userRegisterRequest.Password = "short";

            // Act
            var result = await _authController.Register(userRegisterRequest);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
            Assert.Equal("Password should be a minimum of 8 characters.", objectResult.Value);
        }

        [Fact]
        public async Task Register_Returns_UnprocessableEntityResult_When_UserAlreadyExists()
        {
            // Arrange
            var userRegisterRequest = _fixture.Create<UserRegisterRequest>();
            userRegisterRequest.Email = "text@gmail.com";
            var existingUser = new User
            {
                Id = _fixture.Create<int>(),
                UserName = _fixture.Create<string>(),
                Email = userRegisterRequest.Email,
                Password = _fixture.Create<string>(),
                Role = Role.User,
                StartDayTime = new TimeSpan(8, 0, 0),
                EndDayTime = new TimeSpan(22, 0, 0),
                BadgeWallet = new BadgeWallet
                {
                    DoneFirstTask = false,
                    DoneFirstCleaningTask = false,
                    DoneFirstLaundryTask = false,
                    DoneFirstKitchenTask = false,
                    DoneFirstBathroomTask = false,
                    DoneFirstBedroomTask = false,
                    DoneFirstOutdoorsTask = false,
                    DoneFirstOrganizeTask = false,
                    EarnedPerDayFiftyPoints = false,
                    EarnedPerDayHundredPoints = false,
                    DoneFiveTaskPerWeek = false,
                    DoneTenTaskPerWeek = false,
                    DoneTwentyFiveTaskPerWeek = false,
                    CreatedTaskWasUsedOtherHome = false,
                    CreateFirstPurchase = false,
                    CreateFirstAdvice = false,
                    CreateFirstIncome = false,
                    CreateFirstExpense = false
                }
            };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(existingUser);

            // Act
            var result = await _authController.Register(userRegisterRequest);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.UnprocessableEntity, objectResult.StatusCode);
            Assert.Equal("User with the same email already exists.", objectResult.Value);
        }

        [Fact]
        public async Task RestoreForgotPassword_Returns_BadRequest_When_Invalid_Token()
        {
            // Arrange
            var restoreData = _fixture.Create<UserRestorePasswordRequest>();
            restoreData.Token = "invalid_token";

            // Act
            var result = await _authController.RestoreForgotPassword(restoreData);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid token.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_Returns_CreatedResult_When_ValidCredentials()
        {
            // Arrange
            var userLoginRequest = _fixture.Create<UserLoginRequest>();
            userLoginRequest.Email = "test@gmail.com";

            var existingUser = new User
            {
                Id = _fixture.Create<int>(),
                UserName = _fixture.Create<string>(),
                Email = userLoginRequest.Email,
                Password = HashPassword("testtest"),
                Role = Role.User,
                StartDayTime = new TimeSpan(8, 0, 0),
                EndDayTime = new TimeSpan(22, 0, 0),
                BadgeWallet = new BadgeWallet
                {
                    DoneFirstTask = false,
                    DoneFirstCleaningTask = false,
                    DoneFirstLaundryTask = false,
                    DoneFirstKitchenTask = false,
                    DoneFirstBathroomTask = false,
                    DoneFirstBedroomTask = false,
                    DoneFirstOutdoorsTask = false,
                    DoneFirstOrganizeTask = false,
                    EarnedPerDayFiftyPoints = false,
                    EarnedPerDayHundredPoints = false,
                    DoneFiveTaskPerWeek = false,
                    DoneTenTaskPerWeek = false,
                    DoneTwentyFiveTaskPerWeek = false,
                    CreatedTaskWasUsedOtherHome = false,
                    CreateFirstPurchase = false,
                    CreateFirstAdvice = false,
                    CreateFirstIncome = false,
                    CreateFirstExpense = false
                }
            };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(existingUser.Email)).ReturnsAsync(existingUser);
            userLoginRequest.Password = "testtest";

            // Act
            var result = await _authController.LoginAsync(userLoginRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        private string HashPassword(string? password)
        {
            var saltValue = "dsdjiajeefiajofijaoifjoaijfoiajgorjag";
            byte[] salt = Encoding.ASCII.GetBytes(saltValue);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashedPassword;
        }

        [Fact]
        public async Task Login_Returns_NotFound_When_InvalidCredentials()
        {
            // Arrange
            var userLoginRequest = _fixture.Create<UserLoginRequest>();
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(userLoginRequest.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _authController.LoginAsync(userLoginRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Invalid email or password. Please try again.", notFoundResult.Value);
        }
    }
}
