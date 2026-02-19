namespace Backend.Model;

public class ReferalCode
{
    public int Id { get; set; }
    public string Code { get; set; }
    public int ReferrerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    public int? UsedByUserId { get; set; }
    public DateTime UsedAt { get; set; }
    
}