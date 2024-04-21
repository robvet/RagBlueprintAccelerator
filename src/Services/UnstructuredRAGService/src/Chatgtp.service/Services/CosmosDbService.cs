using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using UnstructuredRAG.Service.Contracts;
using UnstructuredRAG.Service.Models;

namespace UnstructuredRAG.Service.Services;

/// <summary>
/// Implementation of IDataReposioty that consums Azure Cosmos DB
/// </summary>
public class CosmosDbService : IDataRepository
{
    private readonly Container _container;

    //  Avoid string interpolation (the '$' thing) when logging for performance lag. Instead, use standard 'Composite Formatting'
    private readonly ILogger<CosmosDbService> _logger;

    /// <summary>
    /// Creates a new instance of the service.
    /// </summary>
    /// <param name="endpoint">Endpoint URI.</param>
    /// <param name="key">Account key.</param>
    /// <param name="databaseName">Name of the database to access.</param>
    /// <param name="containerName">Name of the container to access.</param>
    /// <exception cref="ArgumentNullException">Thrown when endpoint, key, databaseName, or containerName is either null or empty.</exception>
    /// <remarks>
    /// This constructor will validate credentials and create a service client instance.
    /// </remarks>
    public CosmosDbService(string endpoint,
                           string key,
                           string databaseName,
                           string containerName,
                           ILogger<CosmosDbService> logger)
    {
        ArgumentException.ThrowIfNullOrEmpty(databaseName);
        ArgumentException.ThrowIfNullOrEmpty(containerName);
        ArgumentException.ThrowIfNullOrEmpty(endpoint);
        ArgumentException.ThrowIfNullOrEmpty(key);

        _logger = logger;

        CosmosSerializationOptions options = new()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };

        CosmosClient client = new CosmosClientBuilder(endpoint, key)
            .WithSerializerOptions(options)
            .Build();

        Database? database = client?.GetDatabase(databaseName);
        Container? container = database?.GetContainer(containerName);

