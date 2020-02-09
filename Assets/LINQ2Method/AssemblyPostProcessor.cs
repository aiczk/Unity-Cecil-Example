﻿using System.Linq;
using System.Text.RegularExpressions;
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
                    var localVariable = body.Variables;
                    var instructions = body.Instructions;
                }
            }

            var typeSystem = mainModule.TypeSystem;
            var methodDefinition = new MethodDefinition("TestMethod", MethodAttributes.Private, typeSystem.Void);
            mainTestClassDefinition.Methods.Add(methodDefinition);
            
            var forLoop = new For(typeSystem);
            var arg = new Arg(typeSystem);
            var where = new Where(typeSystem);
            var select = new Select();
            var methodBody = methodDefinition.Body;
            var whereFuncMethod = mainTestClassDefinition.NestedTypes[0].Methods[2];
            var selectFuncMethod = mainTestClassDefinition.NestedTypes[0].Methods[3];
            var paramType = whereFuncMethod.Parameters[0].ParameterType;
            
            arg.Define(methodBody, paramType);
            forLoop.Start(methodBody);
            forLoop.Local(methodBody, paramType);
            
            where.Define(methodBody, whereFuncMethod.Body, forLoop, forLoop.LoopEnd);
            select.Define(methodBody, selectFuncMethod.Body, forLoop);
            
            forLoop.End(methodBody, arg.ElementLengthDefinition);
            
            InstructionHelper.Return(methodBody);
            mainModule.Write("Test.dll");
        }
    }
}
