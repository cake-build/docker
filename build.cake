#addin "nuget:?package=System.Text.Json&version=5.0.0&loaddependencies=true"
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
            new ("dotnet/sdk", "sdk-") //,
            //new ("dotnet/core/sdk", "sdk-")
        },
        "https://api.nuget.org/v3/index.json",
        StringComparer.OrdinalIgnoreCase.Equals(
            bool.TrueString,
            Argument("remove-base-image", bool.FalseString)
        ),
        StringComparer.OrdinalIgnoreCase.Equals(
            "Publish",
            setupContext.TargetTask.Name
        )
    )
);

Task("Get-Base-Image-Tags")
    .DoesForEach<BuildData, Repository>(
        (data, context) => data.DockerRepos,
        (data, repository, context) => {
            var repositoryTags = data
                                .Client
                                .GetAsync<RepositoryTags>(repository.DockerRepoTagUri)
                                .ConfigureAwait(false)
                                .GetAwaiter()
                                .GetResult();

            data.BaseImages.AddRange(
                from tag in repositoryTags.Tags
                where tag switch {
                                        "2.1" => context.IsRunningOnUnix(),
                                        "3.1" => context.IsRunningOnUnix(),
                                        "5.0" => context.IsRunningOnUnix(),
                                        string =>   (
                                                        tag.StartsWith("2.1-")
                                                        || tag.StartsWith("3.1-")
                                                        || tag.StartsWith("5.0-")
                                                    )
                                                    && (
                                                        (
                                                            tag.Contains("windows")
                                                            || tag.Contains("nanoserver")
                                                        ) == context.IsRunningOnWindows()
                                                    )
                                                    && !tag.Contains("-arm")

                                                    // Investigate fails on GitHub actions,
                                                    // move excluded images to command line parameter
                                                    && tag != "5.0-alpine3.11"

                                                    // Seems to be missing RTM .NET 5
                                                    && tag != "5.0-nanoserver-1903",
                                        _=> false
                    }
                select new BaseImage(
                    repository.Name,
                    tag,
                    repository.CakePrefix
                )
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
                    orderby version.CommitTimeStamp descending
                    select version.CatalogEntry.Version
                ).Take(5)
            );
            foreach(var version in data.CakeVersions)
            {
                context.Information(version);;
            }
        }
    );

Task("Docker-Build-BaseImages")
    .IsDependentOn("Get-Base-Image-Tags")
    .IsDependentOn("Get-Cake-Versions")
    .DoesForEach<BuildData, BaseImage>(
        (data, context) => data.BaseImages,
        (data, baseImage, context) => {
            ICollection<(string cakeVersion, string tag)> versionTags = data
                                                                            .CakeVersions
                                                                            .Select(
                                                                                cakeVersion =>
                                                                                (
                                                                                    cakeVersion,
                                                                                    string.Concat(
                                                                                            baseImage.CakeImage,
                                                                                            "-v",
                                                                                            cakeVersion
                                                                                        )
                                                                                )
                                                                            )
                                                                            .ToArray();

            context.Information("Pulling base image {0}...", baseImage.Image);
            Docker.Pull(baseImage.Image);
            context.Information("Pulled image {0}.", baseImage.Image);

            Parallel.ForEach(
                versionTags,
                cakeVersionTag => {
                    context.Information("Building image {0} based on {1}...", cakeVersionTag.tag, baseImage.Image);
                    Docker.Build(
                        cakeVersionTag.tag,
                        (
                            context.IsRunningOnWindows()
                                ? "./src/Windows.Dockerfile"
                                : "./src/Linux.Dockerfile"
                        ),
                        new BuildArg[] {
                            new("BASE_IMAGE", baseImage.Image),
                            new("CAKE_VERSION", cakeVersionTag.cakeVersion)
                        });
                    context.Information("Built image {0} based on {1}.", cakeVersionTag.tag, baseImage.Image);
                }
            );

            foreach (var (version, tag) in versionTags)
            {
                    context.Information("Testing image {0} based on {1}...", tag, baseImage.Image);
                    var builtVersion = Docker.Run(
                        true,
                        tag,
                        null,
                        "dotnet-cake",
                        "--version");

                    context.Information(
                        "Built version {0}, expected version {1}.",
                        builtVersion,
                        version);

                    if (version != builtVersion)
                    {
                        throw new Exception($"Built version {builtVersion}, expected version {version}");
                    }

                    context.Information("Tested image {0} based on {1}.", tag, baseImage.Image);

            }

            if (data.ShouldPublish)
            {
                Parallel.ForEach(
                    versionTags,
                    cakeVersionTag => {

                        context.Information("Publishing image {0}...", cakeVersionTag.tag);
                        Docker.Push(cakeVersionTag.tag);
                        context.Information("Published image {0}.", cakeVersionTag.tag);
                });
            }

            foreach(var (_, tag) in versionTags)
            {
                context.Information("Removing image {0}...", tag);
                Docker.ImageRemove(tag);
                context.Information("Removed image {0}.", tag);
            }

            if (data.RemoveBaseImage)
            {
                context.Information("Removing base image {0}...", baseImage.Image);
                Docker.ImageRemove(baseImage.Image);
                context.Information("Removed base image {0}.", baseImage.Image);
            }
        }
    );


Task("Default")
    .IsDependentOn("Docker-Build-BaseImages");

Task("Publish")
    .IsDependentOn("Docker-Build-BaseImages");

RunTarget(Argument("target", "Default"));