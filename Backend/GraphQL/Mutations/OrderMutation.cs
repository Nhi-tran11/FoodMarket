using Backend.Model;
using Backend.Service;

namespace Backend.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class OrderMutation
    {
        public async Task<Order> CreateOrder(
            int customerId,
            int shippingDetailId,
            List<OrderItemInputType> items,
            decimal subtotal,
            decimal shipping,
            decimal tax,
            [Service] IOrderService orderService)
        {
            var orderItems = items.Select(i => new OrderItemInput
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            return await orderService.CreateOrderAsync(customerId, shippingDetailId, orderItems, subtotal, shipping, tax);
        }

        public async Task<Order> UpdateOrderStatus(
            int orderId,
            string status,
            [Service] IOrderService orderService)
        {
            return await orderService.UpdateOrderStatusAsync(orderId, status);
        }
    }

    public class OrderItemInputType
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
