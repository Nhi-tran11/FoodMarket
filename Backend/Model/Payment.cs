namespace Backend.Model
{
    public class Payment
    {
        public int Id {get; set;}
        public int OrderId {get; set;}
        public decimal Amount {get; set;}
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Currency { get; set; } = "nzd";
    }
}