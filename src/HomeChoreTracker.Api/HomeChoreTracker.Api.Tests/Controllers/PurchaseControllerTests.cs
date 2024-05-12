using AutoFixture;
using HomeChoreTracker.Api.Contracts.Purchase;
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
	public class PurchaseControllerTests
	{
		private readonly Fixture _fixture;
		private readonly PurchaseController _purchaseController;
		private readonly Mock<IPurchaseRepository> _purchaseRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IGamificationRepository> _gamificationRepositoryMock;
		private readonly Mock<INotificationRepository> _notificationRepositoryMock;
		private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
		private readonly Mock<IIncomeRepository> _incomeRepositoryMock;

		public PurchaseControllerTests()
		{
			_fixture = new Fixture();
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			_userRepositoryMock = new Mock<IUserRepository>();
			_purchaseRepositoryMock = new Mock<IPurchaseRepository>();
			_gamificationRepositoryMock = new Mock<IGamificationRepository>();
			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_expenseRepositoryMock = new Mock<IExpenseRepository>();
			_incomeRepositoryMock = new Mock<IIncomeRepository>();
			_purchaseController = new PurchaseController(_purchaseRepositoryMock.Object, _userRepositoryMock.Object, _gamificationRepositoryMock.Object, _notificationRepositoryMock.Object, _expenseRepositoryMock.Object, _incomeRepositoryMock.Object);
		}

		[Fact]
		public async Task AddPurchase_Returns_OkResult()
		{
			// Arrange
			var userId = 1;
			_purchaseController.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name, userId.ToString())
					}, "mock"))
				}
			};

			var purchaseRequest = new PurchaseRequest
			{
				HomeId = 1,
				PurchaseDate = DateTime.Now,
				Items = new System.Collections.Generic.List<ShoppingItemRequest>
				{
					new ShoppingItemRequest
					{
						Title = "Item 1",
						Quantity = 1,
						QuantityType = Constants.QuantityType.Pieces,
						ProductType = Constants.ProductType.Cookware,
						IsCompleted = false,
						HomeChoreTaskId = 1,
					}
				}
			};
			_userRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(new User());
			_gamificationRepositoryMock.Setup(repo => repo.UserHasCreateFirstPurchaseBadge(userId)).ReturnsAsync(false);
			_purchaseRepositoryMock.Setup(repo => repo.AddPurchase(It.IsAny<Purchase>())).Returns(Task.CompletedTask);
			_gamificationRepositoryMock.Setup(repo => repo.GetUserBadgeWallet(userId)).ReturnsAsync(new BadgeWallet());
			_notificationRepositoryMock.Setup(repo => repo.CreateNotification(It.IsAny<Notification>())).Returns(Task.CompletedTask);

			// Act
			var result = await _purchaseController.AddPurchase(purchaseRequest);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Purchase added successfully", okResult.Value);
		}

		[Fact]
		public async Task UpdatePurchase_Returns_OkResult()
		{
			// Arrange
			var purchaseId = 1;
			var itemsToUpdate = new List<ShoppingPurchaseUpdate>
	{
		new ShoppingPurchaseUpdate
		{
			Id = "1",
            Title = "Updated Item 1",
			Quantity = 1,
			QuantityType = Constants.QuantityType.Pieces,
			ProductType = Constants.ProductType.Cookware,
			HomeChoreTaskId = 1,
		}
	};
			var purchaseToUpdate = new Purchase { Id = purchaseId, Items = new List<ShoppingItem>() };
			purchaseToUpdate.Items.Add(new ShoppingItem { Id = 1 });
			_purchaseRepositoryMock.Setup(repo => repo.GetPurchaseById(purchaseId)).ReturnsAsync(purchaseToUpdate);
			_purchaseRepositoryMock.Setup(repo => repo.UpdatePurchase(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

			// Act
			var result = await _purchaseController.UpdatePurchase(purchaseId, itemsToUpdate);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Purchase updated successfully.", okResult.Value);
		}


		[Fact]
		public async Task DeletePurchase_Returns_OkResult()
		{
			// Arrange
			var purchaseId = 1;
			var purchaseToDelete = new Purchase { Id = purchaseId };
			_purchaseRepositoryMock.Setup(repo => repo.GetPurchaseById(purchaseId)).ReturnsAsync(purchaseToDelete);
			_purchaseRepositoryMock.Setup(repo => repo.DeletePurchase(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

			// Act
			var result = await _purchaseController.DeletePurchase(purchaseId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Purchase deleted successfully.", okResult.Value);
		}

		[Fact]
		public async Task UpdatePurchase_InvalidItemId_Returns_BadRequest()
		{
			// Arrange
			var purchaseId = 1;
			var itemsToUpdate = new List<ShoppingPurchaseUpdate>
	{
		new ShoppingPurchaseUpdate
		{
			Id = "invalidId",
            Title = "Updated Item 1",
			Quantity = 1,
			QuantityType = Constants.QuantityType.Pieces,
			ProductType = Constants.ProductType.Cookware,
			HomeChoreTaskId = 1,
		}
	};
			var purchaseToUpdate = new Purchase { Id = purchaseId, Items = new List<ShoppingItem>() };
			_purchaseRepositoryMock.Setup(repo => repo.GetPurchaseById(purchaseId)).ReturnsAsync(purchaseToUpdate);

			// Act
			var result = await _purchaseController.UpdatePurchase(purchaseId, itemsToUpdate);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid item ID: invalidId", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdatePurchase_NewItem_Returns_OkResult()
		{
			// Arrange
			var purchaseId = 1;
			var itemsToUpdate = new List<ShoppingPurchaseUpdate>
	{
		new ShoppingPurchaseUpdate
		{
			Id = "0",
            Title = "New Item",
			Quantity = 1,
			QuantityType = Constants.QuantityType.Pieces,
			ProductType = Constants.ProductType.Cookware,
			HomeChoreTaskId = 1,
		}
	};
			var purchaseToUpdate = new Purchase { Id = purchaseId, Items = new List<ShoppingItem>() };
			_purchaseRepositoryMock.Setup(repo => repo.GetPurchaseById(purchaseId)).ReturnsAsync(purchaseToUpdate);
			_purchaseRepositoryMock.Setup(repo => repo.UpdatePurchase(It.IsAny<Purchase>())).Returns(Task.CompletedTask);

			// Act
			var result = await _purchaseController.UpdatePurchase(purchaseId, itemsToUpdate);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Purchase updated successfully.", okResult.Value);
		}

		[Fact]
		public async Task UpdateShoppingItems_ShouldReturnNotFound_WhenShoppingItemNotFound()
		{
			// Arrange
			var itemsToUpdate = new List<ShoppingItemUpdateRequest>
			{
				new ShoppingItemUpdateRequest { Id = 1, IsCompleted = true }
			};
			_purchaseRepositoryMock.Setup(repo => repo.GetShoppingItemById(It.IsAny<int>())).ReturnsAsync((ShoppingItem)null);
			var purchaseProduct = new UpdatePurchaseRequest { PriceForProducts = 0, Items = new List<ShoppingItemUpdateRequest> { new ShoppingItemUpdateRequest { Id = 1, IsCompleted = true } } };
			// Act
			var result = await _purchaseController.UpdateShoppingItems(purchaseProduct);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Shopping item with ID 1 not found.", notFoundResult.Value);
		}

		[Fact]
		public async Task UpdateShoppingItems_ShouldReturnOkResult_WhenAllItemsCompletedAndPurchaseExists()
		{
			// Arrange
			var itemsToUpdate = new List<ShoppingItemUpdateRequest>
			{
				new ShoppingItemUpdateRequest { Id = 1, IsCompleted = true }
			};
			var purchaseId = 1;
			var shoppingItem = new ShoppingItem { Id = 1, IsCompleted = true, PurchaseId = purchaseId };
			var purchase = new Purchase { Id = purchaseId, Items = new List<ShoppingItem> { shoppingItem } };
			var purchaseProduct = new UpdatePurchaseRequest { PriceForProducts = 0, Items = new List<ShoppingItemUpdateRequest> { new ShoppingItemUpdateRequest { Id = 1, IsCompleted = true } } };
			_purchaseRepositoryMock.Setup(repo => repo.GetShoppingItemById(It.IsAny<int>())).ReturnsAsync(shoppingItem);
			_purchaseRepositoryMock.Setup(repo => repo.GetPurchaseById(purchaseId)).ReturnsAsync(purchase);

			// Act
			var result = await _purchaseController.UpdateShoppingItems(purchaseProduct);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Shopping items updated successfully.", okResult.Value);
		}
	}
}
