using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace B3cBonsai.Utility.Services.AI
{
    public class GeminiAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiAIService> _logger;

        public GeminiAIService(IConfiguration configuration, ILogger<GeminiAIService> logger)
        {
            _apiKey = configuration["Gemini:ApiKey"];
            _model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string context = "")
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "Lỗi: Gemini API Key chưa được cấu hình. Vui lòng kiểm tra appsettings.json.";
            }

            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

                var systemPrompt = "Bạn là chuyên gia tư vấn cây cảnh Bonsai của cửa hàng B3cBonsai. " +
                                   "Hãy trả lời khách hàng một cách thân thiện, chuyên nghiệp và hữu ích. " +
                                   "Nếu có ngữ cảnh về sản phẩm, hãy sử dụng nó để tư vấn chính xác hoặc gợi ý các sản phẩm liên quan. " +
                                   "Bạn CÓ THỂ sử dụng Markdown để định dạng câu trả lời (in đậm, danh sách, v.v.) để thông tin dễ đọc hơn. " +
                                   "Hãy trả lời bằng tiếng Việt.";

                var fullPrompt = $"{systemPrompt}\n\nNgữ cảnh: {context}\n\nCâu hỏi khách hàng: {userMessage}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    }
                };

                var jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                
                // Trích xuất text từ format của Gemini API
                var responseText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return responseText ?? "Xin lỗi, tôi không thể tìm thấy câu trả lời lúc này.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Gemini API");
                return "Đã xảy ra lỗi khi kết nối với máy chủ AI. Vui lòng thử lại sau.";
            }
        }
    }
}
