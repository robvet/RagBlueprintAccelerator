namespace Shared.New.Options;

public record CosmosDb
{
    public required string CosmosEndpoint { get; init; }

    public required string CosmosKey { get; init; }

    public required string CosmosDatabase { get; init; }

    public required string CosmosContainer { get; init; }
};