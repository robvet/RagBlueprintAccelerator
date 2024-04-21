using UnstructuredRAG.Service.Models;

namespace UnstructuredRAG.Service.Contracts
{
    public interface IDataRepository
    {
        Task<List<Session>> GetChatSessionsAsync(bool returnMessages, string corrleationToken);
        Task DeleteSessionAndMessagesAsync(string sessionId, string corrleationToken);
        Task<List<Message>> GetSessionMessagesAsync(string sessionId, string corrleationToken);
        Task<Session> GetChatSessionByIdAsync(string sessionId, bool returnMessages, string correlationToken);
        Task<Session> InsertMessageAsync(Session currentSession, string corrleationToken);
        Task<Session> InsertSessionAsync(Session session, string corrleationToken);
        Task<Session> UpdateSessionAsync(Session session, string corrleationToken);

        //Task UpsertSessionBatchAsync(string corrleationToken, params dynamic[] messages);
    }
}
