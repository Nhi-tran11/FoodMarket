

namespace Backend.Model
{
    public class ReferalCode
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime UsedAt { get; set; }
        //Who created the code
        public int RefererId { get; set; }
        public Customers Referrer { get; set; }=null!;
        //Who received the code
        public int? ReceivedByUserId  { get; set; }
        public Customers? ReceivedByUser { get; set; }
                // Discount details
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool RewardClaimed { get; set; } = false;
        public int? ReferrerDiscountId { get; set; }// link to referrer's discount


    }
}