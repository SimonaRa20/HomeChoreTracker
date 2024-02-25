namespace HomeChoreTracker.Api.Models
{
    public class ShoppingItem
    {
        public int Id { get; set; }
        public int ProductId {  get; set; }
        public decimal Quantity { get; set; }
        public bool IsCompleted { get; set; }
    }
}
