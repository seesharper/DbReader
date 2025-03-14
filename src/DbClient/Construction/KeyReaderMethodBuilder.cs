namespace DbClient.Construction
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Linq;
    using Extensions;
    using Interfaces;
    using Mapping;

    /// <summary>
    /// A class that is capable of creating a method that reads the fields from an <see cref="IDataRecord"/>
    /// that maps to the key properties of a given <see cref="Type"/>.
    /// </summary>    
    public class KeyReaderMethodBuilder : IKeyReaderMethodBuilder
    {
        private readonly Func<Type, IReaderMethodBuilder<IStructuralEquatable>> constructorReaderMethodBuilderFactory;

        private readonly IKeyPropertyMapper keyPropertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReaderMethodBuilder"/> class.
        /// </summary>
        /// <param name="constructorReaderMethodBuilderFactory">The function used to create an <see cref="IReaderMethodBuilder{T}"/>
        /// that is responsible for building a method that reads key fields from a given <see cref="IDataRecord"/>.</param>
        /// <param name="keyPropertyMapper">The <see cref="IPropertyMapper"/> that is responsible for mapping key properties to key fields.</param>
        public KeyReaderMethodBuilder(
            Func<Type, IReaderMethodBuilder<IStructuralEquatable>> constructorReaderMethodBuilderFactory, 
            IKeyPropertyMapper keyPropertyMapper)
        {
            this.constructorReaderMethodBuilderFactory = constructorReaderMethodBuilderFactory;
            this.keyPropertyMapper = keyPropertyMapper;
        }

        /// <summary>
        /// Creates a method that reads the key fields from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="type">The type for which to create the key reader method.</param>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The field prefix used to identify the key fields.</param>
        /// <returns>A method that reads the key fields from the given <paramref name="dataRecord"/>.</returns>
        public Func<IDataRecord, IStructuralEquatable> CreateMethod(Type type, IDataRecord dataRecord, string prefix)
        {
            MappingInfo[] keyProperties = keyPropertyMapper.Execute(type, dataRecord, prefix);
            Type[] keyTypes = keyProperties.Select(pm => pm.Property.PropertyType).ToArray();
            int[] ordinals = keyProperties.Select(pm => pm.ColumnInfo.Ordinal).ToArray();
            Type tupleType = keyTypes.ToTupleType();
            IReaderMethodBuilder<IStructuralEquatable> methodBuilder = constructorReaderMethodBuilderFactory(tupleType);
            var method = methodBuilder.CreateMethod();
            return record => method(record, ordinals);
        }
    }
}