namespace HomeChoreTracker.Api.Contracts.Inventory
{
    public class PurchaseRequest
    {
        public int HomeId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<ShoppingItemRequest> Items { get; set; }
    }
}
