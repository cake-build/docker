using System.Text.Json.Serialization;

public record NuGetIndex
{
    [JsonPropertyName("version")]
    public string Version { get; init; }

    [JsonPropertyName("resources")]
    public NuGetResource[] Resources { get; init; }
}

public record NuGetResource
{
    [JsonPropertyName("@id")]
    public string Id { get; init; }
    [JsonPropertyName("@type")]
    public string Type { get; init; }
}

public record NuGetCommit
{
    [JsonPropertyName("@id")]
    public string Id { get; init; }

    [JsonPropertyName("commitId")]
    public Guid CommitId { get; init; }

    [JsonPropertyName("commitTimeStamp")]
    public DateTimeOffset CommitTimeStamp { get; init; }
}

public record NuGetContainer<T> : NuGetCommit
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("items")]
    public T[] Items { get; init; }
}
public record NuGetPackageEntry: NuGetCommit
{
    [JsonPropertyName("catalogEntry")]
    public NuGetCatalogEntry CatalogEntry { get; init; }
}
public record NuGetCatalogEntry: NuGetResource
{
    [JsonPropertyName("version")]
    public string Version { get; init; }
}