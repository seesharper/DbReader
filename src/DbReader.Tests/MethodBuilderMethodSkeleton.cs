using DbReader.Construction;
#if NET462
namespace DbReader.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;

    public class MethodBuilderMethodSkeleton : IMethodSkeleton
    {
        private readonly string name;
        private readonly string outputPath;
        private readonly string fileName;
        private AssemblyBuilder assemblyBuilder;
        private TypeBuilder typeBuilder;
        private MethodBuilder methodBuilder;

        public MethodBuilderMethodSkeleton(string name, Type returnType, Type[] parameterTypes)
        {
            this.name = name;
            this.outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".dll");
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            fileName = name + ".dll";
            CreateAssemblyBuilder();
            CreateTypeBuilder();
            CreateMethodBuilder(returnType, parameterTypes);
        }

        private void CreateAssemblyBuilder()
        {
            AppDomain myDomain = AppDomain.CurrentDomain;
            assemblyBuilder = myDomain.DefineDynamicAssembly(CreateAssemblyName(), AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(outputPath));
        }

        private AssemblyName CreateAssemblyName()
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputPath);
            return new AssemblyName(fileNameWithoutExtension);
        }

        private void CreateTypeBuilder()
        {
            var moduleName = Path.GetFileNameWithoutExtension(outputPath);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(moduleName, fileName);
            typeBuilder = module.DefineType(name, TypeAttributes.Public);
        }

        private void CreateMethodBuilder(Type returnType, Type[] parameterTypes)
        {
            methodBuilder = typeBuilder.DefineMethod(
                "DynamicMethod", MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);
            methodBuilder.InitLocals = true;
        }

        public ILGenerator GetGenerator()
        {
            return methodBuilder.GetILGenerator();
        }

        public Delegate CreateDelegate(Type delegateType)
        {
            var dynamicType = typeBuilder.CreateType();
            assemblyBuilder.Save(fileName);
            Console.WriteLine("Saving file " + fileName);
            AssemblyAssert.IsValidAssembly(outputPath);
            MethodInfo methodInfo = dynamicType.GetMethod("DynamicMethod", BindingFlags.Static | BindingFlags.Public);
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }
    }
}
#endif