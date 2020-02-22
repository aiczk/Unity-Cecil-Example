using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();
            EditorApplication.LockReloadAssemblies();
            try
            {
                var readerParams = AssemblyHelper.ReadAndWrite();
                var mainModule = AssemblyHelper.FindModule("Main", readerParams);
                var l2MModule = AssemblyHelper.FindModule("L2MAttributes", readerParams);
                Execute(mainModule,l2MModule);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
            //stopWatch.Stop();
            //Debug.Log(stopWatch.Elapsed.ToString());
        }

        private static void Execute(ModuleDefinition mainModule, ModuleDefinition l2MModule)
        {
            var l2MOptimizeAttribute = l2MModule.GetType("LINQ2Method.Attributes", "OptimizeAttribute");
            var typeSystem = mainModule.TypeSystem;
            
            var classAnalyzer = new ClassAnalyzer(mainModule);
            var methodAnalyzer = new MethodAnalyzer(typeSystem);
            
            var analyzedClass = classAnalyzer.Analyze(l2MOptimizeAttribute);
            
            foreach (var targetClass in analyzedClass.OptimizeTypes)
            {
                var nestedType = targetClass.NestedTypes[0];
                var argType = nestedType.Methods[2].Parameters[0].ParameterType;
                //var returnType mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);
                var returnType = typeSystem.Void;
                
                var method = new Method(typeSystem, targetClass);
                foreach (var targetMethod in analyzedClass.OptimizeMethods)
                {
                    var analyzedMethod = methodAnalyzer.Analyze(targetMethod);
                    method.Create($"TestMethod_{Guid.NewGuid().ToString("N")}", analyzedMethod.Parameter, returnType);
                    method.Begin();
                    
                    foreach (var linqOperator in analyzedMethod.Operators)
                    {
                        var op = methodAnalyzer.Generate(linqOperator, method);
                        method.AppendOperator(op);
                    }
                    
                    method.BuildOperator();
                    method.End();
                }
            }

            mainModule.Write("Test.dll");
        }
    }
}
