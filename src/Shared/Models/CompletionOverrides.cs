using Azure.AI.OpenAI;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Shared.Models
{
    public class CompletionOverrides
    {
        public CompletionOverrides()
        {
            StopSequences = new string[] { "\n" };
            ChatMessages = new List<ChatMessage>();
        }

        [JsonProperty("temperature")]
        [DefaultValue(0.3)]
        public float Temperature { get; set; }

        [JsonProperty("nucleus")]
        [DefaultValue(0.5)]
        public float Nucleus { get; set; }

        [JsonProperty("maxtokens")]
        [DefaultValue(4000)]
        public int MaxTokens { get; set; }

        //[JsonProperty("userprompt")]
        //[DefaultValue("You are an AI assistant that helps people find information. Provide concise answers that are polite and professional.")]
        //public string UserPrompt { get; set; }

        //[JsonProperty("systemprompt")]
        //[DefaultValue("You are an AI assistant that helps people find information. Provide concise answers that are polite and professional.")]
        //public string SystemPrompt { get; set; }

        [JsonProperty("frequencypenalty")]
        [DefaultValue(0)]
        public float FrequencyPenalty { get; set; }

        [JsonProperty("stopsequences")]
        public string[] StopSequences { get; set; }

        [JsonProperty("presencepenalty")]
        [DefaultValue(0)]
        public float PresencePenalty { get; set; }

        //[JsonProperty("systemprompt")]
        //public ChatRequestSystemMessage SystemPrompt { get; set; }

        [JsonProperty("userprompt")]
        //public ChatRequestUserMessage UserPrompt { get; set; }
        public string UserPrompt { get; set; }

        //[JsonProperty("chathistory")]
        ////public ChatRequestAssistantMessage ChatHistory { get; set; }
        //public string ChatHistory { get; set; }

        [JsonProperty("chatmessage")]
        public List<ChatMessage> ChatMessages { get; set; }
    }
}
