using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace B3cBonsai.Utility.Services
{
    public class TelegramService
    {
        private readonly TelegramBotClient? _botClient;
        private readonly ILogger<TelegramService> _logger;

        // Cập nhật constructor để nhận thêm ILogger thông qua Dependency Injection
        public TelegramService(string token, ILogger<TelegramService> logger = null)
        {
            _logger = logger;

            // Kiểm tra nếu token rỗng, null hoặc là token mặc định chưa cấu hình
            if (string.IsNullOrWhiteSpace(token) || 
                token.Contains("YOUR_TELEGRAM_BOT_TOKEN") || 
                token == "dummy_token_to_prevent_crash_check_appsettings")
            {
                _botClient = null;
                _logger?.LogWarning("TelegramBot Token chưa được cấu hình hoặc sử dụng giá trị mặc định. Tính năng gửi tin nhắn Telegram sẽ bị vô hiệu hóa.");
            }
            else
            {
                try
                {
                    _botClient = new TelegramBotClient(token);
                }
                catch (Exception ex)
                {
                    // Nếu token sai định dạng, không khởi tạo client để tránh crash ứng dụng
                    _botClient = null;
                    _logger?.LogError(ex, "Lỗi khi khởi tạo TelegramBotClient. Token có thể không hợp lệ.");
                }
            }
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            // Nếu không có client (do chưa cấu hình hoặc lỗi), bỏ qua việc gửi tin nhắn
            if (_botClient == null) return;

            try 
            {
                await _botClient.SendMessage(chatId, message);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi kết nối telegram nhưng không làm gián đoạn luồng chính
                _logger?.LogError(ex, "Không thể gửi tin nhắn đến Telegram (ChatId: {ChatId}).", chatId);
            }
        }
    }
}
