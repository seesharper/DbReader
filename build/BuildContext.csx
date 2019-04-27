#load "nuget:Dotnet.Build, 0.3.9"

using System.Runtime.CompilerServices;
using static FileUtils;

public class BuildContext
{
    public BuildContext(string owner, string projectName)
    {
        Root = FileUtils.GetScriptFolder();
        var artifactsFolder = CreateDirectory(Root, "Artifacts");
        GitHubArtifactsFolder = CreateDirectory(artifactsFolder, "GitHub");
        NuGetArtifactsFolder = CreateDirectory(artifactsFolder, "NuGet");
        PathToReleaseNotes = Path.Combine(GitHubArtifactsFolder, "ReleaseNotes.md");
        PathToProjectFolder = Path.Combine(Root, "..", "src", projectName);
        PathToTestProjectFolder = Path.Combine(Root, "..", "src", $"{projectName}.Tests");
        Owner = owner;
        ProjectName = projectName;
    }

    public string GitHubArtifactsFolder { get; }

    public string NuGetArtifactsFolder { get; }

    public string Root { get; }

    public string PathToProjectFolder { get; }

    public string PathToTestProjectFolder { get; }

    public string PathToReleaseNotes { get; }
    public string Owner { get; }
    public string ProjectName { get; }
}