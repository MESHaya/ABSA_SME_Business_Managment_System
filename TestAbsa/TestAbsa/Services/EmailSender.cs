using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using TestAbsa.Data;

namespace TestAbsa.Services
{
    // The implementation for IEmailSender
    public class EmailSender : IEmailSender
    {
        // Replace with your actual configuration settings
        private readonly string SmtpServer = "smtp.gmail.com"; // Example: Gmail
        private readonly int SmtpPort = 587; // Standard port for TLS/STARTTLS
        private readonly string SmtpUser = "loonatsiraj@gmail.com"; // Your email address
        private readonly string SmtpPass = "nbyj khym uddt jvtc "; // IMPORTANT: Use an App Password, not your account password

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage
            {
                From = new MailAddress(SmtpUser, "TestAbsa SME Portal"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email);

            using (var client = new SmtpClient(SmtpServer, SmtpPort))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SmtpUser, SmtpPass);

                // You may need to adjust the delivery method depending on your host
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                return client.SendMailAsync(message);
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            // Identity calls this for email confirmation.
            return SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>."
            );
        }
    }
}