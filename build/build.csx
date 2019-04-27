#! "netcoreapp2.0"
#load "BuildContext.csx"
#load "nuget:dotnet-steps, 0.0.1"
#load "nuget:Dotnet.Build, 0.3.9"
#load "nuget:github-changelog, 0.1.2"

using static ChangeLog;
using static ReleaseManagement;

var context = new BuildContext("seesharper", "DbReader");

Step test = () =>
{
    DotNet.Test(context.PathToTestProjectFolder);
};

await StepRunner.Execute(Args);
return;

DotNet.Build(context.PathToProjectFolder);
DotNet.Pack(context.PathToProjectFolder, context.NuGetArtifactsFolder);

if (BuildEnvironment.IsSecure)
{
    var generator = ChangeLogFrom(context.Owner, context.ProjectName, BuildEnvironment.GitHubAccessToken).SinceLatestTag();
    if (!Git.Default.IsTagCommit())
    {
        generator = generator.IncludeUnreleased();
    }
    await generator.Generate(context.PathToReleaseNotes);

    if (Git.Default.IsTagCommit())
    {
        Git.Default.RequreCleanWorkingTree();
        var releaseManager = ReleaseManagerFor(context.Owner, context.ProjectName, BuildEnvironment.GitHubAccessToken);
        await releaseManager.CreateRelease(Git.Default.GetLatestTag(), context.PathToReleaseNotes, Array.Empty<ReleaseAsset>());
        NuGet.Push(context.NuGetArtifactsFolder);
    }
}
