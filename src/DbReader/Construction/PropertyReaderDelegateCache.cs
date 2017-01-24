namespace DbReader.Construction
{
    /// <summary>
    /// Caches a <see cref="PropertyReaderDelegate{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of object cached.</typeparam>
    public sealed class PropertyReaderDelegateCache<T> : Cache<string, PropertyReaderDelegate<T>>
    {
        
    }
}