namespace DbReader.Readers
{
    using System;
    using System.Collections;
    using System.Data;
    using DbReader.Interfaces;

    /// <summary>
    /// A class that is capable of reading the key columns for a given <see cref="Type"/>.
    /// </summary>
    public class KeyReader : IKeyReader
    {
        private readonly IKeyReaderMethodBuilder keyReaderMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReader"/> class.
        /// </summary>
        /// <param name="keyReaderMethodBuilder">The <see cref="IKeyReaderMethodBuilder"/> that is responsible 
        /// for creating a method that can read the key columns for a given <see cref="Type"/>.</param>
        public KeyReader(IKeyReaderMethodBuilder keyReaderMethodBuilder)
        {
            this.keyReaderMethodBuilder = keyReaderMethodBuilder;
        }

        /// <summary>
        /// Reads the key columns from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to read the key columns.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the key columns.</param>
        /// <param name="prefix">The current column prefix.</param>
        /// <returns>An <see cref="IStructuralEquatable"/> that represent the key for an instance of the given <paramref name="type"/>.</returns>
        public IStructuralEquatable Read(Type type, IDataRecord dataRecord, string prefix)
        {
            return keyReaderMethodBuilder.CreateMethod(type, dataRecord, prefix)(dataRecord);
        }
    }
}