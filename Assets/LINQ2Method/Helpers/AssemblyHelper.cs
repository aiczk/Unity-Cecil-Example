using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using UnityEngine;

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
        
        public static ModuleDefinition FindModule(string assemblyName)
        {
            var assembly = FindAssembly(assemblyName);
            return AssemblyDefinition.ReadAssembly(assembly.Location).MainModule;
        }
        
        public static ModuleDefinition FindModule(string assemblyName, ReaderParameters readerParameters)
        {
            var assembly = FindAssembly(assemblyName);
            return AssemblyDefinition.ReadAssembly(assembly.Location, readerParameters).MainModule;
        }

        public static Assembly FindAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.First(x => x.GetName().Name == assemblyName);
            return assembly;
        }

        public static AssemblyDefinition ToAssemblyDefinition(this Assembly assembly) => 
            AssemblyDefinition.ReadAssembly(assembly.Location);

        public static AssemblyDefinition ToAssemblyDefinition(this Assembly assembly, ReaderParameters parameters) => 
            AssemblyDefinition.ReadAssembly(assembly.Location, parameters);

    }
}
