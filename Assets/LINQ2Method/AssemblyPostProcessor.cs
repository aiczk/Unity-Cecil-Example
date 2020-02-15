using System;
using System.Collections.Generic;
using _Script;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using UnityEditor;
using UnityEngine;

namespace LINQ2Method
{
    [InitializeOnLoad]
    public static class AssemblyPostProcessor
    {
        static AssemblyPostProcessor()
        { 
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            PostCompile();
        }

        private static void PostCompile()
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                var readerParams = AssemblyHelper.ReadAndWrite();
                var mainAssembly = AssemblyHelper.FindModule("Main", readerParams);
                Execute(mainAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(ModuleDefinition mainModule)
        {
            var mainDefinition = mainModule.GetType("_Script", "FuncTester");
            
            var typeSystem = mainModule.TypeSystem;
            var nestedType = mainDefinition.NestedTypes[0];
            var argType = nestedType.Methods[2].Parameters[0].ParameterType;
            var returnType = mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);
            var method = new Method(typeSystem, mainDefinition);

            var where = new Where(typeSystem, nestedType.Methods[2], method.MainLoop);
            var where2 = new Where(typeSystem, nestedType.Methods[3], method.MainLoop);
            var select = new Select(nestedType.Methods[4], method.MainLoop);
            
            method.Create("TestMethod", argType, typeSystem.Void);
            method.Begin();

            method.AddOperator(where);
            method.AddOperator(where2);
            method.AddOperator(select);

            method.Build();
            method.End();
            
            mainModule.Write("Test.dll");
        }

        private static void Add(ModuleDefinition main)
        {
            Dictionary<string, MethodDefinition> linq = new Dictionary<string, MethodDefinition>();
            
            foreach (var type in main.Types)
            {
                if(!type.IsClass)
                    continue;
                
                if(!type.HasNestedTypes)
                    continue;
                
                foreach (var nestedType in type.NestedTypes)
                {
                    if(!nestedType.IsClass)
                        continue;

                    if (!nestedType.HasMethods)
                        continue;

                    var methods = nestedType.Methods;
                }
            }
            
        }
    }
}
