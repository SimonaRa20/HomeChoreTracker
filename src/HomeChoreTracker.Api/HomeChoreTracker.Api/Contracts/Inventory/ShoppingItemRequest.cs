namespace HomeChoreTracker.Api.Contracts.Inventory
{
    public class ShoppingItemRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
