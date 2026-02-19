using Backend.Data;
using Backend.Model;
using Stripe;

namespace Backend.Service
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, int orderId);
                Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId);
        Task<Payment> RecordPaymentAsync(int orderId, decimal amount, string currency, string stripePaymentIntentId, string status);
        string GetStripePublishableKey();
    }
    public class PaymentService : IPaymentService

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public PaymentService (ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            // Set Stripe API key from environment variable
            var stripeKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") 
                ?? _configuration["Stripe:SecretKey"];
            
            if (string.IsNullOrEmpty(stripeKey))
            {
                throw new InvalidOperationException("Stripe API key is not configured. Please set STRIPE_SECRET_KEY environment variable.");
            }
            
            StripeConfiguration.ApiKey = stripeKey;
        }

        // Implement the methods here
        public Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, int orderId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount *100), // Convert to cents
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" },
                Description = $"Order #{orderId}",
                Metadata = new Dictionary<String, String>
                {
                    { "orderId", orderId.ToString() },
                    {"integrationCheck", "accept_a_payment" }
                }

            };
            var service = new PaymentIntentService();
            return service.CreateAsync(options);
        }

        public Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return service.ConfirmAsync(paymentIntentId);
        }
        public async Task<Payment> RecordPaymentAsync(int orderId, decimal amount, string currency, string stripePaymentIntentId, string status)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Currency = currency,
                StripePaymentIntentId = stripePaymentIntentId,
                Status = status
            };
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
            return payment;
        }

        public string GetStripePublishableKey()
        {
            var publishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY") 
                ?? _configuration["Stripe:PublishableKey"];
            if (string.IsNullOrEmpty(publishableKey))
            {
                throw new InvalidOperationException("Stripe publishable key is not configured. Please set STRIPE_PUBLISHABLE_KEY environment variable.");
            }
            return publishableKey;
        }
    }
}