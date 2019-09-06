using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Mono_Cecil_Sample.Script
{
    public static class CecilUtility
    {
        public static ReaderParameters ReadAndWrite => 
            new ReaderParameters
        {
            ReadWrite = true,
            InMemory = true,
            ReadingMode = ReadingMode.Immediate
        };
        
        public static Assembly GetAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);
            return assembly;
        }
        
        public static AssemblyDefinition ToAssemblyDefinition(this Assembly assembly) => 
            AssemblyDefinition.ReadAssembly(assembly.Location);

        public static AssemblyDefinition ToAssemblyDefinition(this Assembly assembly,ReaderParameters parameters) => 
            AssemblyDefinition.ReadAssembly(assembly.Location, parameters);

        public static bool IsExistAttributesInClass(TypeDefinition classDefinition, string attributeFullName)
        {
            if (classDefinition.Name == "<Module>")
                return false;
                
            return classDefinition
                   .Methods
                   .SelectMany(x => x.CustomAttributes)
                   .Any(x => x.AttributeType.FullName == attributeFullName);
        }
        
        public static bool IsAttachedMethodAttribute(MethodDefinition methodDefinition, string attributeFullName)
        {
            return methodDefinition
                   .CustomAttributes
                   .Any(x => x.AttributeType.FullName == attributeFullName);
        }
        
        public static bool IsExistAttributeInGlobal(TypeDefinition typeDefinition,string attributeFullName) => 
            typeDefinition.CustomAttributes.Any(x => x.AttributeType.FullName == attributeFullName);

        public static void RemoveAttribute(MethodDefinition targetMethod, string attributeFullName)
        {
            foreach (var customAttribute in targetMethod.CustomAttributes)
            {
                if(customAttribute.AttributeType.FullName != attributeFullName)
                    continue;
                    
                targetMethod.CustomAttributes.Remove(customAttribute);
                break;
            }
        }
        
        public static void RemoveAttribute(TypeDefinition targetDefinition, string attributeFullName)
        {
            foreach (var customAttribute in targetDefinition.CustomAttributes)
            {
                if(customAttribute.AttributeType.FullName != attributeFullName)
                    continue;
                    
                targetDefinition.CustomAttributes.Remove(customAttribute);
                break;
            }
        }
    }
}
