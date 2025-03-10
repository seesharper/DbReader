#load "nuget:Dotnet.Build, 0.24.0"
#load "nuget:dotnet-steps, 0.0.2"

BuildContext.CodeCoverageThreshold = 87;

[StepDescription("Runs the tests with test coverage")]
Step testcoverage = () => DotNet.TestWithCodeCoverage();

[StepDescription("Runs all the tests for all target frameworks")]
Step test = () => DotNet.Test();

[StepDescription("Creates the NuGet packages")]
Step pack = async () =>
{
    // test();
    // testcoverage();
    await buildTrackingPackage();
    DotNet.Pack();
};

[DefaultStep]
[StepDescription("Deploys packages if we are on a tag commit in a secure environment.")]
AsyncStep deploy = async () =>
{
    pack();
    await Artifacts.Deploy();
};

AsyncStep buildTrackingPackage = async () =>
{
    /*
    dotnet pack /p:NuspecFile=DbReader.Tracking.nuspec /p:IsPackable=true -o ../../build/Artifacts/NuGet 
    */
    var workingDirectory = Path.Combine(BuildContext.SourceFolder, "DbReader.Tracking");
    await Command.ExecuteAsync("dotnet", $"pack /p:NuspecFile=DbReader.Tracking.nuspec /p:IsPackable=true -o ../../build/Artifacts/NuGet", workingDirectory);

};


await StepRunner.Execute(Args);
return 0;

