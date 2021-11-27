EnsureDirectoryExists("./tools");
EnsureDirectoryExists("./tools/Addins");
EnsureDirectoryExists("./tools/Modules");
EnsureDirectoryExists("/src");

public class BootStapperFile
{
    public FilePath Path { get; set;}
    public string Content { get; set;}
}

var bootstrapperPath = IsRunningOnWindows()
                        ? new BootStapperFile {
                                Path = MakeAbsolute(File("./cake.cmd")),
                                Content = "@dotnet-cake %*"
                            }
                        : new BootStapperFile {
                            Path = MakeAbsolute(File("./cake")),
                            Content = "#!/bin/sh\nexec dotnet-cake \"$@\""
                        };

Information("Creating bootstrapper {0}...", bootstrapperPath.Path);

System.IO.File.WriteAllText(
    bootstrapperPath.Path.FullPath,
    bootstrapperPath.Content,
    System.Text.Encoding.ASCII
);


if (!IsRunningOnWindows())
{
    Information("Setting execute bit on bootstrapper {0}...", bootstrapperPath.Path);
    StartProcess(
        "/bin/sh",
        $"-c \"chmod 755 {bootstrapperPath.Path.FullPath}\""
    );
}

Information(
    "Successfully installed Cake {0} ({1})",
    typeof(ICakeContext).Assembly.GetName().Version.ToString(),
    Argument<string>("containerVersion")
    );