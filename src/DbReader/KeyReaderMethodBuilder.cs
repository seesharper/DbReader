/*****************************************************************************   
    The MIT License (MIT)
    Copyright (c) 2014 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    DbReader version 1.0.0.1
    https://github.com/seesharper/DbReader
    http://twitter.com/bernhardrichter
******************************************************************************/
namespace DbReader
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Linq;

    using DbReader.Interfaces;

    /// <summary>
    /// A class that is capable of creating a method that reads the fields from an <see cref="IDataRecord"/>
    /// that maps to the key properties of a given <see cref="Type"/>.
    /// </summary>    
    public class KeyReaderMethodBuilder : IKeyReaderMethodBuilder
    {
        private readonly Func<Type, IReaderMethodBuilder<IStructuralEquatable>> constructorReaderMethodBuilderFactory;

        private readonly IPropertyMapper keyPropertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="constructorReaderMethodBuilderFactory">The function used to create an <see cref="IReaderMethodBuilder{T}"/>
        /// that is responsible for building a method that reads key fields from a given <see cref="IDataRecord"/>.</param>
        /// <param name="keyPropertyMapper">The <see cref="IPropertyMapper"/> that is responsible for mapping key properties to key fields.</param>
        public KeyReaderMethodBuilder(
            Func<Type, IReaderMethodBuilder<IStructuralEquatable>> constructorReaderMethodBuilderFactory, 
            IPropertyMapper keyPropertyMapper)
        {
            this.constructorReaderMethodBuilderFactory = constructorReaderMethodBuilderFactory;
            this.keyPropertyMapper = keyPropertyMapper;
        }

        /// <summary>
        /// Creates a method that reads the key fields from the given <paramref name="dataRecord"/>.
        /// </summary>        
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