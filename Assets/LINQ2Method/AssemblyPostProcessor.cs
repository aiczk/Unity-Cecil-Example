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
/*
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
*/

            var typeSystem = mainModule.TypeSystem;
            var nestedType = mainTestClassDefinition.NestedTypes[0];
            var whereFunc = nestedType.Methods[2];
            var where2Func = nestedType.Methods[3];
            var selectFunc = nestedType.Methods[4];
            var argType = whereFunc.Parameters[0].ParameterType;
            var method = new Method(typeSystem, mainTestClassDefinition);
            var where = new Where(typeSystem, whereFunc, method.ForLoop);
            var where2 = new Where(typeSystem, where2Func, method.ForLoop);
            var select = new Select(selectFunc, method.ForLoop);

            method.Create("TestMethod", argType, typeSystem.Void);
            method.Start();

            method.AddOperator(Operator.Where, where);
            method.AddOperator(Operator.Where, where2);
            method.AddOperator(Operator.Select, select);
            
            method.Build();
            
//            where.Define(method.Body, where2.Next());
//            where2.Define(method.Body, select.Next());
//            select.Define(method.Body);
            
            method.End();
            
            mainModule.Write("Test.dll");
        }
    }
}
