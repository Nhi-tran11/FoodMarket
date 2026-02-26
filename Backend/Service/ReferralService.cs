using System.Text;
using Amazon.SimpleEmail.Model;
using Backend.Data;
using Backend.Model;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Backend.Service;

public interface IReferralService
{
    Task<String> GenerateReferralCodeAsync(int userId,  string invitedUserEmail);
    Task<byte[]> GenerateQRCodeAsync(string payload);
    Task<InvitationResult> SendInvitationEmailAsync(int inviterId, string inviteeEmail);
    Task<List<ReferalCode>> GetReferalCodeByUserIdAsync(int userId);
    Task<ReferalCode?> GetReferalCodeByCodeAndUserIdAsync(string code, int userId);
}
public class ReferralService: IReferralService
{

    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ReferralService (ApplicationDbContext dbContext, IEmailService emailService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailService = emailService;
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
                RefererId = user.Id,
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                DiscountPercentage = 10, // Example discount, can be customized
                ExpiryDate = DateTime.UtcNow.AddDays(10) // Example expiry, can be customized

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

    public Task<byte[]> GenerateQRCodeAsync(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("QR payload cannot be empty.", nameof(payload));
        }

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

        var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(pixelsPerModule: 10);

        return Task.FromResult(bytes);
    }

    public async Task<InvitationResult> SendInvitationEmailAsync(int inviterId, string inviteeEmail)
    {
        var inviter = await _dbContext.Customers.FindAsync(inviterId);
        if (inviter == null)
        {
            return new InvitationResult
            {
                Success = false,
                Message = "Inviter not found."
            };
        }
        var existingInvitee = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == inviteeEmail);
        if(existingInvitee != null)
        {
            return new InvitationResult
            {
                Success = false,
                Message = "This email already has an account. Referral codes are only for new customers."
            };
        }
        //Get or create referral code for invitee
        string referralCode = await GenerateReferralCodeAsync(inviterId, inviteeEmail);
        //Prepare email content
        var sender = _configuration["Email:SenderAddress"];
        if (string.IsNullOrEmpty(sender))
        {
            return new InvitationResult
            {
                Success = false,
                Message = "Email sender address is not configured."
            };
        }
        var frontendUrl = _configuration["FrontendUrl"];
        var signUpUrl = $"{frontendUrl}/signup?referralCode={referralCode}";
        var inviterName = inviter.Email.Split('@')[0]; // Use part of email as name
        //Generate QR code for sign up link
        var qrCodeBytes = await GenerateQRCodeAsync(signUpUrl);
        var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
        var subject = $"{inviterName} has invited you to join FoodMarket!";
        var bodyText = $"Hi there,\n\n{inviterName} has invited you to join FoodMarket. Use the referral code {referralCode} when you sign up to get exclusive benefits!\n\nSign up here: {signUpUrl}\n\nOr scan the QR code below:\n\n[QR Code Image]\n\nLooking forward to seeing you on FoodMarket!\n\n Cheers,Food Market Team";
        var bodyHtml = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0;'>🎉 You're Invited!</h1>
    </div>
    <div style='background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 10px 10px;'>       
        <p style='font-size: 16px;'>Hi there!</p>
        <p style='font-size: 16px;'><strong>{inviterName}</strong> has invited you to join our <strong>Food Market App</strong>.</p>
        
        <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; text-align: center;'>
            <p style='margin: 0 0 10px 0; color: #666;'>Your Referral Code:</p>
            <p style='font-size: 24px; font-weight: bold; color: #667eea; letter-spacing: 2px; margin: 0;'>{referralCode}</p>
        </div>

        <div style='text-align: center; margin: 30px 0;'>
            <p style='margin: 0 0 15px 0; color: #666;'>Scan this QR code to sign up:</p>
            <img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' style='max-width: 200px; border: 2px solid #e0e0e0; border-radius: 8px; padding: 10px;' />
        </div>

        <div style='text-align: center; margin: 30px 0;'>
            <a href='{signUpUrl}' style='display: inline-block; padding: 15px 40px; background-color: #667eea; color: white; text-decoration: none; border-radius: 25px; font-weight: bold; font-size: 16px;'>Or Click Here to Sign Up</a>
        </div>

        <p style='color: #28a745; font-weight: bold; text-align: center;'>✨ Get a special discount when you sign up! ✨</p>

        <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
        
        <p style='color: #666; font-size: 14px;'>Cheers,<br/><strong>Food Market Team</strong></p>
    </div>
    <div style='text-align: center; padding: 20px; color: #999; font-size: 12px;'>
        <p>This invitation was sent by {inviterName}.</p>
    </div>
</body>
</html>";
      try{
        await _emailService.SendEmailAsync(subject, bodyText, bodyHtml, sender, inviteeEmail);
        return new InvitationResult
        {
            Success = true,
            Message = "Invitation email sent successfully."
        };
      }
      catch(Exception ex)
      {
        return new InvitationResult
        {
            Success = false,
            Message = $"Failed to send invitation email: {ex.Message}"
        };
      }
    }

    public async Task<List<ReferalCode>> GetReferalCodeByUserIdAsync(int userId)
    {
        var existingUser = await _dbContext.Customers.FindAsync(userId);
        if(existingUser == null)
        {
            throw new Exception("User not found");
        }
        var referralCode = await _dbContext.ReferalCodes
        .Where(rc => rc.RefererId == userId)
        .Include(rc => rc.Referrer)
        .Include(rc => rc.ReceivedByUser)
        .ToListAsync();
        return referralCode;
    }
    public async Task<ReferalCode?> GetReferalCodeByCodeAndUserIdAsync(string code, int userId)
    {
        // RefererId = user who created/owns the code
        var referalCode = await _dbContext.ReferalCodes
            .Include(rc => rc.Referrer)
            .Include(rc => rc.ReceivedByUser)
            .FirstOrDefaultAsync(rc => rc.Code == code && rc.ReceivedByUserId == userId);

        if (referalCode == null)
        {
            throw new Exception("Referral code not found for this user");
        }

        return referalCode;
    }

   
} 
public class InvitationResult
{
    public bool Success { get; set;}
    public string Message { get; set; } = string.Empty;
}
