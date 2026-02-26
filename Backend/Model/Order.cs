namespace Backend.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "pending"; // pending, paid, shipped, delivered, cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        //Customer
                
        public Customers? Customer { get; set; }
        // Shipping info
        public int ShippingDetailId { get; set; }
        public ShippingDetail? ShippingDetail { get; set; }
        
        // Order items - what products were purchased
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Payment
        public Payment? Payment { get; set; }
        //Discount Used
        public int? DiscountId { get; set; }
        public Discount? AppliedDiscount { get; set; }
        public decimal? DiscountAmount { get; set; }
        // Referral code used for this order
        public int? UsedReferalCodeId { get; set; }
        public ReferalCode? UsedReferalCode { get; set; }
    }
}
