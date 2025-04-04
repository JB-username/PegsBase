using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PegsBase.Data;
using System.Net.Mail;

namespace PegsBase.Services.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;

        public EmailSender(ApplicationDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var whitelist = await _dbContext.WhitelistedEmails.Select(e => e.Email).ToListAsync();

            if (!whitelist.Contains(email, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"❌ Blocked email attempt to: {email}");
                return;
            }

            using var smtp = new SmtpClient("127.0.0.1", 25)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 10000
            };

            var fromAddress = "noreply@ampelondigital.com";

            var mail = new MailMessage
            {
                From = new MailAddress(fromAddress, "Ampelon Digital"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(email);

            try
            {
                Console.WriteLine($"📧 To: [{email}]");
                Console.WriteLine($"📨 Subject: {subject}");
                Console.WriteLine($"📝 Body Preview: {htmlMessage.Substring(0, Math.Min(100, htmlMessage.Length))}...");

                await smtp.SendMailAsync(mail);

                Console.WriteLine("✅ Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                throw;
            }
        }

    }
}
