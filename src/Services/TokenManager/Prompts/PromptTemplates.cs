namespace TokenManager.Prompts
{
    internal static class PromptTemplates
    {
        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string SystemPromptTemplate = """
            You are an intelligent assistant tasked with summarizing chat history. Your objective is to concisely capture the key points and essence of the conversations, ensuring that the summary is as brief as possible without omitting any crucial information.
            """;


        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string MainPromptTemplate = """
            <|im_start|>
            Please provide a clear and succinct summary of the chat history presented below. Focus on highlighting the key points and any conclusions that were reached. The summary should be as brief as possible while still capturing the essential elements of the conversation.
            
            Text to summarize:
            {{$prompt}}
            
            <|im_end|>
            """;
    }
}
