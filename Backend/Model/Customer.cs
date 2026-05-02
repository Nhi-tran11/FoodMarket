namespace Backend.Model
{
    public class Customer
    {
        public int Id{get; set;}
        public string Email{get; set;}=string.Empty;
        public string Password{get; set;}=string.Empty;
        public string Role{get; set;}=string.Empty;
        public ICollection<ShippingDetail> ShippingDetails {get; set;} = new List<ShippingDetail>();


        //Codes this customer 
        public ICollection<ReferalCode> ReferalCodes {get; set;} = new List<ReferalCode>();
        public DateTime? SignUpDate{get; set;} = DateTime.UtcNow;
    
    }

}