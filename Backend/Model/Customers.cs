namespace Backend.Model
{
    public class Customers
    {
        public int Id{get; set;}
        public string Email{get; set;}=string.Empty;
        public string Password{get; set;}=string.Empty;
        public string Role{get; set;}=string.Empty;
        public ICollection<ShippingDetail> ShippingDetails {get; set;} = new List<ShippingDetail>();
        public int? ReferredByCodeId{get; set;}// Track who referred this user
        public ReferalCode? ReferredByCode{get; set;} 
        public DateTime? SignUpDate{get; set;} = DateTime.Now;
    
    }

}