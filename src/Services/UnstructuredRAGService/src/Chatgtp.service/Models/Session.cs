using Newtonsoft.Json;

namespace UnstructuredRAG.Service.Models;

public record Session
{
    public Session()
    {
        Id = Guid.NewGuid().ToString();
        Type = nameof(Session);
        SessionId = Id;
        TokensUsed = 0;
        Name = "New Chat";
        Messages = new List<Message>();
    }

    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; }

    public string Type { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    public string SessionId { get; set; }

    //public string ChatScreenTitle { get; set; }

    public int? TokensUsed { get; set; }

    public string Name { get; set; }

    public double Temperature { get; set; } = 0.3;

    public double NucluesSampling { get; set; } = 0.5;

    public double FrequencyPenalty { get; set; } = 0.0;

    public double PresencePenalty { get; set; } = 0.0;

    public string PrivatePrompt { get; set; } = "You are an AI assistant that helps people find information. Provide concise answers that are polite and professional.";

    public int TokenMax { get; set; } = 4000;

    //[JsonIgnore]
    public List<Message> Messages { get; set; }


    public void AddMessage(Message message)
    {
        Messages.Add(message);
    }

    public void UpdateMessage(Message message)
    {
        var match = Messages.Single(m => m.Id == message.Id);
        var index = Messages.IndexOf(match);
        Messages[index] = message;
    }
}