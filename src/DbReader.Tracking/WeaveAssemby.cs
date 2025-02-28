namespace DbReader.Tracking;

public class WeaveAssembly : Microsoft.Build.Utilities.Task
{

    public string TargetAssemblyPath { get; set; } = string.Empty;
    
    public override bool Execute()
    {
        TrackingAssemblyWeaver.Weave(TargetAssemblyPath, "TrackedAttribute");
        Log.LogMessage("Weaving assembly {0}", TargetAssemblyPath);
        return true;
    }
}