namespace Backend.Model
{
    public class Discount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Customers?  User{ get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public int? UsedInOrder { get; set; }//link to order where used
        public string Source { get; set; } = "referral"; // NEW: "referral", "promotion", etc.
        
    }
}