using Newtonsoft.Json;
using System.ComponentModel;

namespace Shared.Configuration
{
    /// <summary>
    /// The CompletionOptions class is part of the Shared.Configuration namespace. 
    /// This class is used for configuring certain parameters for a process, possibly related to AI model predictions or text generation.
    /// </summary>
    public static class CompletionOptions
    {
        /// <summary>
        /// This is a float value that is used to control the randomness in the model's output. A higher value produces more random results.
        /// </summary>
        [JsonProperty("temperature")]
        [DefaultValue(0.5)]
        public static float Temperature { get; set; } = .5F;

        /// <summary>
        /// This is another float value that is used to control the randomness in the model's output. It's another way to add randomness apart from temperature.
        /// </summary>
        [JsonProperty("nucleus")]
        [DefaultValue(0.5)]
        public static float Nucleus { get; set; } = .5F;

        /// <summary>
        /// This integer value refers to the maximum number of tokens the model will generate in response 
        /// to a given input. It's important to distinguish this from the total token count that includes
        /// both the input and the output in a single interaction, often referred to as a "chat turn" or "completion."
        /// When you specify max_tokens in a request to the model, you are limiting the length of the model's output.
        /// The model will stop generating tokens when it reaches the max_tokens limit or when it encounters a stop sequence.
        /// This value does not include the tokens in your input query.
        /// </summary>
        [JsonProperty("maxchatturntokens")]
        [DefaultValue(4000)]
        public static int MaxTokens { get; set; } = 4000;

        /// <summary>
        /// This integer value represents the average number of tokens that are expected
        /// for an prompt and chathistory for a typical chatturn.
        /// </summary>
        [JsonProperty("averageinputtokens")]
        [DefaultValue(700)]
        //public static int AverageInputTokens { get; set; } = 1500;
        public static int AverageInputTokens { get; set; } = 400;

        /// <summary>
        /// This float value is used to penalize common words or phrases to reduce their occurrence in the output.
        /// </summary>
        [JsonProperty("frequencypenalty")]
        [DefaultValue(0)]
        public static float FrequencyPenalty { get; set; }

        /// <summary>
        /// This is an array of strings that, when encountered, signal the model to stop generating further tokens.
        /// </summary>
        [JsonProperty("stopsequences")]
        public static string[] StopSequences { get; set; }

        /// <summary>
        /// This float value is used to penalize new words or phrases to reduce their occurrence in the output.
        /// </summary>
        [JsonProperty("presencepenalty")]
        [DefaultValue(0)]
        public static float PresencePenalty { get; set; }
    }
}
