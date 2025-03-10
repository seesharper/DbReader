namespace DbReader.Tracking;

public class WeaveAssembly : Microsoft.Build.Utilities.Task
{

    public string TargetAssemblyPath { get; set; } = string.Empty;

    private TrackingAssemblyWeaver weaver = new TrackingAssemblyWeaver();

    public override bool Execute()
    {
        Log.LogMessage("Weaving assembly {0}", TargetAssemblyPath);
        weaver.Weave(TargetAssemblyPath, "TrackedAttribute");
        return true;
    }
}