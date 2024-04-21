using Shared.Models;

namespace Shared.Contracts
{
    public interface ITokenManager
    {
        Task<List<ChatMessage>> OptimizeChatHistoryAsync(List<ChatMessage> chatMessages);
    }
}