#load "build/models/docker.cake"
#load "build/models/nuget.cake"
#load "build/models/builddata.cake"
#load "build/helpers/httpclient.cake"
#load "build/helpers/docker.cake"

if (BuildSystem.GitHubActions.IsRunningOnGitHubActions)
{
    TaskSetup(context=> System.Console.WriteLine($"::group::{context.Task.Name.Quote()}"));
    TaskTeardown(context=>System.Console.WriteLine("::endgroup::"));
}

Setup<BuildData>(
    setupContext => new(
        new HttpClient(),
        new Repository[] {
            new ("dotnet/sdk", "sdk-")
        },
        "https://api.nuget.org/v3/index.json",
        StringComparer.OrdinalIgnoreCase.Equals(
            bool.TrueString,
            Argument("remove-base-image", bool.FalseString)
        ),
        StringComparer.OrdinalIgnoreCase.Equals(
            "Publish",
            setupContext.TargetTask.Name
        ),
        DockerLinuxEngine,
        DockerWindowsEngine,
        new HashSet<string>(Arguments<string>("base-image-include-filter", Array.Empty<string>()), StringComparer.OrdinalIgnoreCase),
        new HashSet<string>(Arguments<string>("base-image-exclude-filter", Array.Empty<string>()), StringComparer.OrdinalIgnoreCase),
        IncompatibleVersions: new [] {
            "cakebuild/cake:sdk-6.0-nanoserver-1909-v1.3.0",
            "cakebuild/cake:sdk-6.0-nanoserver-1909-v2.0.0-rc0001"
        }
    )
);

Task("Get-Base-Image-Tags")
    .DoesForEach<BuildData, Repository>(
        (data, context) => data.DockerRepos,
        (data, repository, context) => {
            context.Information(
                "Fetching tags WindowsContainer: {0}, LinuxContainer: {1}.",
                data.WindowsContainer,
                data.LinuxContainer
            );
            var repositoryTags = data
                                .Client
                                .GetAsync<RepositoryTags>(repository.DockerRepoTagUri)
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();

            data.BaseImages.AddRange(
                from tag in repositoryTags.Tags
                where   // Exclude arm for now
                        !tag.Contains("-arm")

                        // Investigate fails on GitHub actions,
                        // move excluded images to command line parameter
                        && !tag.StartsWith("5.0-alpine3.11")
                        && !tag.StartsWith("6.0-alpine3.13")

                        // Seems to be missing RTM .NET 5
                        && tag != "5.0-nanoserver-1903"

                        // Exclude preview tags
                        && !tag.Contains("preview")

                        // No longer supported
                        && !tag.StartsWith("5.0.100-rc.")

                        // Argument BaseImageFilter
                        && (
                            !data.BaseImageIncludeFilter.Any()
                            ||
                            data.BaseImageIncludeFilter.Any(filter=>tag.StartsWith(filter))
                        )
                        && (
                            !data.BaseImageIncludeFilter.Any()
                            ||
                            !data.BaseImageExcludeFilter.Any(filter=>tag.StartsWith(filter))
                        )
                let baseImage =  new BaseImage(
                    repository.Name,
                    tag,
                    repository.CakePrefix
                )
                where   baseImage.WindowsContainer == data.WindowsContainer
                        && baseImage.LinuxContainer == data.LinuxContainer
                select baseImage
            );

            foreach(var baseImage in data.BaseImages)
            {
                context.Information("Image: {0}", baseImage.Image);
            }
        }
    );

Task("Get-Cake-Versions")
    .Does<BuildData>(
        async (context, data) => {
            var nuGetIndex = await data.Client.GetAsync<NuGetIndex>(data.NuGetSource);
            var cakeBaseUrl = string.Concat(
                                    nuGetIndex
                                        ?.Resources
                                        ?.Where(type => type.Type is { Length: 20 }
                                                        && type.Type == "RegistrationsBaseUrl"
                                                        && type.Id is { Length: > 8 }
                                                        && type.Id.StartsWith("https://"))
                                        .Select(url => url.Id)
                                        .FirstOrDefault()
                                        ?? throw new Exception($"Failed to fetch RegistrationsBaseUrl from {data.NuGetSource}."),
                                    "cake.tool/index.json"
                                );

            var cakeNuGetIndex = await data.Client.GetAsync<NuGetContainer<NuGetContainer<NuGetPackageEntry>>>(cakeBaseUrl);

            data.CakeVersions.AddRange(
                (
                    from item in cakeNuGetIndex.Items
                    from version in item.Items
                    where version.CatalogEntry.Version switch {
                        "1.0.0-rc0001" => false,
                        "1.0.0-rc0002" => false,
                        "1.0.0-rc0003" => false,
                        _ => true
                    }
                    let semVersion = SemVersion.TryParse(
                                version.CatalogEntry.Version,
                                out var semVersion
                            )
                                ? semVersion
                                : SemVersion.Zero
                    orderby semVersion descending
                    where semVersion.Major >= 1
                    select semVersion
                ).Take(10)
            );

            foreach(var version in data.CakeVersions)
            {
                if (data.LatestStableCakeVersion < version && !version.IsPreRelease)
                {
                    data.LatestStableCakeVersion = version;
                }
                context.Information("CakeVersion: {0}", version);
            }

                context.Information("LatestStableCakeVersion: {0}", data.LatestStableCakeVersion);
        }
    );

