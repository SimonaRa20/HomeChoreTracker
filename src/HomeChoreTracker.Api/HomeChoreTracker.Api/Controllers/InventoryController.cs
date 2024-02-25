using HomeChoreTracker.Api.Contracts.Inventory;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeChoreTracker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRepository _purchaseRepository;

        public InventoryController(IProductRepository productRepository, IPurchaseRepository purchaseRepository)
        {
            _productRepository = productRepository;
            _purchaseRepository = purchaseRepository;
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<List<Product>>> GetAllProducts(int id)
        {
            try
            {
                var products = await _productRepository.GetAllProducts(id);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("product")]
		[Authorize]
		public async Task<IActionResult> AddProduct(ProductRequest productRequest)
        {
            try
            {
                Product product = new Product
                {
                    Title = productRequest.Title,
                    ExpirationDate = productRequest.ExpirationDate,
                    Quantity = productRequest.Quantity,
                    QuantityType = productRequest.QuantityType,
                    ProductType = productRequest.ProductType,
                    HomeId = productRequest.HomeId,
                };
                await _productRepository.AddProduct(product);
                await _productRepository.Save();
                return Ok("Income added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("purchases")]
        public async Task<ActionResult<List<Purchase>>> GetAllPurchases()
        {
            try
            {
                var purchases = await _purchaseRepository.GetAllPurchases();
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("purchase")]
        public async Task<ActionResult> AddPurchase(Purchase purchase)
        {
            try
            {
                await _purchaseRepository.AddPurchase(purchase);
                await _purchaseRepository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
