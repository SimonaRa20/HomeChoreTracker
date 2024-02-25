using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Inventory
{
    public class ProductRequest
    {
        public string Title { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Quantity { get; set; }
        public QuantityType QuantityType { get; set; }
        public ProductType ProductType { get; set; }
        public int HomeId { get; set; }
    }
}
