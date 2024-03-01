namespace HomeChoreTracker.Api.Contracts.Purchase
{
    public class ShoppingItemUpdateRequest
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
    }
}
