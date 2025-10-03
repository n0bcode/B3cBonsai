using System.Threading.Tasks;

namespace B3cBonsai.Utility.Services.Email.Abstractions
{
    public interface IEmailTemplateReader
    {
        Task<string> ReadTemplateAsync(string templateName);
    }
}
