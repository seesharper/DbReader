using System;
using System.Reflection.Emit;

namespace DbReader.DynamicArguments
{
    /// <summary>
    /// Creates <see cref="IDynamicType"/> instances.
    /// </summary>
    public class DynamicTypeActivator
    {
        /// <summary>
        /// Creates an instance of the given <paramref name="dynamicType"/>
        /// </summary>
        /// <param name="dynamicType">The dynamic type for which to create an instance.</param>
        /// <param name="members">The list of members and their values.</param>
        /// <returns></returns>
        public IDynamicType Activate(Type dynamicType, DynamicMemberInfo[] members)
        {
            var activatorMethod = new DynamicMethod("Activate", typeof(IDynamicType), parameterTypes: new Type[] { typeof(DynamicMemberInfo[]) }, typeof(DynamicTypeActivator), true);
            var generator = activatorMethod.GetILGenerator();

            for (int i = 0; i < members.Length; i++)
            {
                DynamicMemberInfo member = members[i];

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);
                var closedGenericMemberInfoType = typeof(DynamicMemberInfo<>).MakeGenericType(member.Type);
                generator.Emit(OpCodes.Castclass, closedGenericMemberInfoType);
                var fieldInfo = closedGenericMemberInfoType.GetField("Value");
                generator.Emit(OpCodes.Ldfld, fieldInfo);
            }

            var constructorInfo = dynamicType.GetConstructors()[0];
            generator.Emit(OpCodes.Newobj, constructorInfo);

            generator.Emit(OpCodes.Ret);



            var activatorDelegate = (Func<DynamicMemberInfo[], IDynamicType>)activatorMethod.CreateDelegate(typeof(Func<DynamicMemberInfo[], IDynamicType>));

            var instance = activatorDelegate(members);

            return instance;
        }
    }

}
