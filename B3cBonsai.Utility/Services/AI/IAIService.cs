namespace B3cBonsai.Utility.Services.AI
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(string userMessage, string context = "");
    }
}
