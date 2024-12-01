using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using B3cBonsai.Utility.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Hosting;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Drawing;
using Org.BouncyCastle.Crypto.Macs;

namespace B3cBonsai.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings emailSettings;
        public EmailSender(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // Đọc nội dung tệp HTML
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mail", "template.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder với dữ liệu thật
            var htmlBody = htmlTemplate
                .Replace("{{title}}", subject)
                .Replace("{{time}}", DateTime.UtcNow.ToString("dd/MM/yyyy"))
                .Replace("{{name}}", "B3cBonsai")
                .Replace("{{email}}", toEmail)
                .Replace("{{message}}", message);

            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        public async Task SendEmailContactAsync(string name, string emailContact, string phoneNumber, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse("smtpmvc555@gmail.com")); // Địa chỉ email người nhận
            email.Subject = "Liên hệ từ khách hàng";

            // Đọc nội dung tệp HTML
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mail", "contact-template.html");
            var htmlTemplate = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder với dữ liệu thật
            var htmlBody = htmlTemplate
                .Replace("{{name}}", name)
                .Replace("{{email}}", emailContact)
                .Replace("{{phone}}", phoneNumber)
                .Replace("{{message}}", message);

            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);



            var emailResend = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(emailContact)); // Địa chỉ email người nhận
            email.Subject = "Liên hệ từ khách hàng";

            // Đọc nội dung tệp HTML
            var templatePathResend = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mail", "resend-contact-template.html");
            var htmlTemplateResend = await File.ReadAllTextAsync(templatePath);

            // Thay thế các placeholder với dữ liệu thật
            var htmlBodyResend = htmlTemplate
                .Replace("{{name}}", name)
                .Replace("{{email}}", emailContact)
                .Replace("{{phone}}", phoneNumber)
                .Replace("{{message}}", message);

            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtpResend = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }

}