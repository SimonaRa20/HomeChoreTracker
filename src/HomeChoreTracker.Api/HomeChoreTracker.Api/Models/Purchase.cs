namespace HomeChoreTracker.Api.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public List<ShoppingItem> Items { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool IsCompleted { get; set; }

        public int HomeId { get; set; }
        public Home? Home { get; set; }
    }
}
