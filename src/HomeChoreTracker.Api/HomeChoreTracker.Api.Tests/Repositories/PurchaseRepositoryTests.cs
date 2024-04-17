using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using HomeChoreTracker.Api.Tests.DatabaseFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.Repositories
{
	[Collection("Sequential")]
	public class PurchaseRepositoryTests
	{
		private readonly HomeChoreTrackerDbFixture _homeChoreTrackerDbFixture;

		public PurchaseRepositoryTests(HomeChoreTrackerDbFixture trakiDbFixture)
		{
			_homeChoreTrackerDbFixture = trakiDbFixture;
		}

		[Fact]
		public async Task AddPurchase_ShouldAddPurchaseToDatabase()
		{
			// Arrange
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new PurchaseRepository(context);
			var purchase = new Purchase
			{
				PurchaseDate = DateTime.Now,
				IsCompleted = false,
				HomeId = 1
			};

			// Act
			await repository.AddPurchase(purchase);
			await repository.Save();

			// Assert
			var addedPurchase = context.Purchases.FirstOrDefault(p => p.Id == purchase.Id);
			Assert.NotNull(addedPurchase);
		}

		[Fact]
		public async Task UpdateShoppingItem_ShouldUpdateShoppingItemInDatabase()
		{
			// Arrange
			var itemId = 1;
			var updatedTitle = "Updated Item Title";
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new PurchaseRepository(context);

			var purchase = new Purchase
			{
				PurchaseDate = DateTime.Now,
				IsCompleted = false,
				HomeId = 1,
				Items = new List<ShoppingItem>()
			};

			// Act
			await repository.AddPurchase(purchase);
			await repository.Save();

			var shopping = new ShoppingItem 
			{
				Title = "Test Item",
				Quantity = 1,
				QuantityType = QuantityType.Pieces,
				ProductType = ProductType.Appliances,
				IsCompleted = false,
				PurchaseId = purchase.Id,
				WasBought = DateTime.Now,
				HomeId = 1
			};

			purchase.Items.Add(shopping);
			await repository.UpdatePurchase(purchase);
			await repository.Save();

			var shoppingItem = await repository.GetShoppingItemById(shopping.Id);
			shoppingItem.Title = updatedTitle;

			// Act
			await repository.UpdateShoppingItem(shoppingItem);

			// Assert
			var updatedItem = await repository.GetShoppingItemById(itemId);
			Assert.Equal(updatedTitle, updatedItem.Title);
		}

		[Fact]
		public async Task UpdatePurchase_ShouldUpdatePurchaseInDatabase()
		{
			// Arrange
			var purchaseId = 1; // Assuming purchase id is 1
			var newPurchaseDate = DateTime.Now.AddDays(-1); // New purchase date
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new PurchaseRepository(context);
			var purchase = await repository.GetPurchaseById(purchaseId);
			purchase.PurchaseDate = newPurchaseDate;

			// Act
			await repository.UpdatePurchase(purchase);

			// Assert
			var updatedPurchase = await repository.GetPurchaseById(purchaseId);
			Assert.Equal(newPurchaseDate, updatedPurchase.PurchaseDate);
		}

		[Fact]
		public async Task DeletePurchase_ShouldDeletePurchaseFromDatabase()
		{
			// Arrange
			var purchaseId = 1;
			using var context = new HomeChoreTrackerDbContext(_homeChoreTrackerDbFixture.Options);
			var repository = new PurchaseRepository(context);
			var purchase = await repository.GetPurchaseById(purchaseId);

			// Act
			await repository.DeletePurchase(purchase);
			await repository.Save();

			// Assert
			var deletedPurchase = await repository.GetPurchaseById(purchaseId);
			Assert.Null(deletedPurchase);
		}

	}
}
