using Ganss.Xss;

namespace B3cBonsai.Utility
{
    public interface IHtmlSanitizerService
    {
        string Sanitize(string html);
    }

    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
            // Configure specific allowed tags/attributes if needed
            // _sanitizer.AllowedTags.Add("my-tag");
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return _sanitizer.Sanitize(html);
        }
    }
}
