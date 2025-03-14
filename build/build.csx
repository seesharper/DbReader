#load "nuget:Dotnet.Build, 0.24.0"
#load "nuget:dotnet-steps, 0.0.2"

Console.WriteLine($"Building with the latest tag {BuildContext.LatestTag}");

BuildContext.CodeCoverageThreshold = 87;

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () => DotNet.TestWithCodeCoverage();

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () => DotNet.Test();

[StepDescription("Creates the NuGet packages")]
AsyncStep pack = async () =>
{
    test();
    testcoverage(); ;
    DotNet.Pack();
    await buildTrackingPackage();
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    await pack();
    await Artifacts.Deploy();
};

AsyncStep buildTrackingPackage = async () =>
{
    var workingDirectory = Path.Combine(BuildContext.SourceFolder, "DbClient.Tracking");
    await Command.ExecuteAsync("dotnet", $"pack /p:NuspecFile=DbClient.Tracking.nuspec /p:IsPackable=true /p:NuspecProperties=version={BuildContext.LatestTag} -o ../../build/Artifacts/NuGet", workingDirectory);

};


await StepRunner.Execute(Args);
return 0;

