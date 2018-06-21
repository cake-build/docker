#load "docker.cake"

string          target              = Argument("target", "Default");
DirectoryPath   baseCakePath        = MakeAbsolute(Directory("./"));
DirectoryPath   bitriseCakePath     = baseCakePath.Combine("Bitrise"),
                bitriseMonoCakePath = baseCakePath.Combine("BitriseMono"),
                outputPath          = MakeAbsolute(Directory("./output"));
FilePath        cakeVersionPath     = outputPath.CombineWithFilePath("cakeversion");

var             images              =   new []{
                                            new { Path = baseCakePath,          Image = "cakebuild/cake", Tag = "2.1-sdk" },
                                            new { Path = bitriseCakePath,       Image = "cakebuild/cake", Tag = "2.1-sdk-bitrise" },
                                            new { Path = bitriseMonoCakePath,   Image = "cakebuild/cake", Tag = "2.1-sdk-bitrise-mono" },
                                        };
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
    Docker.Pull("microsoft/dotnet:2.1-sdk");
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
    Docker.Run(
        image.Image,
        image.Tag,
        new KeyValuePair<DirectoryPath,DirectoryPath>(outputPath, "/output"),
        "/bin/bash",
        "-c",
        "\"cp /cake/cakeversion /output/;cake --version\""
        );
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