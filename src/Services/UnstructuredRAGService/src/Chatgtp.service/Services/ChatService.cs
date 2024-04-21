using Azure.AI.OpenAI;
using UnstructuredRAG.Service.Constants;
using UnstructuredRAG.Service.Contracts;
using UnstructuredRAG.Service.Models;
using UnstructuredRAG.Service.Options;

namespace UnstructuredRAG.Service.Services;

public class ChatService : IChatService
{
    private readonly IDataRepository _dataStore;
    private readonly IOpenAiService _openAiService;
    private int _maxConversationTokens;

    //  Avoid string interpolation (the '$' thing) when logging for performance lag. Instead, use standard 'Composite Formatting'
    private readonly ILogger<ChatService> _logger;

    public ChatService(IOpenAiService openAiService,
                       IDataRepository cosmosDbService,
                       string maxConversationTokens,
                       ILogger<ChatService> logger)
    {
        _dataStore = cosmosDbService;
        _openAiService = openAiService;
        _logger = logger;

        // Most modern models have a token limit of 4096 tokens.
        // By default, enable the prompt to be up to 4000 tokens, or limit passed at instantiation.
        // Leave remaining tokens for the completion.
        _maxConversationTokens = int.TryParse(maxConversationTokens, out _maxConversationTokens) ? _maxConversationTokens : 4000;
    }

