namespace DbReader.Construction
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    /// <summary>
    /// A class that is capable of creating a method that reads the fields from an <see cref="IDataRecord"/>
    /// that maps to the key properties of a given <see cref="Type"/>.
    /// </summary>
    public class CachedKeyReaderMethodBuilder : IKeyReaderMethodBuilder
    {
        private readonly IKeyReaderMethodBuilder keyReaderMethodBuilder;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedKeyReaderMethodBuilder"/> class.
        /// </summary>
        /// <param name="keyReaderMethodBuilder">The <see cref="IKeyReaderMethodBuilder"/> that is responsible for dynamically creating a method 
        /// that is capable of reading key columns.</param>        
        public CachedKeyReaderMethodBuilder(IKeyReaderMethodBuilder keyReaderMethodBuilder)
        {
            this.keyReaderMethodBuilder = keyReaderMethodBuilder;            
        }

        /// <summary>
        /// Creates a method that reads the key fields from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to read the key fields.</param>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The field prefix used to identify the key fields.</param>
        /// <returns>A method that reads the key fields from the given <paramref name="dataRecord"/>.</returns>
        public Func<IDataRecord, IStructuralEquatable> CreateMethod(Type type, IDataRecord dataRecord, string prefix)
        {
            return StaticCache<Func<IDataRecord, IStructuralEquatable>>.GetOrAdd(type, prefix,
                () => keyReaderMethodBuilder.CreateMethod(type, dataRecord, prefix));               
        }
    }
}