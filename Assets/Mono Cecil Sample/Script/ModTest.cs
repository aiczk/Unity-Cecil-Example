using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace Mono_Cecil_Sample.Script
{
    [InitializeOnLoad]
    public static class ModTest
    {
        private static string NameSpace => $"{Application.productName.Replace(" ","_")}.Mod";
        
        static ModTest()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            //PostCompile();
        }

        private static void PostCompile()
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                var mainAssembly = CecilUtility.GetAssembly("Assembly-CSharp").ToAssemblyDefinition();
                var editorAssembly = CecilUtility.GetAssembly("MCI").ToAssemblyDefinition();
                var engineAssembly = CecilUtility.EngineAssemblyDefinition();
                var modAssembly = CreateAssembly();
                
                Process(mainAssembly, editorAssembly, engineAssembly, modAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Process
            (
                AssemblyDefinition mainAssemblyDefinition,
                AssemblyDefinition editorAssemblyDefinition,
                AssemblyDefinition engineAssemblyDefinition,
                AssemblyDefinition modAssemblyDefinition
            )
        {
            var mainModuleDefinition = mainAssemblyDefinition.MainModule;
            var editorModuleDefinition = editorAssemblyDefinition.MainModule;
            var engineModuleDefinition = engineAssemblyDefinition.MainModule;
            var modModuleDefinition = modAssemblyDefinition.MainModule;

            var modAttributeTypeDefinition = editorModuleDefinition.GetType("Mono_Cecil_Sample.Attributes", "ModAttribute");
            var modAttributeFullName = modAttributeTypeDefinition.FullName;

            var definitions = mainModuleDefinition
                              .Types
                              .Where(x => CecilUtility.IsExistAttributeInGlobal(x, modAttributeFullName))
                              .ToArray();
            
            TryInjectTypes(in definitions, modModuleDefinition);
            //modModuleDefinition.Write("Mod.dll");
        }

        private static void TryInjectTypes(in TypeDefinition[] globalModTargets,ModuleDefinition mod)
        {
            //todo 不完全なので改善する。
            foreach (var target in globalModTargets)
            {
                if (target.IsInterface)
                {
                    //Debug.Log($"interface {target.Name}");
                }

                if (target.IsEnum)
                {
                    //Debug.Log($"enum {target.Name}");
                }

                //Debug.Log($"class {target.Name}");

                var methods = target.Methods;

                foreach (var name in methods.Select(x => x.Name))
                {
                    foreach (var instruction in methods.SelectMany(methodDefinition => methodDefinition.Body.Instructions))
                    {
                        Debug.Log($"Type:{target.Name} Method:{name} IL:{instruction}");
                    }
                }
                
                //var typeDefinition = new TypeDefinition(NameSpace, target.Name, target.Attributes);
                //mod.Types.Add(typeDefinition);
            }
        }

        //todo すでに存在していたら上書き
        private static AssemblyDefinition CreateAssembly()
        {
            return AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Md", new Version(1, 0, 0)), "Mod", ModuleKind.Dll);
        }
    }
}
