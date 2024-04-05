using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Contracts.Purchase;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseController(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPurchase(PurchaseRequest purchaseRequest)
        {
            try
            {
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
                    });
                }

                await _purchaseRepository.AddPurchase(purchase);
                await _purchaseRepository.Save();

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
        public async Task<IActionResult> UpdateShoppingItems(List<ShoppingItemUpdateRequest> itemsToUpdate)
        {
            try
            {
                foreach (var item in itemsToUpdate)
                {
                    var shoppingItem = await _purchaseRepository.GetShoppingItemById(item.Id);
                    if (shoppingItem == null)
                    {
                        return NotFound($"Shopping item with ID {item.Id} not found.");
                    }

                    shoppingItem.IsCompleted = item.IsCompleted;
                    _purchaseRepository.UpdateShoppingItem(shoppingItem);
                }

                await _purchaseRepository.Save();

                var firstShoppingItem = await _purchaseRepository.GetShoppingItemById(itemsToUpdate[0].Id);
                var purchaseId = firstShoppingItem?.PurchaseId;
                if (purchaseId.HasValue)
                {
                    var purchase = await _purchaseRepository.GetPurchaseById(purchaseId.Value);
                    if (purchase != null && purchase.Items.All(item => item.IsCompleted))
                    {
                        purchase.IsCompleted = true;
                        _purchaseRepository.UpdatePurchase(purchase);
                    }
                    else
                    {
                        purchase.IsCompleted = false;
                        _purchaseRepository.UpdatePurchase(purchase);
                    }    
                }

                await _purchaseRepository.Save();

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
							IsCompleted = false
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
