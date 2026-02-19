namespace Backend.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Products? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at time of order
        public decimal Subtotal { get; set; } // Price * Quantity
    }
}
