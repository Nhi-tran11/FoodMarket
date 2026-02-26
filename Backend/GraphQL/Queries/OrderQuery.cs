using Backend.Model;
using Backend.Service;

namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class OrderQuery
    {
        public async Task<Order?> GetOrderPendingByUserIdAsync(
            int userId,
            [Service] IOrderService orderService)
        {
            return await orderService.GetOrderPendingByUserIdAsync(userId);
        }

        
    }
}