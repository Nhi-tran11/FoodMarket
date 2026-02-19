using Backend.Data;
using Backend.Model;
using Backend.Service;
using Microsoft.EntityFrameworkCore;

namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class CustomerQuery
    {
        public async Task<Customers?>CheckAuthCustomerAsync(
            string email, 
            string password, 
            [Service] IAuthService authService)
        {
            return await authService.AuthenticateAsync(email, password);
        }
        public async Task<Customers?>CustomerByEmailAsync(
            string email,
            [Service] IAuthService authService)
        {
            return await authService.GetCustomersAsync(email);
        }
    }
}