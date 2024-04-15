using DocumentFormat.OpenXml.Presentation;
using HomeChoreTracker.Api.Constants;

namespace HomeChoreTracker.Api.Models
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
        public DateTime WasBought { get; set; }

        public int? HomeChoreTaskId { get; set; }
        public int? Time { get; set; }

        public int HomeId { get; set; }
    }
}
