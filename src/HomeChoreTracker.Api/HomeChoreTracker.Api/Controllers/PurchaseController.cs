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
    }
}
