using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal Quantity { get; set; }
        public QuantityType QuantityType { get; set; }
        public ProductType ProductType { get; set; }

        public int HomeId { get; set; }
    }
}
