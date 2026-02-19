namespace Backend.Model;
public class ShippingDetail
{
    public int Id {get; set;}
    public int CustomerId {get; set;}
    public Customers? Customer {get; set;}
    public string FullName {get; set;} =string.Empty;
    public string PhoneNumber {get; set;}=string.Empty;
    public string Address {get; set;}=string.Empty;
    public string City {get; set;}=string.Empty;
    public string State {get; set;}=string.Empty;
    public string ZipCode {get; set;}=string.Empty;
    public string Country {get; set;}=string.Empty;
    public bool IsDefault {get; set;}
    public DateTime CreatedAt {get; set;}=DateTime.UtcNow;
}