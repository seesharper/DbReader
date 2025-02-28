using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using DbReader.Tracking;

using Shouldly;
using Xunit;

namespace DbReader.Tests;


public class AssemblyWeaverTests
{
    // [Fact]
    // public void ShouldImplementITrackedObject()
    // {
    //     typeof(PositionalRecord).GetInterfaces().ShouldContain(typeof(ITrackedObject));
    // }

    [Fact]
    public async Task ShouldWeaveAssembly()
    {
        //await BuildSampleAssembly();

        var pathToThisAssembly = Path.GetDirectoryName(typeof(AssemblyWeaverTests).Assembly.Location);
        var assemblyPath = Path.GetFullPath(Path.Combine(pathToThisAssembly, "..", "..", "..", "..", "DbReader.Tracking.SampleAssembly", "bin", "Debug", "net8.0", "DbReader.Tracking.SampleAssembly.dll"));

        var weaver = new TrackingAssemblyWeaver();
        var path = "/Users/bernhardrichter/GitHub/DbReader/src/DbReader.Tracking.SampleAssembly/bin/Debug/net8.0/DbReader.Tracking.SampleAssembly.dll";

        TrackingAssemblyWeaver.Weave(path, "TrackedAttribute");

        List<string> list = null;
        test(ref list);
    }


    private void test(ref List<string> list)
    {
        if (list == null)
        {
            list = new List<string>();
        }
    }


    private async Task BuildSampleAssembly()
    {
        var pathToThisAssembly = Path.GetDirectoryName(typeof(AssemblyWeaverTests).Assembly.Location);
        var projectPath = Path.GetFullPath(Path.Combine(pathToThisAssembly, "..", "..", "..", "..", "DbReader.Tracking.SampleAssembly"));
        var projectFile = Path.Combine(projectPath, "DbReader.Tracking.SampleAssembly.csproj");

        var buildCommand = Cli.Wrap("dotnet")
            .WithArguments($"build {projectFile} --no-incremental -c Debug")
            .WithWorkingDirectory(projectPath);

        await buildCommand.ExecuteAsync();
    }


}