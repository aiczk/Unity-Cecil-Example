using System.Linq;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
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

            foreach (var nestedType in mainTestClassDefinition.NestedTypes)
            {
                foreach (var method in nestedType.Methods)
                {
                    if (method.Name.Equals(".ctor") || method.Name.Equals(".cctor"))
                        continue;
                    
                }
            }

            foreach (var typeDefinition in mainModule.Types)
            {
                if(!typeDefinition.IsClass)
                    continue;
                
                foreach (var method in typeDefinition.Methods)
                {
                    if (!method.CustomAttributes.Any(x => x.AttributeType.Name.Equals("OptimizationAttribute")))
                        continue;

                    var body = method.Body;
                    
                    foreach (var instruction in body.Instructions)
                    {
                        
                    }
                }
            }

            var typeSystem = mainModule.TypeSystem;
            var methodDefinition = new MethodDefinition("TestMethod", MethodAttributes.Private, typeSystem.Void);
            mainTestClassDefinition.Methods.Add(methodDefinition);
            
            var forLoop = new For(typeSystem);
            var arg = new Arg(typeSystem);
            var where = new Where(typeSystem, forLoop);
            var where2 = new Where(typeSystem, forLoop);
            var select = new Select(forLoop);
            var methodBody = methodDefinition.Body;
            var whereFuncMethod = mainTestClassDefinition.NestedTypes[0].Methods[2];
            var selectFuncMethod = mainTestClassDefinition.NestedTypes[0].Methods[3];
            var paramType = whereFuncMethod.Parameters[0].ParameterType;
            
            arg.Define(methodBody, paramType);
            forLoop.Start(methodBody);
            forLoop.CreateLocal(methodBody, paramType);
            
            where.Define(methodBody, whereFuncMethod.Body, where2.Next(selectFuncMethod.Body));
            where2.Define(methodBody, selectFuncMethod.Body, forLoop.LoopEnd);
            //select.Define(methodBody, selectFuncMethod.Body, forLoop);
            
            forLoop.End(methodBody);
            
            InstructionHelper.Return(methodBody);
            mainModule.Write("Test.dll");
        }
    }
}
