using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Mono.Cecil.Utils
{
    public static class CecilUtility
    {
        public static Assembly GetAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);
            return assembly;
        }
        
        public static bool IsExistAttributesInClass(TypeDefinition classDefinition, string attributeFullName)
        {
            if (classDefinition.Name == "<Module>")
                return false;
                
            return classDefinition
                   .Methods
                   .SelectMany(x => x.CustomAttributes)
                   .Any(x => x.AttributeType.FullName == attributeFullName);
        }
        
        public static bool IsAttachedAttribute(MethodDefinition methodDefinition, string attributeFullName)
        {
            return methodDefinition
                   .CustomAttributes
                   .Any(x => x.AttributeType.FullName == attributeFullName);
        }
        
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
    }
}
