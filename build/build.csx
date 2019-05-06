#load "BuildContext.csx"
#load "nuget:dotnet-steps, 0.0.1"
#load "nuget:Dotnet.Build, 0.3.9"
#load "nuget:github-changelog, 0.1.2"

using static ChangeLog;
using static ReleaseManagement;

Console.WriteLine(BuildEnvironment.IsSecure);

[StepDescription("Runs all tests")]
Step test = () =>
{
    DotNet.Test(PathToTestProjectFolder);
};

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () =>
{
    Command.Execute("dotnet", $"test -c release -f netcoreapp2.0  /property:CollectCoverage=true /property:Include=\"[DbReader*]*\" /property:Exclude=\"[*Tests*]*\" /property:CoverletOutputFormat=\\\"opencover,lcov,json\\\" /property:CoverletOutput={CodeCoverageArtifactsFolder}/coverage /property:Threshold=99", PathToTestProjectFolder);
    var pathToOpenCoverResult = Path.Combine(CodeCoverageArtifactsFolder, "coverage.opencover.xml");
    Command.Execute("dotnet", $"reportgenerator \"-reports:{pathToOpenCoverResult}\"  \"-targetdir:{CodeCoverageArtifactsFolder}/Report\" \"-reportTypes:XmlSummary;Xml;HtmlInline_AzurePipelines_Dark\" \"--verbosity:warning\"", PathToTestProjectFolder);
};

[StepDescription("Runs the tests with test coverage")]
Step pack = () =>
{
    DotNet.Pack(PathToProjectFolder, NuGetArtifactsFolder);
};

[DefaultStep]
[StepDescription("Builds and creates a release")]
Step release = () =>
{
    testcoverage();
    DotNet.Pack(PathToProjectFolder, NuGetArtifactsFolder);
};


await StepRunner.Execute(Args);
return;

private async Task PublishRelease()
{
    if (!BuildEnvironment.IsSecure)
    {
        Logger.Log("Pushing a new release can only be done in a secure build environment");
        return;
    }

    await CreateReleaseNotes();

    if (Git.Default.IsTagCommit())
    {
        Git.Default.RequreCleanWorkingTree();
        var releaseManager = ReleaseManagerFor(Owner, ProjectName, BuildEnvironment.GitHubAccessToken);
        await releaseManager.CreateRelease(Git.Default.GetLatestTag(), PathToReleaseNotes, Array.Empty<ReleaseAsset>());
        NuGet.Push(NuGetArtifactsFolder);
    }
}

private async Task CreateReleaseNotes()
{
    Logger.Log("Creating release notes");
    var generator = ChangeLogFrom(Owner, ProjectName, BuildEnvironment.GitHubAccessToken).SinceLatestTag();
    if (!Git.Default.IsTagCommit())
    {
        generator = generator.IncludeUnreleased();
    }
    await generator.Generate(PathToReleaseNotes);
}


