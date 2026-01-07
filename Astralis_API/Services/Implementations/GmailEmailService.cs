using Astralis_API.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Astralis_API.Services.Implementations
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var email = _config["EmailSettings:SenderEmail"];
            var password = _config["EmailSettings:AppPassword"];
            var name = _config["EmailSettings:SenderName"];

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(email, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email!, name),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}