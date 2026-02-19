namespace Backend.Model;

public class Discount
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpiresAt { get; set; }

}