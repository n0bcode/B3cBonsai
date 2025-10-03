using System.IO;
using System.Threading.Tasks;
using B3cBonsai.Utility.Services.Email.Abstractions;

namespace B3cBonsai.Utility.Services.Email
{
    public class FileSystemEmailTemplateReader : IEmailTemplateReader
    {
        public Task<string> ReadTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "mail", templateName);
            return File.ReadAllTextAsync(templatePath);
        }
    }
}
