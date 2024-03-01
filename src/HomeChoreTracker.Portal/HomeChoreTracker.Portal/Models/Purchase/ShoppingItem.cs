using HomeChoreTracker.Portal.Constants;

namespace HomeChoreTracker.Portal.Models.Purchase
{
    public class ShoppingItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Quantity { get; set; }
        public QuantityType QuantityType { get; set; }
        public ProductType ProductType { get; set; }
        public bool IsCompleted { get; set; }
        public int PurchaseId { get; set; }
        public int HomeId { get; set; }
    }
}
