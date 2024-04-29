using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Challenge;
using HomeChoreTracker.Api.Contracts.Forum;
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

namespace HomeChoreTracker.Api.Tests.Controllers
{
	public class ChallengeControllerTests
	{
		private readonly ChallengeController _challengeController;
		private readonly Mock<IChallengeRepository> _challengeRepositoryMock;

		public ChallengeControllerTests()
		{
			_challengeRepositoryMock = new Mock<IChallengeRepository>();
			_challengeController = new ChallengeController(_challengeRepositoryMock.Object);
		}

		[Fact]
		public async Task AddChallenge_Returns_OkResult()
		{
			// Arrange
			var challengeRequest = new ChallengeRequest
			{
				OpponentType = OpponentType.Home,
				HomeId = 1,
				OpponentHomeId = 1,
				ChallengeType = ChallengeType.EarnPoints,
				ChallengeCount = 10,
				Days = 1,
				Hours = 0,
				Minutes = 0,
				Seconds = 0
			};
			int userId = 1;
			
			_challengeRepositoryMock.Setup(repo => repo.AddChallenge(It.IsAny<Challenge>())).Returns(Task.CompletedTask);
			var userClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "1")
			};
			var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");
			var userPrincipal = new ClaimsPrincipal(userIdentity);
			_challengeController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = userPrincipal }
			};
			// Act
			var result = await _challengeController.AddChallenge(challengeRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Challenge added successfully", okResult.Value);
		}
	}
}
