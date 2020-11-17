public static bool IsDockerExperimental = false;

FilePath        dockerPath          =  Context.Tools.Resolve(IsRunningOnWindows() ? "docker.exe" : "docker")
                                        ?? Context.Tools.Resolve(IsRunningOnWindows() ? "docker" : "docker.exe")
                                        ?? throw new System.IO.FileNotFoundException("Docker tool couldn't be resolved.", IsRunningOnUnix() ? "docker" : "docker.exe");

var DockerToolTimeout = (int)TimeSpan.FromMinutes(8).TotalMilliseconds;

Func<FilePath, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> Cmd = (path, args, redirectStandardOutput) => {
    var result = StartProcess(
        path,
        new ProcessSettings {
            Arguments = args(new ProcessArgumentBuilder()),
            RedirectStandardOutput = redirectStandardOutput,
            Timeout  = DockerToolTimeout
        },
        out IEnumerable<string> redirectedStandardOutput);

    var output = string.Join(System.Environment.NewLine, redirectedStandardOutput ?? Enumerable.Empty<string>());

    if(0 != result)
    {
        throw new Exception($"Failed to execute tool {path.GetFilename()} ({result})");
    }

    return output;
};

Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> Docker =
    (command, args, redirectStandardOutput) =>
        Cmd(dockerPath, pab => args(pab.Append(command)), redirectStandardOutput);

// Disable experimental
//IsDockerExperimental = Docker("version", arg=>arg.Append("-f {{.Server.Experimental}}"), true) == "true";

public static void Pull (this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker, string image)
{
    if (string.IsNullOrWhiteSpace(image))
    {
        throw new ArgumentNullException(nameof(image));
    }

    docker(
        "pull",
        args => args
                    .Append("--quiet")
                    .AppendQuoted(image),
        false
        );
}

public static void Push (this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker, string tag)
{
    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }

    docker("push", args => args.AppendQuoted(tag), false);
}

public static void Tag (
    this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker,
    string sourceImage,
    string sourceTag,
    string targetImage,
    string targetTag)
{
    if (string.IsNullOrWhiteSpace(sourceImage))
    {
        throw new ArgumentNullException(nameof(sourceImage));
    }

    if (string.IsNullOrWhiteSpace(sourceTag))
    {
        throw new ArgumentNullException(nameof(sourceTag));
    }

    if (string.IsNullOrWhiteSpace(targetImage))
    {
        throw new ArgumentNullException(nameof(targetImage));
    }

    if (string.IsNullOrWhiteSpace(targetTag))
    {
        throw new ArgumentNullException(nameof(targetTag));
    }

    docker("tag", args => args.AppendQuoted($"{sourceImage}:{sourceTag}").AppendQuoted($"{targetImage}:{targetTag}"), false);
}

public static string Run (
    this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker,
    bool redirectStandardOutput,
    string image,
    string tag,
    KeyValuePair<DirectoryPath, DirectoryPath>? volume,
    params string[] commands)
{
    if (string.IsNullOrWhiteSpace(image))
    {
        throw new ArgumentNullException(nameof(image));
    }

    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }

    if (commands==null)
    {
        throw new ArgumentNullException(nameof(commands));
    }

    return docker("run", args =>
    {
        if (volume != null)
        {
            args.AppendSwitchQuoted("--volume", "=", $"{volume?.Key}:{volume?.Value}");
        }

        args.Append("--rm")
            .AppendQuoted($"{image}:{tag}");

        foreach(var command in commands)
        {
            args.Append(command);
        }

        return args;
    }, redirectStandardOutput);
}

public record BuildArg(string Name, string Value);

public static void Build(
    this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker,
    string tag,
    FilePath dockerFilePath,
    IEnumerable<BuildArg> buildArgs
    )
{
    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }

    if (buildArgs == null)
    {
        throw new ArgumentNullException(nameof(buildArgs));
    }

    Func<ProcessArgumentBuilder, ProcessArgumentBuilder> experimentalArgs = IsDockerExperimental
                                                                                ? args=> args.Append("--squash")
                                                                                : new Func<ProcessArgumentBuilder, ProcessArgumentBuilder>(args=> args);



    docker("build", args => experimentalArgs(args)
                        .Append("--no-cache")
                        .Append("--rm")
                        .Append("--quiet")
                        .AppendDockerBuildArgs(buildArgs)
                        .AppendSwitchQuoted("--tag","=", tag)
                        .AppendSwitchQuoted("--file","=", dockerFilePath.FullPath)
                        .AppendQuoted(dockerFilePath.GetDirectory().FullPath),
                        false);
}

public static ProcessArgumentBuilder AppendDockerBuildArgs(this ProcessArgumentBuilder args, IEnumerable<BuildArg> buildArgs)
{
    foreach(var buildArg in buildArgs)
    {
        args.Append("--build-arg");
        args.AppendQuoted($"{buildArg.Name}={buildArg.Value}");
    }
    return args;
}

public static void ImageRemove(
    this Func<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>, bool, string> docker,
    string tag
    )
{
    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }

    docker("image", args => args
                        .AppendSwitchQuoted("remove"," ", tag)
                        .Append("--force"),
                        false);
}