    /// <summary>
    /// Creates a new Chat Session
    /// </summary>https://portal.azure.com/#
    public async Task<Session> CreateNewChatSessionAsync(string correlationToken)
    {
        try
        {
            Session session = new();

            //List<Session> _sessions = new();
            //_sessions.Add(session);

            return await _dataStore.InsertSessionAsync(session, correlationToken);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw in CreateNewChatSessionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Returns list of chat session with option to return conversation messages for each
    /// </summary>
    public async Task<List<Session>> GetAllChatSessionsAsync(string correlationToken, bool returnMessages)
    {
        try
        {
            var _sessions = await _dataStore.GetChatSessionsAsync(returnMessages, correlationToken);
            return _sessions;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all Chat Sessions() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Returns chat session for given sessionId with option to return conversation messages
    /// </summary>
    public async Task<Session> GetChatSessionByIdAsync(string sessionId, bool returnMessages, string correlationToken)
    {
        try
        {
            var currentSession = await _dataStore.GetChatSessionByIdAsync(sessionId, returnMessages, correlationToken);

            if (currentSession == null)
            {
                return null;
            }

            return currentSession;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all GetChatSessionByIdAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Returns chat messages for a given SessionId
    /// </summary>
    public async Task<List<Message>> GetChatSessionMessagesAsync(string? sessionId, string correlationToken)
    {
        try
        {
            // Retrieve traget session
            var currentSession = await GetChatSessionByIdAsync(sessionId, true, correlationToken);

            if (currentSession == null)
            {
                return null;
            }

            List<Message> chatMessages = currentSession.Messages;

            return chatMessages;


            //List<Session> _sessions = await RehydrateSessions(correlationToken);
            //if (_sessions.Count == 0)
            //{
            //    return Enumerable.Empty<Message>().ToList();
            //}
            //int index = _sessions.FindIndex(s => s.SessionId == sessionId);
            //if (_sessions[index].Messages.Count == 0)
            //{
            //    // Messages are not cached, go read from database
            //    chatMessages = await _dataStore.GetSessionMessagesAsync(sessionId, correlationToken);
            //    // Cache results
            //    _sessions[index].Messages = chatMessages;
            //}
            //else
            //{
            //    // Load from cache
            //    chatMessages = _sessions[index].Messages;
            //}


        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all GetChatSessionMessagesAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Updates a chat session 
    /// </summary>
    public async Task<Session> UpdateChatSessionAsync(Session session, string correlationToken)
    {
        try
        {
            var currentSession = await _dataStore.GetChatSessionByIdAsync(session.SessionId, false, correlationToken);

            if (currentSession == null)
            {
                return null;
            }

            var updatedSession = await _dataStore.UpdateSessionAsync(currentSession, correlationToken);

            return updatedSession;

            //List<Session> _sessions = await RehydrateSessions(correlationToken);
            //int index = _sessions.FindIndex(s => s.SessionId == session.SessionId);
            //_sessions[index] = session;

        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all RenameChatSessionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Renames a chat session by summarizing the prompt
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="prompt"></param>
    /// <param name="correlationToken"></param>
    /// <returns></returns>
    public async Task<Summary> RenameChatSessionAsync(string sessionId, string prompt, string correlationToken)
    {
        try
        {
            // Get target session
            var currentSession = await _dataStore.GetChatSessionByIdAsync(sessionId, true, correlationToken);

            if (currentSession == null)
            {
                return null;
            }

            // Call to OpenAI to summarize prompt
            var completion = await _openAiService.SummarizeAsync(sessionId, prompt, correlationToken);

            currentSession.Name = completion;

            var response = await _dataStore.UpdateSessionAsync(currentSession, correlationToken);

            //await RenameChatSessionAsync(sessionId, completion, correlationToken);

            return new Summary { Name = completion };
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all SummarizeChatSessionNameAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// User deletes a chat session
    /// </summary>
    public async Task<bool> DeleteChatSessionAsync(string sessionId, string correlationToken)
    {
        try
        {
            // Get target session
            var currentSession = await _dataStore.GetChatSessionByIdAsync(sessionId, false, correlationToken);

            if (currentSession == null)
            {
                return false;
            }

            await _dataStore.DeleteSessionAndMessagesAsync(sessionId, correlationToken);

            return true;

            //// Rehydrate sessions
            //List<Session> _sessions = await RehydrateSessions(correlationToken);
            //int index = _sessions.FindIndex(s => s.SessionId == sessionId);
            //_sessions.RemoveAt(index);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all DeleteChatSessionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Implements a single prompt/completion conversation
    /// </summary>
    public async Task<Completion> PostChatCompletionAsync(string sessionId, string prompt, CompletionOptions completionOptions, string correlationToken)
    {
        int currentSessionIndex = -1;
        Session currentSession = null;

        try
        {
            // Get target session -- important that we bring back all chat messages as well
            currentSession = await _dataStore.GetChatSessionByIdAsync(sessionId, true, correlationToken);

            if (currentSession == null)
            {
                return null;
            }

            // First, create new message object for prompt
            Message promptMessage = new(currentSession.SessionId, nameof(Participants.User), default, prompt);
            currentSession.Messages.Add(promptMessage);

            // Generate EmbeddingsOptions object from CompletionOptions 
            ////EmbeddingsOptions embeddingOptions = new(prompt);
            ////OpenAIClient openAIClient = new((_openAiService);
            ////var returnValue = _openAiService.GetEmbeddings("YOUR_DEPLOYMENT_NAME", embeddingOptions);


            // Insert message into the data service
            currentSession = await _dataStore.InsertMessageAsync(currentSession, correlationToken);

            // Adds the message object with prompt to the current chat session object
            ////Message promptMessage = await AddPromptMessageAsync(sessionId, prompt, correlationToken, sessions);

            // Pull out the token limit from the completion options that will be allowed to build prompt.
            var tokenLimit = completionOptions.TokenLimit;

            // Build the chat conversation state managing newest to oldest up to max conversation tokens
            string conversation = BuildChatConversation(currentSession, correlationToken, tokenLimit);

            //string conversation = BuildChatConversation(sessionId, correlationToken, sessions, tokenLimit);

            // Call OpenAI to get completion
            (string response, int promptTokens, int responseTokens) = await _openAiService.PostChatCompletionAsync(sessionId, completionOptions, conversation, correlationToken);

            // Call helper method that adds completion message to session
            var tokensUsed = await AddPromptCompletionMessageAsync(currentSession, promptTokens, responseTokens, promptMessage, response, correlationToken);

            return new Completion { Response = response, TokensUsed = tokensUsed };
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception throw fetching all GetChatCompletionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Get current conversation from newest to oldest up to max conversation tokens and add to the prompt
    /// </summary>
    private string BuildChatConversation(Session currentSession, string correlationToken, int tokenLimit)
    {
        // Set the max conversation tokens to the default if not set
        if (tokenLimit > 0)
            _maxConversationTokens = tokenLimit;

        int? tokensUsed = 0;

        List<string> conversationBuilder = new List<string>();

        //int index = sessions.FindIndex(s => s.SessionId == sessionId);

        List<Message> messages = currentSession.Messages;

        //Start at the end of the list and work backwards
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            // We're storing number of tokens for each conversation as provided by OpenAI in the message object
            tokensUsed += messages[i].Tokens is null ? 0 : messages[i].Tokens;

            // Drops trailing messages if the max conversation tokens is exceeded
            if (tokensUsed > _maxConversationTokens)
                break;

            conversationBuilder.Add(messages[i].Text);
        }

        //Invert the chat messages to put back into chronological order and output as string.        
        string conversation = string.Join(Environment.NewLine, conversationBuilder.Reverse<string>());

        return conversation;
    }



    /// <summary>
    /// Add user prompt and AI assistance response to the chat session message list object and insert into the data service as a transaction.
    /// </summary>
    private async Task<int> AddPromptCompletionMessageAsync(Session currentSession,
                                                        int promptTokens,
                                                        int completionTokens,
                                                        Message promptMessage,
                                                        string completionText,
                                                        string correlationToken)
    {
        //int index = sessions.FindIndex(s => s.SessionId == sessionId);

        //Create completion message, add to the cache
        Message completionMessage = new(currentSession.SessionId, nameof(Participants.Assistant), completionTokens, completionText);
        currentSession.AddMessage(completionMessage);

        //Update prompt message with tokens used and insert into the cache
        Message updatedPromptMessage = promptMessage with { Tokens = promptTokens };

        // not sure we need this
        currentSession.UpdateMessage(updatedPromptMessage);


        //Update session with tokens users and udate the cache
        currentSession.TokensUsed += updatedPromptMessage.Tokens;
        currentSession.TokensUsed += completionMessage.Tokens;

        currentSession = await _dataStore.InsertMessageAsync(currentSession, correlationToken);

        // original code
        ////await _cosmosDbService.UpsertSessionBatchAsync(correlationToken, updatedPromptMessage, completionMessage, currentSession);

        return (int)currentSession.TokensUsed;
    }


}




/// <summary>
/// Get a completion from _openAiService
/// </summary>
//public async Task<Completion> GetChatCompletionAsync(string? sessionId, string prompt, string correlationToken)
//{
//    ArgumentNullException.ThrowIfNull(sessionId);

//    try
//    {
//        // Rehydrate chat session state for this operation
//        List<Session> sessions = RehydrateSessions(correlationToken).Result;

//        // Adds the message object with prompt to the current chat session object
//        Message promptMessage = await AddPromptMessageAsync(sessionId, prompt, correlationToken, sessions);

//        // Get conversation from newest to oldest up to max conversation tokens
//        string conversation = GetChatSessionConversation(sessionId, correlationToken, sessions);

//        // Get completion from OpenAI
//        (string response, int promptTokens, int responseTokens) = await _openAiService.GetChatCompletionAsync(sessionId, conversation);

//        var tokensUsed = await AddPromptCompletionMessagesAsync(sessionId, promptTokens, responseTokens, promptMessage, response, correlationToken, sessions);

//        return new Completion { Response = response, TokensUsed = tokensUsed };
//    }
//    catch (Exception ex)
//    {
//        var errorMessage = $"Exception throw fetching all GetChatCompletionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
//        _logger.LogError($"Error creating new chat session: {errorMessage}");
//        throw;
//    }
//}


///// <summary>
///// Get current conversation from newest to oldest up to max conversation tokens and add to the prompt
///// </summary>
//private string BuildChatConversation(string sessionId, string correlationToken, List<Session> sessions, int tokenLimit)
//{
//    // Set the max conversation tokens to the default if not set
//    if (tokenLimit > 0)
//        _maxConversationTokens = tokenLimit;

//    int? tokensUsed = 0;

//    List<string> conversationBuilder = new List<string>();

//    int index = sessions.FindIndex(s => s.SessionId == sessionId);

//    List<Message> messages = sessions[index].Messages;

//    //Start at the end of the list and work backwards
//    for (int i = messages.Count - 1; i >= 0; i--)
//    {
//        // We're storing number of tokens for each conversation as provided by OpenAI in the message object
//        tokensUsed += messages[i].Tokens is null ? 0 : messages[i].Tokens;

//        // Drops trailing messages if the max conversation tokens is exceeded
//        if (tokensUsed > _maxConversationTokens)
//            break;

//        conversationBuilder.Add(messages[i].Text);
//    }

//    //Invert the chat messages to put back into chronological order and output as string.        
//    string conversation = string.Join(Environment.NewLine, conversationBuilder.Reverse<string>());

//    return conversation;
//}

///// <summary>
///// Add user prompt and AI assistance response to the chat session message list object and insert into the data service as a transaction.
///// </summary>
//private async Task<int> AddPromptCompletionMessagesAsync(string sessionId, 
//                                                    int promptTokens, 
//                                                    int completionTokens, 
//                                                    Message promptMessage, 
//                                                    string completionText, 
//                                                    string correlationToken,
//                                                    List<Session> sessions)
//{

//    int index = sessions.FindIndex(s => s.SessionId == sessionId);

//    //Create completion message, add to the cache
//    Message completionMessage = new(sessionId, nameof(Participants.Assistant), completionTokens, completionText);
//    sessions[index].AddMessage(completionMessage);

//    //Update prompt message with tokens used and insert into the cache
//    Message updatedPromptMessage = promptMessage with { Tokens = promptTokens };

//    sessions[index].UpdateMessage(updatedPromptMessage);


//    //Update session with tokens users and udate the cache
//    sessions[index].TokensUsed += updatedPromptMessage.Tokens;
//    sessions[index].TokensUsed += completionMessage.Tokens;

//    // something not passed here correctly
//    await _cosmosDbService.UpsertSessionBatchAsync(correlationToken, updatedPromptMessage, completionMessage, sessions[index]);

//    return (int) sessions[index].TokensUsed;
//}


/// <summary>
/// Add user prompt to the chat session message list object and insert into the data service.
/// </summary>
//private async Task<Session> SavePromptToStateStore(Session currentSession, string promptText, string correlationToken)
//{
//    // New message object for prompt
//    Message promptMessage = new(currentSession.SessionId, nameof(Participants.User), default, promptText);

//    currentSession.Messages.Add(promptMessage);

//    // Insert message into the data service
//    return await _cosmosDbService.InsertMessageAsync(currentSession, correlationToken);
//}


/// <summary>
/// Add user prompt to the chat session message list object and insert into the data service.
/// </summary>
//private async Task<Message> AddPromptMessageAsync(string sessionId, string promptText, string correlationToken, List<Session> sessions)
//{
//    // New message object
//    Message promptMessage = new(sessionId, nameof(Participants.User), default, promptText);

//    int index = -1;

//    try
//    {
//        index = sessions.FindIndex(s => s.SessionId == sessionId);
//    }
//    catch (Exception ex)
//    {
//        var errorMessage = $"Exception throw fetching all AddPromptMessageAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
//        _logger.LogError($"Error creating new chat session: {errorMessage}");
//        throw;
//    }

//    // Add message to the session object
//    sessions[index].AddMessage(promptMessage);

//    // Insert message into the data service
//    return await _cosmosDbService.InsertMessageAsync(promptMessage, correlationToken);
//}

///// <summary>
///// Rename the Chat Ssssion from "New Chat" to the summary provided by OpenAI
///// </summary>
//public async Task<Session> RenameChatSessionAsync(string sessionId, string newChatSessionName, string correlationToken)
//{
//    try
//    {
//        List<Session> _sessions = await RehydrateSessions(correlationToken);

//        int index = _sessions.FindIndex(s => s.SessionId == sessionId);

//        _sessions[index].Name = newChatSessionName;

//        var response = await _dataStore.UpdateSessionAsync(_sessions[index], correlationToken);

//        return response;
//    }
//    catch (Exception ex)
//    {
//        var errorMessage = $"Exception throw fetching all RenameChatSessionAsync() in ChatService with CorrelationToken {correlationToken}: {ex.Message}";
//        _logger.LogError($"Error creating new chat session: {errorMessage}");
//        throw;
//    }
//}


///// <summary>
///// Get the chat active chat sessions from cache
///// </summary>
///// <param name="correlationToken"></param>
///// <returns>List of Active Sessions</returns>
//private async Task<List<Session>> RehydrateSessions(string correlationToken)
//{
//    var _sessions = new List<Session>();

//    //var sessions = await _cosmosDbService.GetChatSessionsAsync(correlationToken);

//    // Call store -- set returnMessages parameter to true
//    var sessions = await _dataStore.GetChatSessionsAsync(true, correlationToken);

//    foreach (var session in sessions)
//    {
//        var messages = await _dataStore.GetSessionMessagesAsync(session.SessionId, correlationToken);

//        foreach (var message in messages)
//        {
//            session.AddMessage(message);
//        }

//        _sessions.Add(session);
//    }

//    return _sessions;
//}