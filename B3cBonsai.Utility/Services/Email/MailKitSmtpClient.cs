using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using B3cBonsai.Utility.Services.Email.Abstractions;

namespace B3cBonsai.Utility.Services.Email
{
    public class MailKitSmtpClient : B3cBonsai.Utility.Services.Email.Abstractions.ISmtpClient
    {
        private readonly SmtpClient _smtpClient;

        public MailKitSmtpClient()
        {
            _smtpClient = new SmtpClient();
        }

        public Task ConnectAsync(string host, int port, SecureSocketOptions options, System.Threading.CancellationToken cancellationToken = default)
        {
            return _smtpClient.ConnectAsync(host, port, options, cancellationToken);
        }

        public Task AuthenticateAsync(string userName, string password, System.Threading.CancellationToken cancellationToken = default)
        {
            return _smtpClient.AuthenticateAsync(userName, password, cancellationToken);
        }

        public Task SendAsync(MimeMessage message, System.Threading.CancellationToken cancellationToken = default)
        {
            return _smtpClient.SendAsync(message, cancellationToken);
        }

        public Task DisconnectAsync(bool quit, System.Threading.CancellationToken cancellationToken = default)
        {
            return _smtpClient.DisconnectAsync(quit, cancellationToken);
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}
