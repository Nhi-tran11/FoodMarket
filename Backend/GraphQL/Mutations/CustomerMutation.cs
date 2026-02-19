using Backend.Data;
using Backend.Model;
using Backend.Service;
namespace Backend.GraphQL.Mutations
{
   [ExtendObjectType("Mutation")]
    public class CustomerMutation
    {
        
        public async Task<Customers> CreateCustomerAsync(
           string email, string password,
            [Service] ApplicationDbContext dbContext,
            [Service] IAuthService authService)
        {
            
             return await authService.RegisterUserAsync(email, password);
             
        }
    }
}