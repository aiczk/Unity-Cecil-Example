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
            
            var classAnalyzer = new ClassAnalyzer(mainModule, l2MOptimizeAttribute);
            var methodAnalyzer = new MethodAnalyzer(typeSystem);
            
            var analyzedClass = classAnalyzer.Analyze();

            //var returnType mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);
            foreach (var targetClass in analyzedClass.OptimizeTypes)
            {
                var methodBuilder = new MethodBuilder(typeSystem, targetClass);
                foreach (var method in classAnalyzer.AnalyzeMethod(targetClass))
                {
                    var analyzedMethod = methodAnalyzer.Analyze(method);
                    methodBuilder.Create(Guid.NewGuid().ToString("N"), analyzedMethod.ParameterType, analyzedMethod.ReturnType);
                    methodBuilder.Begin();
                    
                    foreach (var linqOperator in analyzedMethod.Operators)
                    {
                        var linq = methodAnalyzer.OperatorFactory(linqOperator, methodBuilder);
                        methodBuilder.AppendOperator(linq);
                    }
                    
                    methodBuilder.BuildOperator();
                    methodBuilder.End();
                }
            }

            mainModule.Write("Test.dll");
        }
    }
}
