using Backend.Data;
using Backend.Model;
using Backend.Service;
using Stripe;

namespace Backend.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class PaymentMutation
    {
        public async Task<Payment> CreatePaymentAsync( int orderId, decimal amount, string currency, string stripePaymentIntentId,string status,
        [Service] IPaymentService paymentService)
        {
            return await paymentService.RecordPaymentAsync(orderId, amount, currency, stripePaymentIntentId, status);
        }
        public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(decimal amount, string currency, int orderId,
            [Service] IPaymentService paymentService)
        {
            var paymentIntent = await paymentService.CreatePaymentIntentAsync(amount, currency, orderId);
            
            if (paymentIntent.ClientSecret == null)
            {
                throw new Exception("Failed to create payment intent");
            }

            return new PaymentIntentResponse
            {
                Id = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                Status = paymentIntent.Status
            };
        }
    }
}