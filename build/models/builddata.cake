#load "docker.cake"
using System.Net.Http;

public record BuildData(
    HttpClient Client,
    ICollection<Repository> DockerRepos,
    string NuGetSource,
    bool RemoveBaseImage,
    bool ShouldPublish,
    bool LinuxContainer,
    bool WindowsContainer,
    string BaseImageFilter
    )
{
    public List<BaseImage> BaseImages { get; } = new();
    public List<string> CakeVersions { get; } = new();
}