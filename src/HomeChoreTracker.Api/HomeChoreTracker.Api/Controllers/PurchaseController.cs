using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Contracts.Purchase;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using System.Linq;
using System.Security.Claims;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IIncomeRepository _incomeRepository;

        public PurchaseController(IPurchaseRepository purchaseRepository, IUserRepository userRepository, IGamificationRepository gamificationRepository, INotificationRepository notificationRepository, 
                                    IExpenseRepository expenseRepository, IIncomeRepository incomeRepository)
        {
            _purchaseRepository = purchaseRepository;
            _userRepository = userRepository;
            _gamificationRepository = gamificationRepository;
            _notificationRepository = notificationRepository;
            _expenseRepository = expenseRepository;
            _incomeRepository = incomeRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPurchase(PurchaseRequest purchaseRequest)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                var user = await _userRepository.GetUserById(userId);

                var purchase = new Purchase
                {
                    HomeId = purchaseRequest.HomeId,
                    PurchaseDate = purchaseRequest.PurchaseDate,
                    IsCompleted = false,
                    Items = new List<ShoppingItem>()
                };

                foreach (var item in purchaseRequest.Items)
                {
                    purchase.Items.Add(new ShoppingItem
                    {
                        Title = item.Title,
                        Quantity = item.Quantity,
                        QuantityType = item.QuantityType,
                        ProductType = item.ProductType,
                        IsCompleted = false,
                        PurchaseId = purchase.Id,
                        HomeId = purchase.HomeId,
                        HomeChoreTaskId = item.HomeChoreTaskId,
                        Time = item.Time
                    });
                }

                await _purchaseRepository.AddPurchase(purchase);
                await _purchaseRepository.Save();

                var hasBadge = await _gamificationRepository.UserHasCreateFirstPurchaseBadge(userId);
                if (!hasBadge)
                {
                    BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(userId);
                    wallet.CreateFirstPurchase = true;
                    await _gamificationRepository.UpdateBadgeWallet(wallet);

                    Notification notification = new Notification
                    {
                        Title = $"You earned badge 'Create first purchase'",
                        IsRead = false,
                        Time = DateTime.Now,
                        UserId = (int)userId,
                        User = user,
                    };

                    await _notificationRepository.CreateNotification(notification);
                }

                return Ok("Purchase added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{homeId}")]
        [Authorize]
        public async Task<IActionResult> GetPurchases(int homeId)
        {
            List<Purchase> purchases = await _purchaseRepository.GetAllPurchases(homeId);
            purchases = purchases.OrderByDescending(item => item.PurchaseDate).ToList();

            return Ok(purchases);
        }

        [HttpGet("purchase/{purchaseId}")]
        [Authorize]
        public async Task<IActionResult> GetPurchase(int purchaseId)
        {
            Purchase purchase = await _purchaseRepository.GetPurchaseById(purchaseId);

            return Ok(purchase);
        }

        [HttpPost("UpdateShoppingItems")]
        [Authorize]
        public async Task<IActionResult> UpdateShoppingItems(UpdatePurchaseRequest itemsToUpdate)
        {
            try
            {
				int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

				foreach (var item in itemsToUpdate.Items)
                {
                    var shoppingItem = await _purchaseRepository.GetShoppingItemById(item.Id);
                    if (shoppingItem == null)
                    {
                        return NotFound($"Shopping item with ID {item.Id} not found.");
                    }

                    shoppingItem.IsCompleted = item.IsCompleted;
                    shoppingItem.WasBought = DateTime.Now;
                    await _purchaseRepository.UpdateShoppingItem(shoppingItem);
                }

                var firstShoppingItem = await _purchaseRepository.GetShoppingItemById(itemsToUpdate.Items[0].Id);
                var purchaseId = firstShoppingItem?.PurchaseId;
                if (purchaseId.HasValue)
                {
                    var purchase = await _purchaseRepository.GetPurchaseById(purchaseId.Value);
                    if (purchase != null && purchase.Items.All(item => item.IsCompleted))
                    {
                        purchase.IsCompleted = true;
                    }
                    else
                    {
                        purchase.IsCompleted = false;
                    }
                    purchase.PriceForProducts = itemsToUpdate.PriceForProducts;
					await _purchaseRepository.UpdatePurchase(purchase);

					bool wasSetAmount = await _purchaseRepository.CheckOrWasSetAmount(purchase);

					FinancialCategory category = await _expenseRepository.CheckCategory("Home chores");
					FinancialCategory addedCategory = new FinancialCategory();
					if (category == null)
					{
						FinancialCategory newfinancialCategory = new FinancialCategory
						{
							Name = "Home chores",
							Type = FinancialType.Expense,
							UserId = userId,
							HomeId = purchase.HomeId
						};

						addedCategory = await _incomeRepository.AddCategory(newfinancialCategory);
					}

					if (wasSetAmount)
                    {
                        var expenseRequest = await _purchaseRepository.GetRecord(purchase.Id);
                        expenseRequest.Amount = purchase.PriceForProducts;
						await _expenseRepository.Update(expenseRequest);
					}
                    else
                    {
						FinancialRecord expense = new FinancialRecord
						{
							Title = $"Purchase {purchase.PurchaseDate}",
							Amount = purchase.PriceForProducts,
							Time = DateTime.Now,
							Type = FinancialType.Expense,
							FinancialCategoryId = category.Id,
							HomeId = purchase.HomeId,
							UserId = userId,
							PurchaseId = purchaseId
						};
						await _expenseRepository.AddExpense(expense);
					}
				}

                return Ok("Shopping items updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

		[HttpPost("UpdatePurchase/{purchaseId}")]
		[Authorize]
		public async Task<IActionResult> UpdatePurchase(int purchaseId, List<ShoppingPurchaseUpdate> itemsToUpdate)
		{
			try
			{
				var purchase = await _purchaseRepository.GetPurchaseById(purchaseId);
				if (purchase == null)
				{
					return NotFound($"Purchase with ID {purchaseId} not found.");
				}

				foreach (var update in itemsToUpdate)
				{
					if (!int.TryParse(update.Id, out int itemId))
					{
						return BadRequest($"Invalid item ID: {update.Id}");
					}

					if (update.Id == "0")
					{
						purchase.Items.Add(new ShoppingItem
						{
							Title = update.Title,
							Quantity = update.Quantity,
							QuantityType = update.QuantityType,
							ProductType = update.ProductType,
							IsCompleted = false,
                            HomeChoreTaskId = update.HomeChoreTaskId,
                            Time = update.Time,
						});
					}
					else
					{
						var existingItem = purchase.Items.FirstOrDefault(item => item.Id == itemId);
						if (existingItem != null)
						{
							existingItem.Title = update.Title;
							existingItem.Quantity = update.Quantity;
							existingItem.QuantityType = update.QuantityType;
							existingItem.ProductType = update.ProductType;
                            existingItem.HomeChoreTaskId = update.HomeChoreTaskId;
                            existingItem.Time = update.Time;
						}
						else
						{
							return NotFound($"Item with ID {itemId} not found in the purchase.");
						}
					}
				}

				var itemIdsToUpdate = itemsToUpdate.Select(item => int.Parse(item.Id));
				var itemsToRemove = purchase.Items.Where(item => !itemIdsToUpdate.Contains(item.Id)).ToList();
				foreach (var itemToRemove in itemsToRemove)
				{
					purchase.Items.Remove(itemToRemove);
				}

				_purchaseRepository.UpdatePurchase(purchase);
				await _purchaseRepository.Save();

				return Ok("Purchase updated successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
		[HttpDelete("{purchaseId}")]
		[Authorize]
		public async Task<IActionResult> DeletePurchase(int purchaseId)
		{
			try
			{
				var purchase = await _purchaseRepository.GetPurchaseById(purchaseId);
				if (purchase == null)
				{
					return NotFound($"Purchase with ID {purchaseId} not found.");
				}

				_purchaseRepository.DeletePurchase(purchase);
				await _purchaseRepository.Save();

				return Ok("Purchase deleted successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
