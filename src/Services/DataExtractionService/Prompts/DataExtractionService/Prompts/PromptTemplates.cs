﻿namespace DataExtractionService.Prompts.DataExtractionService.Prompts
{
    internal static class PromptTemplates
    {
        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string SystemPromptTemplate = """
            Task: Extract key financial data from the provided document, which may be a balance sheet or an income statement.

            1. Identify the type of financial document: Is it a balance sheet or an income statement?
            2. Based on the document type, extract the following key information:
               - If it's a balance sheet, identify and list the total assets, total liabilities, and shareholder's equity.
               - If it's an income statement, identify and list the total revenue, gross profit, operating expenses, and net income.
            3. Format the extracted data clearly. For example:
               - "Total Assets: [amount]"
               - "Total Revenue: [amount]"
            4. Highlight any unusual items or discrepancies in the financial data that might require further attention.
            5. If the document is part of a series (e.g., quarterly reports), compare the extracted data with previous periods to identify significant trends or changes.
            6. Summarize the overall financial health indicated by the document, based on the extracted data.
            """;

        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string MainPromptTemplate = """
            <|im_start|>system
            {{$prompt}}
            Answer the question and return with a markdown format.
            <|im_end|>
            """;

        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string SugestionsPromptTemplate = """
            <|im_start|>system
            Generate three follow-up question based on the answer you just generated.
            # Answer
            {{$answer}}
 
            Remove quotes, brackets and any other special characters from the answer.
            Each follow-up question should be no more than 15 words

            Here's a few examples of good search queries:
            # Format of the response
            Return the follow-up question as a json string list.
            e.g.
            [
                "What is the deductible?",
                "What is the co-pay?",
                "What is the out-of-pocket maximum?"
            ]
            <|im_end|>
            """;



        /// <summary>
        /// Prompt template for for extracting keyword search from user question to be used 
        /// against the search engine to build retriever.
        /// </summary>
        public const string QueryPromptTemplate = """
            <|im_start|>system
            Chat history:
            {{$chat_history}}
            
            Here's a few examples of good search queries:
            ### Good example 1 ###
            Prices and payment methods
            ### Good example 2 ###
            Menu
            ### Good example 3 ###
            Wifi service
            ###


            <|im_end|>
            <|im_start|>system
            Generate search query for followup question. You can refer to chat history for context information. Just return search query and don't include any other information.
            {{$question}}
            <|im_end|>
            <|im_start|>assistant
            """;

        /// <summary>
        /// Prompt template for generating answer to user question based on response return from the retriever.
        /// </summary>
        public const string AnswerPromptTemplate = """
            <|im_start|>system
            You are a system assistant who helps the company employees with their healthcare plan questions, and questions about the employee handbook. Be brief in your answers.
            Answer ONLY with the facts listed in the list of sources below. If there isn't enough information below, say you don't know. Do not generate answers that don't use the sources below.


            For tabular information return it as an html table. Do not return markdown format.
            Each source has a name followed by colon and the actual information, ALWAYS reference source for each fact you use in the response. Use square brakets to reference the source. List each source separately.
     


            Here're a few examples:
            ### Good Example 1 (include source) ###
            Apple is a fruit[reference1.pdf].
            ### Good Example 2 (include multiple source) ###
            Apple is a fruit[reference1.pdf][reference2.pdf].
            ### Good Example 2 (include source and use double angle brackets to reference question) ###
            Microsoft is a software company[reference1.pdf].  <<followup question 1>> <<followup question 2>> <<followup question 3>>
            ### END ###
            Sources:
            {{$sources}}

            Chat history:
            {{$chat_history}}
            <|im_end|>
            <|im_start|>user
            {{$question}}
            <|im_end|>
            <|im_start|>assistant
            """;

        public const string SystemMessage = @"You are a virtual assistant from a Microsoft Partner. Your goal is to answer all customer questions to help them understand the Azure cloud platform. Responses to customers should be friendly and include appropriate emojis. Your responses should be in the same language as the user's question.";
        //public const string SystemMessage = @"You are a virtual assistant from the Marriott Hotel. Your goal is to answer all customer questions to help them make a reservation. Responses to customers should be friendly and include appropriate emojis. Your responses should be in the same language as the user's question.";
    }
}