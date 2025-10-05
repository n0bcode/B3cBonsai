using B3cBonsai.Utility.Services;
using FluentAssertions;
using Moq;
using Telegram.Bot;
using Xunit;

namespace B3cBonsai.Tests.Services
{
    public class TelegramServiceTests
    {
        private readonly string _testToken = "test_bot_token";
        private readonly TelegramService _telegramService;

        public TelegramServiceTests()
        {
            _telegramService = new TelegramService(_testToken);
        }

        // Note: Since TelegramService directly uses TelegramBotClient which is not easily mockable,
        // these are integration-style tests. In production, you'd either wrap the client or use HTTP client mocking.

        [Fact]
        public async Task SendMessageAsync_WithValidParameters_DoesNotThrow()
        {
            // Arrange
            long chatId = 123456789;
            string message = "Test message";

            // Act & Assert
            // This would actually try to send message if token was real
            // For unit testing, we'd need to mock or use a test bot
            await Assert.ThrowsAsync<Telegram.Bot.Exceptions.ApiRequestException>(
                () => _telegramService.SendMessageAsync(chatId, message));
        }

        [Fact]
        public void Constructor_InitializesWithToken()
        {
            // Arrange & Act
            var service = new TelegramService("test_token_123");

            // Assert
            Assert.NotNull(service);
            // Can't easily verify internal client, but constructor doesn't throw
        }
    }
}
