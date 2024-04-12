using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Contracts.Purchase
{
	public class ShoppingPurchaseUpdate
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public decimal Quantity { get; set; }
		public QuantityType QuantityType { get; set; }
		public ProductType ProductType { get; set; }
		public int? HomeChoreTaskId { get; set; }
		public int? Time { get; set; }
	}
}
