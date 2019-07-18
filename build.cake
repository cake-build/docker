#load "docker.cake"

string          target              = Argument("target", "Default");
DirectoryPath   baseCakePath        = MakeAbsolute(Directory("./"));
DirectoryPath   bitriseCakePath     = baseCakePath.Combine("Bitrise"),
                bitriseMonoCakePath = baseCakePath.Combine("BitriseMono"),
                monoCakePath        = baseCakePath.Combine("Mono"),
                windowsNanoPath     = baseCakePath.Combine("Windows").Combine("Nano"),
                outputPath          = MakeAbsolute(Directory("./output"));
FilePath        cakeVersionPath     = outputPath.CombineWithFilePath("cakeversion");
string          tagFilter           = Argument("tagfilter", "").ToLower();

var             images              =   new []{
                                            new { Path = baseCakePath,          Image = "cakebuild/cake", Tag = "2.1-sdk" , Windows = false },
                                            new { Path = bitriseCakePath,       Image = "cakebuild/cake", Tag = "2.1-sdk-bitrise", Windows = false },
                                            new { Path = bitriseMonoCakePath,   Image = "cakebuild/cake", Tag = "2.1-sdk-bitrise-mono", Windows = false },
                                            new { Path = monoCakePath,          Image = "cakebuild/cake", Tag = "2.1-sdk-mono", Windows = false },
                                            new { Path = windowsNanoPath,       Image = "cakebuild/cake", Tag = "2.1-sdk-nanoserver-1803", Windows = true },
                                        };
if (!string.IsNullOrWhiteSpace(tagFilter))
{
    images = images
                .Where(image=>tagFilter ==image.Tag)
                .ToArray();
}
string          cakeVersion         = null;

Task("Clean")
 .Does(()=>
{
    CleanDirectory(outputPath);
});

Task("Pull-Base-Image")
 .IsDependentOn("Clean")
 .Does(()=>
{
    string baseTag;
    switch(tagFilter)
    {
        case "2.1-sdk-bitrise":
            baseTag = "cakebuild/cake:2.1-sdk";
            break;

        case "2.1-sdk-bitrise-mono":
            baseTag = "cakebuild/cake:2.1-sdk-bitrise";
            break;

        case "2.1-sdk-mono":
            baseTag = "cakebuild/cake:2.1-sdk";
            break;

        case "2.1-sdk-nanoserver-1803":
            baseTag = "microsoft/dotnet:2.1-sdk-nanoserver-1803";
            break;

        default:
            baseTag = "microsoft/dotnet:2.1-sdk";
            break;
    }
    Docker.Pull(baseTag);
});

Task("Build-Images")
    .IsDependentOn("Pull-Base-Image")
    .DoesForEach(
        images,
        image =>
{
    Information("Building: {0}", image);
    Docker.Build(image.Image, image.Tag, image.Path);
});

Task("Test-Images")
    .IsDependentOn("Build-Images")
    .DoesForEach(
        images,
        image =>
{
    Information("Testing: {0}", image);
    var testResult = image.Windows
        ? Docker.Run(
                true,
                image.Image,
                image.Tag,
                null,
                "C:\\WINDOWS\\system32\\cmd.exe",
                "/c",
                "\"cake --version\""
            )
        : Docker.Run(
                true,
                image.Image,
                image.Tag,
                null,
                "/bin/bash",
                "-c",
                "\"cat /cake/cakeversion;cake --version\""
            );
    Information(testResult);
    using (var file =Context.FileSystem
                        .GetFile(cakeVersionPath)
                        .OpenWrite())
    {
        using(var writer = new StreamWriter(file, Encoding.UTF8))
        {
            writer.WriteLine(testResult);
        }
    }
});

Task("Fetch-Version")
    .IsDependentOn("Test-Images")
 .Does(()=>
{
    cakeVersion = Context.FileSystem
                    .GetFile(cakeVersionPath)
                    .ReadLines(Encoding.UTF8)
                    .FirstOrDefault()
                    ?.Trim();

    if (string.IsNullOrWhiteSpace(cakeVersion))
    {
        throw new Exception($"Failed to fetch version from {cakeVersionPath}");
    }

    Information("Version built: {0}", cakeVersion);
});


Task("Version-Tag-Images")
    .IsDependentOn("Fetch-Version")
    .DoesForEach(
        images,
        image =>
{
    var targetTag = $"{cakeVersion}-{image.Tag}";

    Information("Tagging: {0}", targetTag);

    Docker.Tag(
        image.Image,
        image.Tag,
        image.Image,
        targetTag
    );
});

Task("Push-Images")
    .IsDependentOn("Version-Tag-Images")
    .DoesForEach(
        images,
        image =>
{
    Information("Pushing: {0}", image);
    Docker.Push(image.Image, $"{cakeVersion}-{image.Tag}");
    Docker.Push(image.Image, image.Tag);
});

Task("Default")
    .IsDependentOn("Push-Images");

RunTarget(target);