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

    public bool LinuxContainer { get; } = Tag switch {
                                            "3.1" => true,
                                            "5.0" => true,
                                            "6.0" => true,
                                            string =>   (
                                                            Tag.StartsWith("3.1-")
                                                            || Tag.StartsWith("5.0-")
                                                            || Tag.StartsWith("6.0-")
                                                        )
                                                        && (
                                                            !(
                                                                Tag.Contains("windows")
                                                                || Tag.Contains("nanoserver")
                                                            )
                                                        ),
                                            _ => false
                                        };
    public bool WindowsContainer { get; } =   Tag switch {
                                                string => (
                                                           Tag.StartsWith("3.1-")
                                                            || Tag.StartsWith("5.0-")
                                                            || Tag.StartsWith("6.0-")
                                                        )
                                                        && (
                                                            (
                                                                Tag.Contains("windows")
                                                                || Tag.Contains("nanoserver")
                                                            )
                                                        ),
                                                _ => false
                                            };
    public string Platform => (LinuxContainer, WindowsContainer) switch
                                {
                                    (true, false) => "linux",
                                    (false, true) => "windows",
                                    _ => null
                                };
}