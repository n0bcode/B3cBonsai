using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using B3cBonsai.Utility.Helper;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using B3cBonsai.Utility.Services.Email.Abstractions;
using B3cBonsai.Utility.Services.Email;
using System;

namespace B3cBonsai.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailTemplateReader _templateReader;
        private readonly ISmtpClientFactory _smtpClientFactory;

        public EmailSender(
            IOptions<EmailSettings> options,
            IEmailTemplateReader templateReader,
            ISmtpClientFactory smtpClientFactory)
        {
            _emailSettings = options.Value;
            _templateReader = templateReader;
            _smtpClientFactory = smtpClientFactory;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var htmlTemplate = await _templateReader.ReadTemplateAsync("template.html");

            var htmlBody = htmlTemplate
                .Replace("{{title}}", subject)
                .Replace("{{time}}", DateTime.UtcNow.ToString("dd/MM/yyyy"))
                .Replace("{{name}}", "B3cBonsai")
                .Replace("{{email}}", toEmail)
                .Replace("{{message}}", message);

            email.Body = new TextPart("html") { Text = htmlBody };

            using var smtp = _smtpClientFactory.Create();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailContactAsync(string name, string emailContact, string phoneNumber, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse("smtpmvc555@gmail.com"));
            email.Subject = "Liên hệ từ khách hàng";

            var htmlTemplate = await _templateReader.ReadTemplateAsync("contact-template.html");

            var htmlBody = htmlTemplate
                .Replace("{{name}}", name)
                .Replace("{{email}}", emailContact)
                .Replace("{{phone}}", phoneNumber)
                .Replace("{{message}}", message);

            email.Body = new TextPart("html") { Text = htmlBody };

            using (var smtp = _smtpClientFactory.Create())
            {
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }

            var emailResend = new MimeMessage();
            emailResend.Sender = MailboxAddress.Parse(_emailSettings.Email);
            emailResend.To.Add(MailboxAddress.Parse(emailContact));
            emailResend.Subject = "Cảm ơn bạn đã liên hệ";

            var resendTemplate = await _templateReader.ReadTemplateAsync("resend-contact-template.html");

            var resendBody = resendTemplate
                .Replace("{{name}}", name)
                .Replace("{{email}}", emailContact)
                .Replace("{{phone}}", phoneNumber)
                .Replace("{{message}}", message);

            emailResend.Body = new TextPart("html") { Text = resendBody };

            using (var smtp = _smtpClientFactory.Create())
            {
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(emailResend);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}