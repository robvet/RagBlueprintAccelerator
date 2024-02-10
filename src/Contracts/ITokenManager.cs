using Shared.Models;

namespace Contracts
{
    public interface ITokenManager
    {
        Task<List<ChatMessage>> OptimizeChatHistoryAsync(List<ChatMessage> chatMessages);
    }
}