        _container = container ??
            throw new ArgumentException("Unable to connect to existing Azure Cosmos DB container or database.");
    }

    /// <summary>
    /// Creates a new chat session.
    /// </summary>
    /// <param name="session">Chat session to create.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Newly created chat session item.</returns>
    public async Task<Session> InsertSessionAsync(Session session, string correlationToken)
    {
        try
        {
            PartitionKey partitionKey = new(session.SessionId);

            return await _container.CreateItemAsync(
                item: session,
                partitionKey: partitionKey
            );
        }
        catch (CosmosException ex)
        {
            var errorMessage = $"Exception throw fetching all InsertSessionAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";

            // ** Avoid string interpolation (the '$' thing) when logging for performance lag. Instead, use standard 'Composite Formatting'
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Inerts new chat message.
    /// </summary>
    /// <param name="currentSession">Target chat session.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Updated Session with newly created chat message.</returns>
    public async Task<Session> InsertMessageAsync(Session currentSession, string correlationToken)
    {
        try
        {
            //PartitionKey partitionKey = new(message.SessionId);
            //Message newMessage = message with { TimeStamp = DateTime.UtcNow };
            //return await _container.CreateItemAsync<Message>(item: message, partitionKey: partitionKey);

            //To update a session object in Cosmos DB, you can use the ReplaceItemAsync method
            //provided by the Cosmos DB SDK.This method replaces the entire document specified
            //by the ID and partition key with a new document.


            var partitionKey = new PartitionKey(currentSession.SessionId);
            var returnObject = await _container.ReplaceItemAsync(currentSession, currentSession.SessionId, partitionKey: partitionKey);
            return returnObject;

            //var partitionKey = new PartitionKey(currentSession.SessionId);
            //var returnObject = await _container.ReplaceItemAsync<Session>(currentSession, currentSession.SessionId, partitionKey: partitionKey);
            //return returnObject;


            //var sessionContainer = _cosmosClient.GetContainer(databaseId, containerId);
            //var sessionToUpdate = await GetChatSessionByIdAsync(message.SessionId, corrleationToken);
            //sessionToUpdate.Messages.Add(message);


            // In the above example, session is the updated Session object that you wish to persist to the database.
            // The Id property identifies the document that you wish to replace.The partitionKey specifies the
            // partition key of the container.

            //This method requires the partition key for the container to be specified.Make sure that you have a
            //partition key set on your container to utilize this feature.

            //var partitionKey = new PartitionKey(session.Id);
            //await _container.ReplaceItemAsync(session, session.Id, partitionKey: partitionKey);

        }
        catch (CosmosException ex)
        {
            var errorMessage = $"Exception throw fetching all InsertMessageAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Inerts new chat message.
    /// </summary>
    /// <param name="returnMessages">Flag to also return all associated chat messages.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>List of distinct chat session items.</returns>
    public async Task<List<Session>> GetChatSessionsAsync(bool returnMessages, string correlationToken)
    {
        try
        {
            QueryDefinition query;

            if (returnMessages)
            {
                query = new QueryDefinition("SELECT DISTINCT * FROM c WHERE c.type = @type")
                .WithParameter("@type", nameof(Session));
            }
            else
            {
                query = new QueryDefinition("SELECT DISTINCT c.id, c.type, c.sessionId, c.tokensUsed, c.name FROM c WHERE c.type = @type")
                .WithParameter("@type", nameof(Session));
            }

            FeedIterator<Session> response = _container.GetItemQueryIterator<Session>(query);

            List<Session> output = new();
            while (response.HasMoreResults)
            {
                FeedResponse<Session> results = await response.ReadNextAsync();
                output.AddRange(results);
            }
            return output;
        }
        catch (CosmosException ex)
        //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorMessage = $"Exception throw fetching all GetChatSessionsAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Gets single session object
    /// </summary>
    /// <returns>List of distinct chat session items.</returns>
    /// 


    /// <summary>
    /// Gets single session object
    /// </summary>
    /// <param name="sessionId">Id of target session.</param>
    /// <param name="returnMessages">Flag to also return all associated chat messages.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>List of distinct chat session items.</returns>
    public async Task<Session> GetChatSessionByIdAsync(string sessionId, bool returnMessages, string correlationToken)
    {
        try
        {
            QueryDefinition query;

            if (!returnMessages)
            {
                query = new QueryDefinition("SELECT c.id, c.type, c.sessionId, c.tokensUsed, c.name FROM c WHERE c.sessionId = @sessionId")
                 .WithParameter("@sessionId", sessionId);

                FeedIterator<Session> iterator = _container.GetItemQueryIterator<Session>(query);

                FeedResponse<Session> response = await iterator.ReadNextAsync();

                return response.FirstOrDefault();
            }
            else
            {
                query = new QueryDefinition("SELECT * FROM c WHERE c.sessionId = @sessionId")
                 .WithParameter("@sessionId", sessionId);

                FeedIterator<Session> iterator = _container.GetItemQueryIterator<Session>(query);

                FeedResponse<Session> response = await iterator.ReadNextAsync();

                return response.FirstOrDefault();
            }



            //QueryDefinition query = new QueryDefinition("SELECT c.id, c.type, c.sessionId, c.tokensUsed, c.name FROM c WHERE c.sessionId = @sessionId")
            //.WithParameter("@sessionId", sessionId);

            //FeedIterator<dynamic> iterator = _container.GetItemQueryIterator<dynamic>(query);

            //List<Session> results = new List<Session>();
            //while (iterator.HasMoreResults)
            //{
            //    FeedResponse<Session> response = await iterator.ReadNextAsync();

            //    foreach (Session session in response)
            //    {
            //        results.Add(session);
            //    }
            //}

            //return results.FirstOrDefault();




            //QueryDefinition query;

            //if (returnMessages)
            //{
            //    query = new QueryDefinition("SELECT DISTINCT * FROM c WHERE c.sessionId = @sessionId")
            //   .WithParameter("@sessionId", sessionId);

            //    //query = new QueryDefinition("SELECT DISTINCT * FROM c WHERE c.sessionId = @sessionId AND c.type = @type")
            //    //.WithParameter("@sessionId", sessionId)
            //    //.WithParameter("@type", nameof(Session));
            //}
            //else
            //{
            //    query = new QueryDefinition("SELECT DISTINCT c.id, c.type, c.sessionId, c.tokensUsed, c.name FROM c WHERE c.sessionId = @sessionId")
            //    .WithParameter("@sessionId", sessionId);
            //}

            //FeedIterator<Session> iterator = _container.GetItemQueryIterator<Session>(query);

            //if (iterator.HasMoreResults)
            //{
            //    var response = await iterator.ReadNextAsync();
            //    return response.FirstOrDefault();
            //}

            //return null;
        }
        catch (CosmosException ex)
        //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorMessage = $"Exception throw fetching all GetChatSessionByIdAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }


    /// <summary>
    /// Gets a list of all current chat messages for a specified session identifier.
    /// </summary>
    /// <param name="sessionId">Id of target session.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>List of chat message items for the specified session.</returns>
    public async Task<List<Message>> GetSessionMessagesAsync(string sessionId, string correlationToken)
    {
        try
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.sessionId = @sessionId AND c.type = @type")
                .WithParameter("@sessionId", sessionId)
                .WithParameter("@type", nameof(Message));

            FeedIterator<Message> results = _container.GetItemQueryIterator<Message>(query);

            List<Message> output = new();
            while (results.HasMoreResults)
            {
                FeedResponse<Message> response = await results.ReadNextAsync();
                output.AddRange(response);
            }
            return output;
        }
        catch (CosmosException ex)
        //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorMessage = $"Exception throw fetching all GetSessionMessagesAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Updates existing chat session.
    /// </summary>
    /// <param name="session">Target chat session to update.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Updated chat session object.</returns>
    public async Task<Session> UpdateSessionAsync(Session session, string correlationToken)
    {
        try
        {
            PartitionKey partitionKey = new(session.SessionId);
            return await _container.ReplaceItemAsync(
                item: session,
                id: session.Id,
                partitionKey: partitionKey
            );
        }
        catch (CosmosException ex)
        //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorMessage = $"Exception throw fetching all UpdateSessionAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }

    /// <summary>
    /// Deletes target chat session and associated messages
    /// </summary>
    /// <param name="sessionId">Target chat session to destory.</param>
    /// <param name="correlationToken">Unique tracking value for the request</param>
    /// <returns>Void</returns>

    public async Task DeleteSessionAndMessagesAsync(string sessionId, string correlationToken)
    {
        try
        {
            PartitionKey partitionKey = new(sessionId);

            QueryDefinition query = new QueryDefinition("SELECT VALUE c.id FROM c WHERE c.sessionId = @sessionId")
                    .WithParameter("@sessionId", sessionId);

            FeedIterator<string> response = _container.GetItemQueryIterator<string>(query);

            TransactionalBatch batch = _container.CreateTransactionalBatch(partitionKey);
            while (response.HasMoreResults)
            {
                FeedResponse<string> results = await response.ReadNextAsync();
                foreach (var itemId in results)
                {
                    batch.DeleteItem(
                        id: itemId
                    );
                }
            }
            await batch.ExecuteAsync();
        }
        catch (CosmosException ex)
        //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var errorMessage = $"Exception throw fetching all DeleteSessionAndMessagesAsync() in CosmosDbService with CorrelationToken {correlationToken}: {ex.Message}";
            _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
            throw;
        }
    }
}


///// <summary>
///// Inerts new chat message.
///// </summary>
///// <param name="message">Chat message item to create.</param>
///// <returns>Newly created chat message item.</returns>
//public async Task<Message> InsertMessageAsync(Message message, string corrleationToken)
//{
//    try
//    {
//        PartitionKey partitionKey = new(message.SessionId);
//        Message newMessage = message with { TimeStamp = DateTime.UtcNow };
//        return await _container.CreateItemAsync<Message>(item: message, partitionKey: partitionKey);
//    }
//    catch (CosmosException ex)
//    {
//        var errorMessage = $"Exception throw fetching all InsertMessageAsync() in CosmosDbService with CorrelationToken {corrleationToken}: {ex.Message}";
//        _logger.LogError($"Error creating new chat session: {errorMessage}");
//        throw;
//    }
//}


///// <summary>
///// Batch create or update chat messages and session.
///// </summary>
///// <param name="session">Target chat session to update.</param>
///// <param name="correlationToken">Unique tracking value for the request</param>
///// <returns>Updated chat session object.</returns>
//public async Task UpsertSessionBatchAsync(string corrleationToken, params dynamic[] messages)
//{
//    try
//    {
//        if (messages.Select(m => m.SessionId).Distinct().Count() > 1)
//        {
//            throw new ArgumentException("All items must have the same partition key.");
//        }

//        PartitionKey partitionKey = new(messages.First().SessionId);
//        TransactionalBatch batch = _container.CreateTransactionalBatch(partitionKey);
//        foreach (var message in messages)
//        {
//            batch.UpsertItem(
//                item: message
//            );
//        }
//        await batch.ExecuteAsync();
//    }
//    catch (CosmosException ex)
//    //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
//    {
//        var errorMessage = $"Exception throw fetching all UpsertSessionBatchAsync() in CosmosDbService with CorrelationToken {corrleationToken}: {ex.Message}";
//        _logger.LogError("Error creating new chat session: {errorMessage}", errorMessage);
//        throw;
//    }
//}