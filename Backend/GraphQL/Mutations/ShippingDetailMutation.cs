using Backend.Service;
using Backend.Data;
using Backend.Model;

namespace Backend.GraphQL.Mutations

{
    [ExtendObjectType("Mutation")]
    public class ShippingDetailMutation
    {
        public async Task<ShippingDetail> CreateShippingDetailAsync(
            int customerId, string fullName, string phoneNumber, string address, string city, string state, string zipCode, string country, bool isDefault,
            [Service] IShippingDetailService shippingDetailService)
        {
            return await shippingDetailService.CreateShippingDetailAsync(customerId, fullName, phoneNumber, address, city, state, zipCode, country, isDefault);
        }
        
    }
}