#load "nuget:Dotnet.Build, 0.3.9"

using System.Runtime.CompilerServices;
using static FileUtils;

var Owner = "seesharper";
var ProjectName = "DbReader";

var RootFolder = FileUtils.GetScriptFolder();

var ArtifactsFolder = CreateDirectory(RootFolder, "Artifacts");

var GitHubArtifactsFolder = CreateDirectory(ArtifactsFolder, "GitHub");

var NuGetArtifactsFolder = CreateDirectory(ArtifactsFolder, "NuGet");

var PathToReleaseNotes = Path.Combine(GitHubArtifactsFolder, "ReleaseNotes.md");

var PathToTestProjectFolder = Path.Combine(RootFolder, "..", "src", $"{ProjectName}.Tests");

var PathToProjectFolder = Path.Combine(RootFolder, "..", "src", ProjectName);

var CodeCoverageArtifactsFolder = CreateDirectory(ArtifactsFolder, "CodeCoverage");