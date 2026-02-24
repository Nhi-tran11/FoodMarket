

namespace Backend.Model
{
    public class ReferalCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ReferrerId { get; set; }
        public Customers? Referrer { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
        public int? UsedByUserId { get; set; }
        public Customers? UsedByUser { get; set; }
        public DateTime UsedAt { get; set; }
        public bool RewardClaimed { get; set; } = false;
        public int? ReferrerDiscountId { get; set; }// link to referrer's discount
        public int? NewUserDiscountId { get; set; }// link to newUser's discount

    }
}