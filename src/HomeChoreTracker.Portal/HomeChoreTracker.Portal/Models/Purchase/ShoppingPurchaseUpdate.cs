using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Purchase
{
	public class ShoppingPurchaseUpdate
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public decimal Quantity { get; set; }
		public QuantityType QuantityType { get; set; }
		public ProductType ProductType { get; set; }
	}
}
