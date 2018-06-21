FilePath        dockerPath          = Context.Tools.Resolve("docker");

Action<FilePath, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> Cmd = (path, args) => {
    var result = StartProcess(
        path,
        new ProcessSettings {
            Arguments = args(new ProcessArgumentBuilder())
        });

    if(0 != result)
    {
        throw new Exception($"Failed to execute tool {path.GetFilename()} ({result})");
    }
};

Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> Docker = (command, args) => {
    Cmd(dockerPath, pab => args(pab.Append(command)));
};

public static void Pull (this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker, string image)
{
    if (string.IsNullOrWhiteSpace(image))
    {
        throw new ArgumentNullException(nameof(image));
    }
    
    docker("pull", args => args.AppendQuoted(image));
}

public static void Push (this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker, string image, string tag)
{
    if (string.IsNullOrWhiteSpace(image))
    {
        throw new ArgumentNullException(nameof(image));
    }

    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }
    
    docker("push", args => args.AppendQuoted($"{image}:{tag}"));
}



public static void Tag (
    this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker,
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
    
    docker("tag", args => args.AppendQuoted($"{sourceImage}:{sourceTag}").AppendQuoted($"{targetImage}:{targetTag}"));
}

public static void Run (
    this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker, 
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
    
    docker("run", args =>
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
    });
}

public static void Build(
    this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker,
    string image,
    string tag,
    DirectoryPath dockerDirectoryPath)
{
    if (string.IsNullOrWhiteSpace(image))
    {
        throw new ArgumentNullException(nameof(image));
    }

    if (string.IsNullOrWhiteSpace(tag))
    {
        throw new ArgumentNullException(nameof(tag));
    }

    if (dockerDirectoryPath == null)
    {
        throw new ArgumentNullException(nameof(dockerDirectoryPath));
    }

    docker("build", args => args
                        .Append("--no-cache")
                        .Append("--rm")
                        .Append("--quiet")
                        .Append("--squash")
                        .AppendSwitchQuoted("--tag","=", $"{image}:{tag}")
                        .AppendQuoted(dockerDirectoryPath.FullPath));
}