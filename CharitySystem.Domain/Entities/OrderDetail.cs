namespace CharitySystem.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        
        public int Quantity { get; set; }
        public int Points { get; set; }

        // Foreign Keys
        public int OrderID { get; set; }
        public Order? Order { get; set; }

        public int ClothID { get; set; }
        public Cloth? Cloth { get; set; }
    }
}