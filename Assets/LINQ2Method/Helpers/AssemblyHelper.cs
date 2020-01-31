using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace LINQ2Method.Helpers
{
    public static class AssemblyHelper
    {
        public static ReaderParameters ReadAndWrite() => 
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

        public static AssemblyDefinition ToAssemblyDefinition(this Assembly assembly, ReaderParameters parameters) => 
            AssemblyDefinition.ReadAssembly(assembly.Location, parameters);

    }
}
