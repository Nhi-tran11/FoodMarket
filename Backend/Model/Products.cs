using System.ComponentModel.DataAnnotations;

namespace Backend.Model;
public class Products
{
    public int Id {get; set;}
    public string Name {get; set;}=string.Empty;
    public double Price {get; set;}
    public string Category {get; set;}=string.Empty;
    public string Description {get; set;}=string.Empty;
    public string Image {get; set;}=string.Empty;
    public bool InStock {get; set;}
    public int StockQuantity {get; set;}
    public string Unit {get; set;}=string.Empty;
    public double? PromotionPrice {get; set;}
    public DateTime? PromotionStartDate {get; set;}
    public DateTime? PromotionEndDate {get; set;}

    public double CurrentPrice
    {
        get
        {
            if (IsOnPromotion)
            {
                return PromotionPrice ?? Price;
            }
            return Price;
        }
    }

    public bool IsOnPromotion
    {
        get
        {
            if (PromotionPrice == null || PromotionStartDate == null || PromotionEndDate == null)
            {
                return false;
            }

            var now = DateTime.Now;
            return now >= PromotionStartDate.Value && now <= PromotionEndDate.Value;
        }
    }

    [Timestamp]
    public byte[]? RowVersion {get; set;}
}
