namespace HomeChoreTracker.Api.Contracts.Purchase
{
	public class UpdatePurchaseRequest
	{
		public decimal PriceForProducts { get; set; }
		public List<ShoppingItemUpdateRequest> Items { get; set; }
	}
}
