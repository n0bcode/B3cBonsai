namespace B3cBonsaiWeb.Services
{
    using Telegram.Bot;

    public class TelegramService
    {
        private readonly TelegramBotClient _botClient;

        public TelegramService(string token)
        {
            _botClient = new TelegramBotClient(token);
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            await _botClient.SendTextMessageAsync(chatId, message);
        }
    }
}
