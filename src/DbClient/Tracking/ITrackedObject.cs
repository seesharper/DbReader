namespace DbClient.Tracking;

using System.Collections.Generic;

/// <summary>
/// This interface is implemented by classes that are tracked for changes.
/// </summary>
public interface ITrackedObject
{
    /// <summary>
    /// Gets a list of the properties that have been modified.
    /// </summary>
    /// <returns></returns>
    HashSet<string> GetModifiedProperties();
}