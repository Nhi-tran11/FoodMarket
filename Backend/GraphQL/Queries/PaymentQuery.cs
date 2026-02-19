using Backend.Service;

namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class PaymentQuery
    {
        public string GetStripePublishableKey([Service] IPaymentService paymentService)
        {
            return paymentService.GetStripePublishableKey();
        }
    }
}