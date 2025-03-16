namespace DbClient.Tracking;

public class WeaveAssembly : Microsoft.Build.Utilities.Task
{
    public string TargetAssemblyPath { get; set; } = string.Empty;

    public string TrackingAttributeName { get; set; } = string.Empty;

    private readonly TrackingAssemblyWeaver weaver = new();

    public override bool Execute()
    {
        Log.LogMessage("Weaving assembly {0}", TargetAssemblyPath);
        weaver.Weave(TargetAssemblyPath, TrackingAttributeName);
        return true;
    }
}