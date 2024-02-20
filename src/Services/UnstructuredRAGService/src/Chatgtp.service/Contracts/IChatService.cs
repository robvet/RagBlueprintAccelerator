using UnstructuredRAG.Service.Models;
using UnstructuredRAG.Service.Options;

namespace UnstructuredRAG.Service.Contracts
{
    public interface IChatService
    {
        Task<List<Session>> GetAllChatSessionsAsync(string correlationToken, bool returnMessages);

        Task<List<Message>> GetChatSessionMessagesAsync(string? sessionId, string correlationToken);

        Task<Session> CreateNewChatSessionAsync(string correlationToken);

        Task<Session> GetChatSessionByIdAsync(string sessionId, bool returnMessages, string correlationToken);

        Task<Session> UpdateChatSessionAsync(Session session, string correlationToken);

        Task<bool> DeleteChatSessionAsync(string sessionId, string correlationToken);

        Task<Completion> PostChatCompletionAsync(string sessionId, string prompt, CompletionOptions promptSensitivity, string correlationToken);

        Task<Summary> RenameChatSessionAsync(string sessionId, string prompt, string correlationToken);
    }
}