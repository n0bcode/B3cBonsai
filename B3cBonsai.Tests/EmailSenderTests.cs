using Xunit;
using Moq;
using FluentAssertions;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Helper;
using B3cBonsai.Utility.Services.Email;
using B3cBonsai.Utility.Services.Email.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Security;

namespace B3cBonsai.Tests
{
    public class EmailSenderTests
    {
        private readonly Mock<IOptions<EmailSettings>> _emailSettingsMock;
        private readonly Mock<IEmailTemplateReader> _templateReaderMock;
        private readonly Mock<ISmtpClientFactory> _smtpClientFactoryMock;
        private readonly Mock<ISmtpClient> _smtpClientMock;
        private readonly EmailSender _emailSender;

        public EmailSenderTests()
        {
            _emailSettingsMock = new Mock<IOptions<EmailSettings>>();
            _templateReaderMock = new Mock<IEmailTemplateReader>();
            _smtpClientFactoryMock = new Mock<ISmtpClientFactory>();
            _smtpClientMock = new Mock<ISmtpClient>();

            _emailSettingsMock.Setup(x => x.Value).Returns(new EmailSettings
            {
                Email = "test@example.com",
                Password = "password",
                Host = "smtp.example.com",
                Port = 587
            });

            _smtpClientFactoryMock.Setup(x => x.Create()).Returns(_smtpClientMock.Object);

            _emailSender = new EmailSender(_emailSettingsMock.Object, _templateReaderMock.Object, _smtpClientFactoryMock.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ConstructsAndSendsEmailCorrectly()
        {
            // Arrange
            var toEmail = "recipient@example.com";
            var subject = "Test Subject";
            var message = "Test Message";
            var template = "<html><body>{{message}}</body></html>";

            _templateReaderMock.Setup(x => x.ReadTemplateAsync("template.html")).ReturnsAsync(template);

            MimeMessage sentMessage = null;
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), default))
                           .Callback<MimeMessage, System.Threading.CancellationToken>((msg, ct) => sentMessage = msg);

            // Act
            await _emailSender.SendEmailAsync(toEmail, subject, message);

            // Assert
            _smtpClientMock.Verify(x => x.ConnectAsync("smtp.example.com", 587, SecureSocketOptions.StartTls, default), Times.Once);
            _smtpClientMock.Verify(x => x.AuthenticateAsync("test@example.com", "password", default), Times.Once);
            _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), default), Times.Once);
            _smtpClientMock.Verify(x => x.DisconnectAsync(true, default), Times.Once);

            sentMessage.Should().NotBeNull();
            sentMessage.To.ToList().Should().ContainSingle(x => x.ToString() == toEmail);
            sentMessage.Subject.Should().Be(subject);
            sentMessage.HtmlBody.Should().Contain(message);
        }
        [Fact]
        public async Task SendEmailContactAsync_SendsTwoEmailsCorrectly()
        {
            // Arrange
            var name = "Test User";
            var emailContact = "user@example.com";
            var phoneNumber = "1234567890";
            var message = "Test contact message";
            var contactTemplate = "<html><body>Contact: {{name}}</body></html>";
            var resendTemplate = "<html><body>Thanks for contacting us, {{name}}</body></html>";

            _templateReaderMock.Setup(x => x.ReadTemplateAsync("contact-template.html")).ReturnsAsync(contactTemplate);
            _templateReaderMock.Setup(x => x.ReadTemplateAsync("resend-contact-template.html")).ReturnsAsync(resendTemplate);

            var sentMessages = new List<MimeMessage>();
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), default))
                           .Callback<MimeMessage, System.Threading.CancellationToken>((msg, ct) => sentMessages.Add(msg));

            // Act
            await _emailSender.SendEmailContactAsync(name, emailContact, phoneNumber, message);

            // Assert
            _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), default), Times.Exactly(2));

            sentMessages.Should().HaveCount(2);

            var adminEmail = sentMessages[0];
            adminEmail.To.ToList().Should().ContainSingle(x => x.ToString() == "smtpmvc555@gmail.com");
            adminEmail.Subject.Should().Be("Liên hệ từ khách hàng");
            adminEmail.HtmlBody.Should().Contain(name);

            var userEmail = sentMessages[1];
            userEmail.To.ToList().Should().ContainSingle(x => x.ToString() == emailContact);
            userEmail.Subject.Should().Be("Cảm ơn bạn đã liên hệ");
            userEmail.HtmlBody.Should().Contain(name);
        }
    }
}
