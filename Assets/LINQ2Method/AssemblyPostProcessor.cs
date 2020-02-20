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
            var contextFactory = new ContextFactory();
            foreach (var optimizeClass in optimizeClasses)
            {
                var nestedType = optimizeClass.NestedTypes[0];
                var argType = nestedType.Methods[2].Parameters[0].ParameterType;
                var operators = contextFactory.CreateContext(optimizeClass);
                var operatorContexts = contextFactory.CreateOperatorContext(optimizeClass);
                var returnType = mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);

                var method = new Method(typeSystem, optimizeClass);
                method.Create("TestMethod", argType, returnType);
                method.Begin();
                method.BuildOperator();
                method.End();
            }

/*
            foreach (var classDefinition in optimizeClasses)
            {
                var nestedType = classDefinition.NestedTypes[0];
                var argType = nestedType.Methods[2].Parameters[0].ParameterType;
                
                //TODO Selectの返り値を持ってくる
                var returnType = mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(argType);

                var method = new Method(typeSystem, classDefinition);
                var where = new Where(typeSystem, nestedType.Methods[2], method.MainLoop);
                var where2 = new Where(typeSystem, nestedType.Methods[3], method.MainLoop);
                var select = new Select(nestedType.Methods[4], method.MainLoop);

                method.Create("TestMethod", argType, returnType);
                method.Begin();

                method.AppendOperator(where);
                method.AppendOperator(where2);
                method.AppendOperator(select);
                method.BuildOperator();
            
                method.End();
            }
*/
            
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
                    
                    foreach (var customAttribute in method.CustomAttributes)
                    {
                        var attributeType = customAttribute.AttributeType;

                        if (attributeType.Name != attributeName)
                            continue;

                        definitions.Add(classDefinition);
                    }
                }
            }

            return definitions;
        }
    }
}
