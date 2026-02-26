
using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using HotChocolate;
namespace Backend.Service;

public interface IAuthService
{
    Task<Customers> AuthenticateAsync(string email, string password);
    Task<Customers> RegisterUserAsync(string email, string password, string? referralCode);
    Task<Customers?> GetCustomersAsync(string email);
}
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    public AuthService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Customers> RegisterUserAsync(string email, string password, string? referralCode )
    {
        var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        if (existingCustomer != null)
        {
            throw new GraphQLException("Customer with this email already exists.");
        }
       
        using (var sha256 = SHA256.Create())
        {
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            var newCustomer = new Customers
            {
                Email = email,
                Password = hashedPassword,
                Role = "User"
            };
            _dbContext.Add(newCustomer);
            await _dbContext.SaveChangesAsync();
            //If they have a referral code,link it to their account
            if(referralCode !=null)
            {
                var code = await _dbContext.ReferalCodes.FirstOrDefaultAsync(c => c.Code == referralCode && c.ReceivedByUserId ==null);
                if(code != null)
                {
                    code.ReceivedByUserId = newCustomer.Id;
                    await _dbContext.SaveChangesAsync();
                }
            }
            return newCustomer;
        }

    }
    public async Task<Customers> AuthenticateAsync(string email, string password)
    {
        // Implement authentication logic here or throw NotImplementedException for now
        var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        if (existingCustomer != null)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
                if (existingCustomer.Password == hashedPassword)
                {
                    return existingCustomer;
                }
                else
                {
                    throw new GraphQLException("Invalid password.");
                }
            }
        }
        throw new GraphQLException("Customer not found.");
    }
    public async Task<Customers?>GetCustomersAsync(string email)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

}