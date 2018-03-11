var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var runtime = Argument("runtime", "linux-x64");


Task("Publish")
    .Does( ()=>
{
    DotNetCorePublish(
        "cake/src/Cake/Cake.csproj",
        new DotNetCorePublishSettings {
            Configuration = configuration,
            Framework = "netcoreapp2.0",
            Runtime = runtime,
            OutputDirectory = "/app",
            ArgumentCustomization = arg => arg
                .Append("--self-contained")
                .Append("--source=\"https://dotnet.myget.org/F/dotnet-core/api/v3/index.json;https://api.nuget.org/v3/index.json\"")
        }
        );
});

RunTarget(target);