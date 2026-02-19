namespace Backend.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customers? Customer { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "pending"; // pending, paid, shipped, delivered, cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Shipping info
        public int ShippingDetailId { get; set; }
        public ShippingDetail? ShippingDetail { get; set; }
        
        // Order items - what products were purchased
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Payment
        public Payment? Payment { get; set; }
    }
}
