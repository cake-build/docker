#addin "nuget:?package=System.Text.Json&version=5.0.0&loaddependencies=true"
using System.Text.Json.Serialization;

public record Repository(string Name, string CakePrefix)
{
    public string DockerRepoTagUri { get; } = $"https://mcr.microsoft.com/v2/{Name}/tags/list";
}

public record RepositoryTags(string Name, string[] Tags)
{
}

public record BaseImage(string Repo, string Tag, string CakePrefix)
{
    public string Image { get; } = $"mcr.microsoft.com/{(Repo.Trim('/'))}:{Tag}";
    public string CakeImage { get; } = $"cakebuild/cake:{CakePrefix}{Tag}";
}