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

//            foreach (var nestedType in mainTestClassDefinition.NestedTypes)
//            {
//                foreach (var method in nestedType.Methods)
//                {
//                    if (method.Name.Equals(".ctor") || method.Name.Equals(".cctor"))
//                        continue;
//                    
//                }
//            }
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
            var nestedType = mainTestClassDefinition.NestedTypes[0];
            var whereFunc = nestedType.Methods[2];
            var where2Func = nestedType.Methods[3];
            var selectFunc = nestedType.Methods[4];
            var paramType = whereFunc.Parameters[0].ParameterType;
            var methodBody = methodDefinition.Body;
            
            arg.Define(methodBody, paramType);
            forLoop.Start(methodBody);
            forLoop.CreateLocal(methodBody, paramType);
            
            where.Define(methodBody, whereFunc.Body, where2.Next(where2Func.Body));
            where2.Define(methodBody, where2Func.Body, select.Next(selectFunc.Body));
            select.Define(methodBody, selectFunc.Body);
            
            forLoop.End(methodBody);
            
            InstructionHelper.Return(methodBody);
            mainModule.Write("Test.dll");
        }
    }
}
