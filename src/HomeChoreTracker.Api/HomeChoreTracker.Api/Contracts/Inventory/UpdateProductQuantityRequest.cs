namespace HomeChoreTracker.Api.Contracts.Inventory
{
    public class UpdateProductQuantityRequest
    {
        public int ProductId { get; set; }
        public decimal NewQuantity { get; set; }
    }
}