Task("Docker-Build-BaseImages")
    .IsDependentOn("Get-Base-Image-Tags")
    .IsDependentOn("Get-Cake-Versions")
    .DeferOnError()
    .DoesForEach<BuildData, BaseImage>(
        (data, context) => data.BaseImages,
        (data, baseImage, context) => {
            ICollection<(SemVersion cakeVersion, string tag)> versionTags = data
                                                                            .CakeVersions
                                                                            .Select(
                                                                                cakeVersion =>
                                                                                (
                                                                                    cakeVersion,
                                                                                    tag: string.Concat(
                                                                                            baseImage.CakeImage,
                                                                                            "-v",
                                                                                            cakeVersion.VersionString
                                                                                        )
                                                                                )
                                                                            )
                                                                            .Where(cakeVersionTag => !data. IncompatibleVersions.Contains(cakeVersionTag.tag))
                                                                            .ToArray();

            try
            {
                context.Information("Pulling base image {0}...", baseImage.Image);
                Docker.Pull(baseImage.Image);
                context.Information("Pulled image {0}.", baseImage.Image);
            }
            catch
            {
                data.PullFailedImages.Add(baseImage.Image);
                throw;
            }

            Parallel.ForEach(
                versionTags,
                cakeVersionTag => {
                    try
                    {
                        context.Information("Building image {0} based on {1}...", cakeVersionTag.tag, baseImage.Image);
                        Docker.Build(
                            cakeVersionTag.tag,
                            (
                                baseImage.WindowsContainer
                                    ? "./src/Windows.Dockerfile"
                                    : "./src/Linux.Dockerfile"
                            ),
                            new BuildArg[] {
                                new("BASE_IMAGE", baseImage.Image),
                                new("CAKE_VERSION", cakeVersionTag.cakeVersion.VersionString)
                            });
                        context.Information("Built image {0} based on {1}.", cakeVersionTag.tag, baseImage.Image);
                    }
                    catch
                    {
                        data.BuildFailedImages.Add(cakeVersionTag.tag);
                    }
                }
            );

            foreach (var (version, tag) in versionTags)
            {
                    context.Information("Testing image {0} based on {1}...", tag, baseImage.Image);
                    var builtVersion =  SemVersion.TryParse(
                                            Docker.Run(
                                                true,
                                                tag,
                                                null,
                                                "dotnet-cake",
                                                "--version"),
                                            out var parsedVersion)
                                            ? parsedVersion
                                            : SemVersion.Zero;

                    context.Information(
                        "Built version {0}, expected version {1}.",
                        builtVersion,
                        version);

                    if (version != builtVersion)
                    {
                        throw new Exception($"Built version {builtVersion}, expected version {version}");
                    }

                    context.Information("Tested image {0} based on {1}.", tag, baseImage.Image);

                    if (version == data.LatestStableCakeVersion)
                    {
                        context.Information("Version {0} is latest stable, tagging image {1} based on {2} ({3})...", version, baseImage.CakeImage, tag, baseImage.Image);
                        Docker.Tag(tag, baseImage.CakeImage);
                        context.Information("Version {0} is latest stable, tagged image {1} based on {2} ({3}).", version, baseImage.CakeImage, tag, baseImage.Image);
                    }
            }

            if (data.ShouldPublish)
            {
                Parallel.ForEach(
                    versionTags,
                    cakeVersionTags => {
                        var (version, tag) = cakeVersionTags;
                        context.Information("Publishing image {0}...", tag);
                        Docker.Push(tag);
                        context.Information("Published image {0}.", tag);

                        if (version == data.LatestStableCakeVersion)
                        {
                            context.Information("Version {0} is latest stable, pushing image {1} based on {2} ({3})...", version, baseImage.CakeImage, tag, baseImage.Image);
                            Docker.Push(baseImage.CakeImage);
                            context.Information("Version {0} is latest stable, pushed image {1} based on {2} ({3}).", version, baseImage.CakeImage, tag, baseImage.Image);
                        }
                });
            }

            foreach(var (version, tag) in versionTags)
            {
                context.Information("Removing image {0}...", tag);
                Docker.ImageRemove(tag);
                context.Information("Removed image {0}.", tag);

                if (version == data.LatestStableCakeVersion)
                {
                    context.Information("Version {0} is latest stable, removing image {1} based on {2} ({3})...", version, baseImage.CakeImage, tag, baseImage.Image);
                    Docker.ImageRemove(baseImage.CakeImage);
                    context.Information("Version {0} is latest stable, removed image {1} based on {2} ({3}).", version, baseImage.CakeImage, tag, baseImage.Image);
                }
            }

            if (data.RemoveBaseImage)
            {
                context.Information("Removing base image {0}...", baseImage.Image);
                Docker.ImageRemove(baseImage.Image);
                context.Information("Removed base image {0}.", baseImage.Image);
            }
        }
    );

Teardown<BuildData>((context, data) =>
{
    foreach(var failedPull in data.PullFailedImages)
    {
        Warning("PullFailedImage: {0}", failedPull);
    }
    foreach(var failedBuild in data.BuildFailedImages)
    {
        Warning("BuildFailedImage: {0}", failedBuild);
    }
});

Task("Default")
    .IsDependentOn("Docker-Build-BaseImages");

Task("Publish")
    .IsDependentOn("Docker-Build-BaseImages");

RunTarget(
    Argument(
        "target",
        (
            BuildSystem.GitHubActions.IsRunningOnGitHubActions
            &&
            !BuildSystem.GitHubActions.Environment.PullRequest.IsPullRequest
        )
            ? "Publish"
            : "Default"
        )
    );