using B3cBonsai.Utility.Services.Email.Abstractions;

namespace B3cBonsai.Utility.Services.Email
{
    public interface ISmtpClientFactory
    {
        ISmtpClient Create();
    }

    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient Create()
        {
            return new MailKitSmtpClient();
        }
    }
}
