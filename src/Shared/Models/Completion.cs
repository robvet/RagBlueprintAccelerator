using Newtonsoft.Json;

namespace Shared.Models
{
    public class Completion
    {
        public Completion()
        {
            ChatHistory = new List<ChatMessage>();
        }
        public string Response { get; set; }

        public string Suggestions { get; set; }

        public int PromptTokens { get; set; }

        public int ResponseTokens { get; set; }

        public int SuggestionTokens { get; set; }

        [JsonProperty("chathistory")]
        public List<ChatMessage> ChatHistory { get; set; }
    }
}
