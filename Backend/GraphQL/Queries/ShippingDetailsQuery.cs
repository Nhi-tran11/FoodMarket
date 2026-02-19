using Backend.Model;
using Backend.Service;

namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ShippingDetailsQuery
    {
        public async Task<ShippingDetail?> GetShippingDetailByDefaultAsync(int customerId, 
            [Service] IShippingDetailService shippingDetailService)
        {
            return await shippingDetailService.GetDefaultShippingDetailByCustomerIdAsync(customerId);
        }
    }
 }
