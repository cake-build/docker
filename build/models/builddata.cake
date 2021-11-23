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
    ICollection<string> BaseImageIncludeFilter,
    ICollection<string> BaseImageExcludeFilter,
    ICollection<string> IncompatibleVersions
    )
{
    public List<BaseImage> BaseImages { get; } = new();
    public List<SemVersion> CakeVersions { get; } = new();
    public List<string> PullFailedImages { get; } = new();
    public List<string> BuildFailedImages { get; } = new();
    public SemVersion LatestStableCakeVersion { get; set; } = SemVersion.Zero;
}