namespace DbReader.Tracking.SampleAssembly;

[Tracked]
public record PositionalRecord(int Id, string Name);

[AttributeUsage(AttributeTargets.Class)]
public class TrackedAttribute : Attribute
{
}
