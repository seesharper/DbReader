using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DbReader.Extensions;
using DbReader.Selectors;

namespace DbReader.Construction
{
    /// <summary>
    /// Represents a class that is capable creating a dynamic method at runtime
    /// that converts the property values of a given object to a dictionary.
    /// </summary>
    public interface IObjectConverterMethodBuilder
    {
        /// <summary>
        /// Creates a dynamic method at runtime that converts
        /// the property values of a given object to a dictionary.
        /// </summary>
        /// <param name="targetType">The target object type.</param>
        /// <returns>A dynamic method at runtime that converts
        /// the property values of a given object to a dictionary.</returns>
        Func<object, Dictionary<string, object>> CreateMethod(Type targetType);
    }

    /// <summary>
    /// A class that creates a dynamic method at runtime
    /// that converts the property values of a given object to a dictionary.
    /// </summary>
    public class ObjectConverterMethodBuilder : IObjectConverterMethodBuilder
    {
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private readonly IPropertySelector readablePropertySelector;
        private static MethodInfo InvariantCultureIgnoreCaseGetMethod;
        private static ConstructorInfo DictionaryConstructorInfo;

        private static MethodInfo DictionaryAddMethodInfo;

        static ObjectConverterMethodBuilder()
        {
            InvariantCultureIgnoreCaseGetMethod = typeof(StringComparer).GetProperty(nameof(StringComparer.InvariantCultureIgnoreCase)).GetMethod;
            DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new[] { typeof(StringComparer) });
            DictionaryAddMethodInfo = typeof(Dictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConverterMethodBuilder"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for providing an <see cref="IMethodSkeleton"/> instance.</param>
        /// <param name="readablePropertySelector">The <see cref="IPropertySelector"/> that is responsible for providing a list of readable properties.</param>
        public ObjectConverterMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IPropertySelector readablePropertySelector)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
            this.readablePropertySelector = readablePropertySelector;
        }

        /// <inheritdoc/>
        public Func<object, Dictionary<string, object>> CreateMethod(Type targetType)
        {
            var properties = readablePropertySelector.Execute(targetType).OrderByDeclaration();

            var dynamicMethod = methodSkeletonFactory.GetMethodSkeleton("ToDictionary", typeof(Dictionary<string, object>), new[] { typeof(object) });
            var generator = dynamicMethod.GetGenerator();
            generator.DeclareLocal(targetType);

            generator.Emit(OpCodes.Call, InvariantCultureIgnoreCaseGetMethod);
            generator.Emit(OpCodes.Newobj, DictionaryConstructorInfo);

            // Load the target object onto the stack.
            generator.Emit(OpCodes.Ldarg_0);

            // Cast it to the target type.
            generator.Emit(OpCodes.Castclass, targetType);

            // Store the target object in the local variable.
            generator.Emit(OpCodes.Stloc_0);

            foreach (var property in properties)
            {
                // Duplicate the topmost value on the stack (Dictionary<string, object>)
                generator.Emit(OpCodes.Dup);

                // Push the property name which is the key (string).
                generator.Emit(OpCodes.Ldstr, property.Name);

                // Load the local variable onto the stack.
                generator.Emit(OpCodes.Ldloc_0);

                // Get the property value.
                generator.Emit(OpCodes.Callvirt, property.GetMethod);

                // If it is a value type, we need to box it.
                if (property.PropertyType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, property.PropertyType);
                }

                // Call the dictionary Add method.
                generator.Emit(OpCodes.Callvirt, DictionaryAddMethodInfo);
            }

            //Return from the method.
            generator.Emit(OpCodes.Ret);

            var del = dynamicMethod.CreateDelegate(typeof(Func<object, Dictionary<string, object>>));
            return (Func<object, Dictionary<string, object>>)del;
        }
    }

    /// <summary>
    /// An <see cref="IObjectConverterMethodBuilder"/> decorator that caches
    /// the method created at runtime that is used to convert an object into a dictionary.
    /// </summary>
    public class CachedObjectConverterMethodBuilder : IObjectConverterMethodBuilder
    {
        private readonly IObjectConverterMethodBuilder methodBuilder;
        private ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> cache = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedObjectConverterMethodBuilder"/> class.
        /// </summary>
        /// <param name="methodBuilder">The target <see cref="IObjectConverterMethodBuilder"/>.</param>
        public CachedObjectConverterMethodBuilder(IObjectConverterMethodBuilder methodBuilder) => this.methodBuilder = methodBuilder;

        /// <inheritdoc/>
        public Func<object, Dictionary<string, object>> CreateMethod(Type targetType) => cache.GetOrAdd(targetType, methodBuilder.CreateMethod);
    }
}