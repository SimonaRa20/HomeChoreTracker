namespace HomeChoreTracker.Portal.Models.Purchase
{
	public class UpdatePurchaseRequest
	{
		public decimal PriceForProducts { get; set; }
		public List<ShoppingItemUpdateRequest> Items { get; set; }
	}
}
