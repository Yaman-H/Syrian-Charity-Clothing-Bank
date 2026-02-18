using CharitySystem.Domain.Enums;

namespace CharitySystem.Domain.Entities
{
    public class Order
    {
        public int OrderID { get; set; }
        public string? OrderCode { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Processing;
        public int TotalPoints { get; set; }
        public string? OrderNotes { get; set; }
        public string? ShippingAddress { get; set; }

        // Foreign Key
        public int FamilyID { get; set; }
        public Family? Family { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];
    }
}