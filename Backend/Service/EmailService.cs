using MimeKit;
using MailKit.Net.Smtp;

namespace Backend.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string bodyText, string bodyHtml, string sender, string toAddress);

    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string subject, string bodyText, string bodyHtml, string sender, string toAddress)
        {
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration["Email:SmtpPort"];

            
            if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                throw new Exception($"SMTP credentials not configured. Username: {smtpUsername}, Password exists: {!string.IsNullOrEmpty(smtpPassword)}");
            }
            
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(sender));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = bodyText,
                HtmlBody = bodyHtml
            };
            message.Body = bodyBuilder.ToMessageBody();
            using var client = new SmtpClient();
   
            await client.ConnectAsync(smtpHost, int.Parse(smtpPort ?? "587"), 
                MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUsername, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}