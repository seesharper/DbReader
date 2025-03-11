using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DbReader.Tracking;

public class TrackingAssemblyWeaver
{
    public void Weave(string assemblyPath, string attributeName)
    {
        var readerParameters = new ReaderParameters();
        readerParameters.ReadSymbols = true;
        readerParameters.ReadWrite = true;
        readerParameters.InMemory = true;
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);
        var types = assemblyDefinition.MainModule.Types
            .Where(t => t.CustomAttributes.Any(a => a.AttributeType.Name == attributeName))
            .ToList();
        foreach (var type in types)
        {
            WeaveType(type);
        }






        // File.Delete(assemblyPath);
        var writerParameters = new WriterParameters();
        writerParameters.WriteSymbols = true;
        assemblyDefinition.Write(assemblyPath, writerParameters);
    }

    private static bool HasParameterlessConstructor(TypeDefinition type)
    {
        return type.GetConstructors().Any(c => c.Parameters.Count == 0);
    }

    private static void CreateParameterlessConstructor(TypeDefinition type)
    {
        // Get the base class's parameterless constructor
        MethodReference baseConstructor = type.Module.ImportReference(
            type.BaseType.Resolve().Methods.FirstOrDefault(m => m.IsConstructor && !m.HasParameters)
        );

        var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, type.Module.ImportReference(typeof(void)));
        constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseConstructor));
        constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        type.Methods.Add(constructor);
    }


    private static void ImplementTrackedObject(TypeDefinition type, FieldDefinition modifiedPropertiesField)
    {
        var trackedObjectInterface = type.Module.ImportReference(typeof(ITrackedObject));
        type.Interfaces.Add(new InterfaceImplementation(trackedObjectInterface));

        var getModifiedPropertiesMethod = new MethodDefinition("GetModifiedProperties", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, type.Module.ImportReference(typeof(HashSet<string>)));
        var il = getModifiedPropertiesMethod.Body.GetILProcessor();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, modifiedPropertiesField);
        il.Emit(OpCodes.Ret);
        type.Methods.Add(getModifiedPropertiesMethod);
    }

    private static void InitializeModifiedPropertiesField(MethodDefinition method, FieldDefinition modifiedPropertiesField)
    {
        var instructions = method.Body.Instructions;
        var lastInstruction = instructions.Last();
        var il = method.Body.GetILProcessor();
        il.InsertBefore(lastInstruction, il.Create(OpCodes.Ldarg_0));
        il.InsertBefore(lastInstruction, il.Create(OpCodes.Newobj, method.Module.ImportReference(typeof(HashSet<string>).GetConstructors().First())));
        il.InsertBefore(lastInstruction, il.Create(OpCodes.Stfld, modifiedPropertiesField));
    }

    private static void AddParameterlessConstructor(TypeDefinition typeDefinition, ModuleDefinition moduleDefinition, FieldDefinition modifiedPropertiesField)
    {
        // Create a new method definition for the parameterless constructor
        MethodDefinition constructor = new MethodDefinition(
            ".ctor", // Constructor name
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
            moduleDefinition.ImportReference(typeof(void)) // Return type (void for constructors)
        );

        // Get the base class's parameterless constructor
        MethodReference baseConstructor = moduleDefinition.ImportReference(
            typeDefinition.BaseType.Resolve().Methods.FirstOrDefault(m => m.IsConstructor && !m.HasParameters)
        );

        // Create the method body
        ILProcessor il = constructor.Body.GetILProcessor();

        // Call the base class's parameterless constructor
        il.Emit(OpCodes.Ldarg_0); // Load 'this'
        il.Emit(OpCodes.Call, baseConstructor); // Call the base constructor

        // initialize the modifiedProperties field
        il.Emit(OpCodes.Ldarg_0); // Load 'this'
        il.Emit(OpCodes.Newobj, moduleDefinition.ImportReference(typeof(HashSet<string>).GetConstructors().First())); // Create a new HashSet<string>
        il.Emit(OpCodes.Stfld, modifiedPropertiesField); // Store the HashSet<string> in the modifiedProperties field

        il.Emit(OpCodes.Ret); // Return

        // Add the constructor to the type
        typeDefinition.Methods.Add(constructor);
    }

    private static void WeaveType(TypeDefinition type)
    {
        var modifiedPropertiesField = new FieldDefinition("modifiedProperties", FieldAttributes.Private, type.Module.ImportReference(typeof(HashSet<string>)));
        type.Fields.Add(modifiedPropertiesField);

        if (!HasParameterlessConstructor(type))
        {
            CreateParameterlessConstructor(type);
        }

        var allConstructors = type.GetConstructors();
        foreach (var constructor in allConstructors)
        {
            InitializeModifiedPropertiesField(constructor, modifiedPropertiesField);
        }

        ImplementTrackedObject(type, modifiedPropertiesField);

        var properties = type.Properties
            .Where(p => p.SetMethod is not null)
            .ToList();
        foreach (var property in properties)
        {
            WeaveSetMethod(property.SetMethod, modifiedPropertiesField);
        }
    }

    private static void WeaveSetMethod(MethodDefinition setMethod, FieldDefinition modifiedPropertiesField)
    {
        var instructions = setMethod.Body.Instructions;
        var lastInstruction = instructions.Last();
        var processor = setMethod.Body.GetILProcessor();

        var propertyName = setMethod.Name.Substring(4);
        // Call the add method of the modifiedProperties field
        processor.InsertBefore(lastInstruction, processor.Create(OpCodes.Ldarg_0)); // Load 'this'
        processor.InsertBefore(lastInstruction, processor.Create(OpCodes.Ldfld, modifiedPropertiesField)); // Load the modifiedProperties field
        processor.InsertBefore(lastInstruction, processor.Create(OpCodes.Ldstr, propertyName)); // Load the property name
        processor.InsertBefore(lastInstruction, processor.Create(OpCodes.Callvirt, setMethod.Module.ImportReference(typeof(HashSet<string>).GetMethod("Add")))); // Call the Add method        
        processor.InsertBefore(lastInstruction, processor.Create(OpCodes.Pop)); // Pop the return value
    }
}
