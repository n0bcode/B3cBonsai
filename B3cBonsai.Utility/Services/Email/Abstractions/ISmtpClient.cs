using MimeKit;
using MailKit.Security;
using System.Threading.Tasks;

namespace B3cBonsai.Utility.Services.Email.Abstractions
{
    public interface ISmtpClient : System.IDisposable
    {
        Task ConnectAsync(string host, int port, SecureSocketOptions options, System.Threading.CancellationToken cancellationToken = default);
        Task AuthenticateAsync(string userName, string password, System.Threading.CancellationToken cancellationToken = default);
        Task SendAsync(MimeMessage message, System.Threading.CancellationToken cancellationToken = default);
        Task DisconnectAsync(bool quit, System.Threading.CancellationToken cancellationToken = default);
    }
}
