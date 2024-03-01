using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Contracts.Purchase;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
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
                    // Retrieve the shopping item from the database
                    var shoppingItem = await _purchaseRepository.GetShoppingItemById(item.Id);
                    if (shoppingItem == null)
                    {
                        return NotFound($"Shopping item with ID {item.Id} not found.");
                    }

                    // Update the IsCompleted property
                    shoppingItem.IsCompleted = item.IsCompleted;

                    // Update the shopping item in the database
                    _purchaseRepository.UpdateShoppingItem(shoppingItem);
                }

                // Save changes to the database
                await _purchaseRepository.Save();

                // Check if all shopping items of the purchase are completed
                var firstShoppingItem = await _purchaseRepository.GetShoppingItemById(itemsToUpdate[0].Id);
                var purchaseId = firstShoppingItem?.PurchaseId;
                if (purchaseId.HasValue)
                {
                    var purchase = await _purchaseRepository.GetPurchaseById(purchaseId.Value);
                    if (purchase != null && purchase.Items.All(item => item.IsCompleted))
                    {
                        // Update the IsCompleted property of the purchase
                        purchase.IsCompleted = true;
                        _purchaseRepository.UpdatePurchase(purchase);
                    }
                    else
                    {
                        purchase.IsCompleted = false;
                        _purchaseRepository.UpdatePurchase(purchase);
                    }    
                }

                // Save changes to the database
                await _purchaseRepository.Save();

                return Ok("Shopping items updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
