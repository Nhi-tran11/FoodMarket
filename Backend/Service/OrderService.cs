using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int customerId, int shippingDetailId, List<OrderItemInput> items, decimal subtotal, decimal shipping, string? discountCode);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<Order> UpdateOrderStatusAsync(int orderId, string status);
        Task<Order?> GetOrderPendingByUserIdAsync(int userId);
        Task<String> DeletePendingOrderAsync(int orderId);
    }

    public class OrderItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> CreateOrderAsync(int customerId, int shippingDetailId, List<OrderItemInput> items, decimal subtotal, decimal shipping, string? discountCode)
        {
            decimal discountAmount = 0;
            decimal discountRate = 0;
            if(String.IsNullOrEmpty(discountCode))
            {
                discountCode = null; // Ensure empty strings are treated as null
            }
            // null ExpiryDate means no expiry (always valid)
            var referalCode = await _dbContext.ReferalCodes.FirstOrDefaultAsync(c =>
                c.Code == discountCode &&
                !c.IsUsed &&
                (c.ExpiryDate == null || c.ExpiryDate > DateTime.UtcNow));

            if (referalCode != null)
            {
               discountRate = (referalCode.DiscountPercentage ?? 0) / 100m;
               discountAmount = decimal.Round(subtotal * discountRate, 2, MidpointRounding.AwayFromZero);
            }

            var order = new Order
            {
                CustomerId = customerId,
                ShippingDetailId = shippingDetailId,
                Subtotal = subtotal,
                Shipping = shipping,
                Tax = (subtotal - discountAmount) * 10 / 100, // Example tax calculation, can be customized
                Total = subtotal - discountAmount + shipping + ((subtotal - discountAmount) * 10 / 100),
                DiscountAmount = discountAmount,
                UsedReferalCodeId = referalCode?.Id,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Add order items with per-item discounted price
            foreach (var item in items)
            {
                var discountedPrice = decimal.Round(item.Price * (1 - discountRate), 2, MidpointRounding.AwayFromZero);
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = discountedPrice,
                    Subtotal = discountedPrice * item.Quantity
                };
                _dbContext.OrderItems.Add(orderItem);
            }
            await _dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<string> DeletePendingOrderAsync(int orderId)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.Status == "pending");
            if (existingOrder == null)
            {
                throw new Exception($"Order with ID {orderId} not found");
            }

            _dbContext.Orders.Remove(existingOrder);
            await _dbContext.SaveChangesAsync();

            return $"Order with ID {orderId} has been deleted successfully.";
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingDetail)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderPendingByUserIdAsync(int userId)
        {
            return await _dbContext.Orders
                .Where(o => o.CustomerId == userId && o.Status == "pending")
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingDetail)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            // Mark referral code as used only when payment is confirmed
            if (status == "paid" && order.UsedReferalCodeId != null)
            {
                var referalCode = await _dbContext.ReferalCodes.FindAsync(order.UsedReferalCodeId);
                if (referalCode != null && !referalCode.IsUsed)
                {
                    referalCode.IsUsed = true;
                    referalCode.UsedAt = DateTime.UtcNow;
                    referalCode.ReceivedByUserId = order.CustomerId;
                }
            }

            await _dbContext.SaveChangesAsync();

            return order;
        }
    }
}
