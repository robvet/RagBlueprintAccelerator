using Newtonsoft.Json;
using System.ComponentModel;

namespace UnstructuredRAG.Service.Options
{
    public class CompletionOptions
    {
        [JsonProperty("temperature")]
        [DefaultValue(0.3)]
        public float Temperature { get; set; }

        [JsonProperty("nucleus")]
        [DefaultValue(0.5)]
        public float Nucleus { get; set; }

        [JsonProperty("tokenlimit")]
        [DefaultValue(4000)]
        public int TokenLimit { get; set; }

        [JsonProperty("systemprompt")]
        [DefaultValue("You are an AI assistant that helps people find information. Provide concise answers that are polite and professional.")]
        public string SystemPrompt { get; set; }

        [JsonProperty("frequencypenalty")]
        [DefaultValue(0)]
        public float FrequencyPenalty { get; set; }

        [JsonProperty("presencepenalty")]
        [DefaultValue(0)]
        public float PresencePenalty { get; set; }

    }
}
