namespace Backend.Data;
using Microsoft.EntityFrameworkCore;
using Backend.Model;
using System.Security.Cryptography;
using System.Text;

public static class DbSeederAdmin
{
    public static void SeedAdminUser(ApplicationDbContext dbContext)
    {
        // Check if admin user already exists
        if(dbContext.Customers.Any(c => c.Role == "Admin"))
        {
            return; // Admin user already exists
        }
        using (var sha256 = SHA256.Create())
        {
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes("Admin@123")));
            var adminCustomer = new Customers
            {
                Email="admin@example.com",
                Password=hashedPassword,
                Role="Admin"
            };
            dbContext.Customers.Add(adminCustomer);
            dbContext.SaveChanges();
        }
    }
}