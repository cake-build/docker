
FilePath        dockerPath          = Context.Tools.Resolve("docker");
string          target              = Argument("target", "Default");
DirectoryPath   baseCakePath        = MakeAbsolute(Directory("./"));
DirectoryPath   bitriseCakePath     = baseCakePath.Combine("Bitrise"),
                bitriseMonoCakePath = baseCakePath.Combine("BitriseMono");

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

public static void Build(this Action<string, Func<ProcessArgumentBuilder, ProcessArgumentBuilder>> docker, DirectoryPath dockerDirectoryPath)
{
    if (dockerDirectoryPath == null)
    {
        throw new ArgumentNullException(nameof(dockerDirectoryPath));
    }

    docker("build", args => args
                        .Append("--no-cache")
                        .AppendQuoted(dockerDirectoryPath.FullPath));
}

Task("Pull-Base-Image")
 .Does(()=>
{
    Docker.Pull("microsoft/dotnet:2.1-sdk");
});

Task("Build-Cake-Base")
    .IsDependentOn("Pull-Base-Image")
    .Does(()=>
{
    Docker.Build(baseCakePath);
});

Task("Build-Cake-Bitrise")
    .IsDependentOn("Build-Cake-Base")
    .Does(()=>
{
    Docker.Build(bitriseCakePath);
});

Task("Build-Cake-Bitrise-Mono")
    .IsDependentOn("Build-Cake-Bitrise")
    .Does(()=>
{
    Docker.Build(bitriseMonoCakePath);
});



Task("Default")
    .IsDependentOn("Pull-Base-Image")
    .IsDependentOn("Build-Cake-Base")
    .IsDependentOn("Build-Cake-Bitrise")
    .IsDependentOn("Build-Cake-Bitrise-Mono");

RunTarget(target);