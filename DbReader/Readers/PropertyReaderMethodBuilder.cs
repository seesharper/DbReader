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
namespace DbReader.Readers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using DbReader.Interfaces;

    /// <summary>
    /// A class that is capable of creating a delegate that creates and populates an instance of <typeparamref name="T"/> from an 
    /// <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam> 
    public class PropertyReaderMethodBuilder<T> : ReaderMethodBuilder<T> 
    {       
        private readonly IPropertySelector simplePropertySelector;

        private readonly IPropertySelector oneToManyPropertySelector;

        private readonly IConstructorSelector constructorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for
        /// creating an <see cref="IMethodSkeleton"/>.</param>
        /// <param name="methodSelector">The <see cref="IMethodSelector"/> that is responsible for selecting the <see cref="IDataRecord"/> 
        /// get method that corresponds to the property type.</param>
        /// <param name="simplePropertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting the target properties.</param>
        /// <param name="oneToManyPropertySelector"></param>
        /// <param name="parameterlessConstructorSelector">The <see cref="IConstructorSelector"/> that is responsible for selecting the constructor.</param>
        public PropertyReaderMethodBuilder(
            IMethodSkeletonFactory methodSkeletonFactory,
            IMethodSelector methodSelector,
            IPropertySelector simplePropertySelector,
            IPropertySelector oneToManyPropertySelector,
            IConstructorSelector parameterlessConstructorSelector)
            : base(methodSkeletonFactory, methodSelector)
        {
            this.simplePropertySelector = simplePropertySelector;
            this.oneToManyPropertySelector = oneToManyPropertySelector;
            constructorSelector = parameterlessConstructorSelector;
        }

        /// <summary>
        /// Builds the methods required to create and populate an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="il">THe <see cref="ILGenerator"/> used to emit instructions.</param>
        protected override void BuildMethod(ILGenerator il)
        {
            var instanceVariable = EmitNewInstance(il, GetParameterlessConstructor());
            EmitPropertySetters(il, instanceVariable);
            InitializeEnumerableProperties(il, instanceVariable);
            LoadInstance(il, instanceVariable);
            EmitReturn(il);
        }

        private void InitializeEnumerableProperties(ILGenerator il, LocalBuilder instanceVariable)
        {
            var properties = oneToManyPropertySelector.Execute(typeof(T));
            foreach (var property in properties)
            {
                InitializeEnumerableProperty(il, instanceVariable, property);
            }

        }

        private void InitializeEnumerableProperty(ILGenerator il, LocalBuilder instanceVariable, PropertyInfo property)
        {
            if (property.PropertyType.IsEnumerable())
            {
                var projectionType = property.PropertyType.GetProjectionType();
                var collectionType = typeof(Collection<>).MakeGenericType(projectionType);
                var collectionConstructor = collectionType.GetConstructor(Type.EmptyTypes);
                var setMethod = property.GetSetMethod();
                LoadInstance(il, instanceVariable);
                il.Emit(OpCodes.Newobj, collectionConstructor);
                il.Emit(OpCodes.Callvirt, setMethod);
            }
        }

        private static void LoadInstance(ILGenerator il, LocalBuilder instanceVariable)
        {
            il.Emit(OpCodes.Ldloc, instanceVariable);
        }

        private static void EmitCallPropertySetterMethod(ILGenerator il, PropertyInfo property)
        {
            var setterMethod = property.GetSetMethod();
            il.Emit(OpCodes.Callvirt, setterMethod);
        }

        private static LocalBuilder EmitNewInstance(ILGenerator il, ConstructorInfo constructorInfo)
        {
            il.Emit(OpCodes.Newobj, constructorInfo);
            LocalBuilder instanceVariable = il.DeclareLocal(typeof(T));
            il.Emit(OpCodes.Stloc, instanceVariable);
            return instanceVariable;
        }

        private ConstructorInfo GetParameterlessConstructor()
        {
            return constructorSelector.Execute(typeof(T));
        }

        private void EmitPropertySetters(ILGenerator generator, LocalBuilder instanceVariable)
        {
            var properties = simplePropertySelector.Execute(typeof(T));
            for (int i = 0; i < properties.Length; i++)
            {
                EmitPropertySetter(generator, properties[i], i, instanceVariable);
            }
        }

        private void EmitPropertySetter(ILGenerator il, PropertyInfo property, int propertyIndex, LocalBuilder instanceVariable)
        {
            MethodInfo getMethod = MethodSelector.Execute(property.PropertyType);
            var endLabel = il.DefineLabel();
            EmitCheckForValidOrdinal(il, propertyIndex, endLabel);
            EmitCheckForDbNull(il, propertyIndex, endLabel);
            LoadInstance(il, instanceVariable);
            EmitGetValue(il, propertyIndex, getMethod, property.PropertyType);
            EmitCallPropertySetterMethod(il, property);
            il.MarkLabel(endLabel);
        }        
    }
}