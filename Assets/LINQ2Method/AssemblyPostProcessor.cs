using System;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
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
                var systemAssembly = AssemblyHelper.FindModule("mscorlib", readerParams);
                Execute(mainAssembly, systemAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(ModuleDefinition mainModule, ModuleDefinition coreModule)
        {
            var iEnumerableDefinition = coreModule.GetType("System.Collections.Generic", "IEnumerable`1");
            var mainDefinition = mainModule.GetType("_Script", "FuncTester");
            mainModule.ImportReference(iEnumerableDefinition);

            var typeSystem = mainModule.TypeSystem;
            var nestedType = mainDefinition.NestedTypes[0];
            var argType = nestedType.Methods[2].Parameters[0].ParameterType;
            var method = new Method(typeSystem, mainDefinition);
            iEnumerableDefinition.GenericParameters.Add(new GenericParameter(argType));
            var where = new Where(typeSystem, nestedType.Methods[2], method.ForLoop);
            var where2 = new Where(typeSystem, nestedType.Methods[3], method.ForLoop);
            var select = new Select(nestedType.Methods[4], method.ForLoop);

            var returnType = typeSystem.Void;
            method.Create("TestMethod", argType, iEnumerableDefinition);
            method.Begin();

            method.AddOperator(where);
            method.AddOperator(where2);
            method.AddOperator(select);

            method.Build();
            method.End();
            
            mainModule.Write("Test.dll");
        }
    }
}
