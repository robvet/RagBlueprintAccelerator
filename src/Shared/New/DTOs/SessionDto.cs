using Newtonsoft.Json;

namespace Shared.New.DTOs;

public record SessionDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; }

    public string Type { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    public string SessionId { get; set; }

    public int? TokensUsed { get; set; }

    public string Name { get; set; }

    public double Temperature { get; set; }

    public double NucluesSampling { get; set; }

    public double FrequencyPenalty { get; set; }

    public double PresencePenalty { get; set; }

    public string PrivatePrompt { get; set; }

    public int TokenMax { get; set; }

    [JsonIgnore]
    public List<MessageDto> Messages { get; set; }

    //public SessionDto()
    //{
    //    Id = Guid.NewGuid().ToString();
    //    Type = nameof(SessionDto);
    //    SessionId = Id;
    //    TokensUsed = 0;
    //    Name = "New Chat";
    //    Messages = new List<MessageDto>();
    //}

    //public void AddMessage(MessageDto message)
    //{
    //    Messages.Add(message);
    //}

    //public void UpdateMessage(MessageDto message)
    //{
    //    var match = Messages.Single(m => m.Id == message.Id);
    //    var index = Messages.IndexOf(match);
    //    Messages[index] = message;
    //}
}