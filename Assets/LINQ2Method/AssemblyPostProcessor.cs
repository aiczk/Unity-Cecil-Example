using System;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace LINQ2Method
{
    [InitializeOnLoad]
    internal static class AssemblyPostProcessor
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
                var coreModule = AssemblyHelper.GetCoreModule();
                Execute(mainModule, l2MModule, coreModule);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
            //stopWatch.Stop();
            //Debug.Log(stopWatch.Elapsed.ToString());
        }

        private static void Execute(ModuleDefinition mainModule, ModuleDefinition l2MModule, ModuleDefinition coreModule)
        {
            var l2MOptimizeAttribute = l2MModule.GetType("LINQ2Method.Attributes", "OptimizeAttribute");
            var typeSystem = mainModule.TypeSystem;
            
            var classAnalyzer = new ClassAnalyzer(mainModule, l2MOptimizeAttribute);
            var methodAnalyzer = new MethodAnalyzer(typeSystem);
            var methodBuilder = new MethodBuilder(mainModule, coreModule);
            
            var analyzedClass = classAnalyzer.Analyze();
            foreach (var targetClass in analyzedClass.OptimizeTypes)
            {
                foreach (var method in classAnalyzer.AnalyzeMethod(targetClass))
                {
                    var analyzedMethod = methodAnalyzer.Analyze(method);
                    var methodName = Guid.NewGuid().ToString("N");
                    
                    methodBuilder.Create(targetClass, methodName, analyzedMethod.ParameterType, analyzedMethod.ReturnType);
                    methodBuilder.Begin();
                    
                    foreach (var linqOperator in analyzedMethod.Operators)
                    {
                        var linq = methodAnalyzer.OperatorFactory(linqOperator, methodBuilder);
                        methodBuilder.AppendOperator(linq);
                    }
                    
                    methodBuilder.BuildOperator();
                    methodBuilder.End();
                    methodBuilder.Replace(method);
                }
            }

            mainModule.Write("Test.dll");
        }
    }
}
