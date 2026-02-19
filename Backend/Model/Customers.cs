namespace Backend.Model
{
    public class Customers
    {
        public int Id{get; set;}
        public string Email{get; set;}=string.Empty;
        public string Password{get; set;}=string.Empty;
        public string Role{get; set;}=string.Empty;
        public ICollection<ShippingDetail> ShippingDetails {get; set;} = new List<ShippingDetail>();
    
    }

}