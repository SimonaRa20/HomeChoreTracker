using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Purchase
{
    public class ShoppingItemRequest
    {
        public string Title { get; set; }
        public decimal Quantity { get; set; }
        public QuantityType QuantityType { get; set; }
        public ProductType ProductType { get; set; }
		public int? HomeChoreTaskId { get; set; }
		public int? Time { get; set; }
		public bool IsCompleted { get; set; }
    }
}
