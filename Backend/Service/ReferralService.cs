using System.Text;
using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public interface IReferralService
{
    Task<String> GenerateReferralCodeAsync(int userId,  string invitedUserEmail);
    Task<byte[]>GenerateQRCodeAsync();
   
}
public class ReferralService: IReferralService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public ReferralService (ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }
    public async Task<string> GenerateReferralCodeAsync(int userId, string invitedUserEmail)
    {
        var user = await _dbContext.Customers.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var invitedUser = await _dbContext.Customers.FirstOrDefaultAsync(c =>c.Email == invitedUserEmail);
        if (invitedUser != null)
        {
            throw new Exception("This email already has an account. Referral codes are only for new customers.");
        }

        //Generate unique code
            string code;
            bool isUnique;
            do
            {
                code = GenerateUniqueCode(userId);
                isUnique = !await _dbContext.ReferalCodes.AnyAsync(c => c.Code == code);
            } while (!isUnique);

            //Create ReferralCode
            var referralCode = new ReferalCode
            {
                Code = code,
                ReferrerId = user.Id,
                CreatedAt = DateTime.Now,
                IsUsed = false,

            };
            _dbContext.ReferalCodes.Add(referralCode);
            await _dbContext.SaveChangesAsync();
            return referralCode.Code;
       
    }

    private string GenerateUniqueCode(int userId)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        var code = new StringBuilder();
        // Add userId prefix (encoded)
        code.Append(chars[userId % chars.Length]);
        //add random characters
        for (int i = 0; i < 7; i++)
        {
            code.Append(chars[random.Next(chars.Length)]);
        }
        return code.ToString();
    }

    public Task<byte[]> GenerateQRCodeAsync()
    {
        throw new NotImplementedException();
    }
}