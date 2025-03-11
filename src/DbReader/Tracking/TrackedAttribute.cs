using System;

namespace DbReader.Tracking;

/// <summary>
/// Used to indicate that a class should be tracked.
/// Note that the DbReader.Tracking package is required to use this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TrackedAttribute : Attribute
{
}