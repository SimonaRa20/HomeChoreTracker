using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Purchase
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
