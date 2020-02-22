using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
                var mainModule = AssemblyHelper.FindModule("Main", readerParams);
                var l2MModule = AssemblyHelper.FindModule("L2MAttributes", readerParams);
                Execute(mainModule,l2MModule);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(ModuleDefinition mainModule, ModuleDefinition l2MModule)
        {
            var l2MOptimizeAttribute = l2MModule.GetType("LINQ2Method.Attributes", "OptimizeAttribute");
            var optimizeClasses = Search(mainModule, l2MOptimizeAttribute);

            if(optimizeClasses.Count < 0)
                return;
            
            var typeSystem = mainModule.TypeSystem;
            foreach (var targetClass in optimizeClasses)
            {
                var nestedType = targetClass.NestedTypes[0];
                var argType = nestedType.Methods[2].Parameters[0].ParameterType;
                //var returnType mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);
                var returnType = typeSystem.Void;

                var methodAnalyzer = new MethodAnalyzer(typeSystem, targetClass);
                var methods = methodAnalyzer.OptimizeMethods(l2MOptimizeAttribute.Name);
                
                var method = new Method(typeSystem, targetClass);
                foreach (var targetMethod in methods)
                {
                    var analyseResult = methodAnalyzer.Analyse(targetMethod);
                    method.Create($"TestMethod_{Guid.NewGuid().ToString("N")}", argType, returnType);
                    method.Begin();
                    
                    foreach (var linqOperator in analyseResult.Operators)
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

        private static List<TypeDefinition> Search(ModuleDefinition main, TypeDefinition attribute)
        {
            var definitions = new List<TypeDefinition>();
            var attributeName = attribute.Name;
            
            foreach (var classDefinition in main.Types)
            {
                if(!classDefinition.IsClass)
                    continue;
                
                if(!classDefinition.HasMethods)
                    continue;
                
                if(!classDefinition.HasNestedTypes)
                    continue;

                foreach (var method in classDefinition.Methods)
                {
                    if (!method.HasCustomAttributes)
                        continue;

                    var find = false;
                    foreach (var customAttribute in method.CustomAttributes)
                    {
                        var attributeType = customAttribute.AttributeType;

                        if (attributeType.Name != attributeName)
                            continue;

                        find = true;
                        break;
                    }

                    if (!find)
                        continue;
                    
                    definitions.Add(classDefinition);
                    break;
                }
            }

            return definitions;
        }
    }
}
