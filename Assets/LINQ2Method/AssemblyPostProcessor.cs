using _Script;
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
                var mainAssembly = AssemblyHelper.GetAssembly("Main").ToAssemblyDefinition(readerParams);
                Execute(mainAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(AssemblyDefinition mainAssembly)
        {
            var mainModule = mainAssembly.MainModule;
            
            var mainTestClassDefinition = mainModule.GetType("_Script", "FuncTester");

/*          foreach (var nestedType in mainTestClassDefinition.NestedTypes)
            {
                foreach (var method in nestedType.Methods)
                {
                    if (method.Name.Equals(".ctor") || method.Name.Equals(".cctor"))
                        continue;
                    
                    foreach (var instruction in method.Body.Instructions)
                    {
                        Debug.Log($"{method.Name} : {instruction}");
                    }
                    
                }
            }*/

            var typeSystem = mainModule.TypeSystem;
            var methodDefinition = new MethodDefinition("TestMethod", MethodAttributes.Private, typeSystem.Void);
            mainTestClassDefinition.Methods.Add(methodDefinition);
            
            var forLoop = new For(typeSystem);
            var array = new Array(typeSystem);
            var branch = new If(typeSystem);
            var methodBody = methodDefinition.Body;
            var funcMethod = mainTestClassDefinition.NestedTypes[0].Methods[2];
            
            array.Create(methodBody, funcMethod.Parameters[0].ParameterType);
            forLoop.Start(methodBody);
            
            branch.Define(methodBody, funcMethod.Body, forLoop, forLoop.LoopEnd);

            forLoop.End(methodBody, 90);
            
            InstructionHelper.Return(methodBody);
            mainModule.Write("Test.dll");
            
        }
    }
}
