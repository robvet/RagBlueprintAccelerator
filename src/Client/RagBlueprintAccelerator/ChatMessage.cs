using Azure.AI.OpenAI;
using Shared.Enums;

namespace RagBlueprintAccelerator
{
    public class ChatMessage
    {
        //public ChatMessage(Role role, string content, int tokens)
        public ChatMessage(Role role, string content, int tokens)
        {
            Role = role;
            Content = content;
            Tokens = tokens;
            TimeStamp = DateTime.UtcNow;
        }
        public string Content { get; set; }
        public Role Role { get; set; }

        public DateTime TimeStamp { get; set; }

        public int Tokens { get; set; }
    }
}
