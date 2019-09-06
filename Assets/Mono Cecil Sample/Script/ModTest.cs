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
        static ModTest()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            PostCompile();
        }

        private static void PostCompile()
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                var mainAssembly = CecilUtility.GetAssembly("Assembly-CSharp").ToAssemblyDefinition();
                var editorAssembly = CecilUtility.GetAssembly("MCI").ToAssemblyDefinition();
                var engineAssembly = EngineAssemblyDefinition();
                var modAssembly = CreateAssembly();

                Process(mainAssembly, editorAssembly, engineAssembly, modAssembly);
                //modAssembly.Write("Mod.dll");
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }

            AssemblyDefinition EngineAssemblyDefinition()
            {
                const string path = "C:\\Program Files\\Unity\\Editor\\Data\\Managed\\UnityEngine.dll";
                return AssemblyDefinition.ReadAssembly(path);
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

            var modAttributeTypeDefinition = editorModuleDefinition.GetType("Mono_Cecil_Sample.Attributes", "ModAttribute");
            var modAttributeFullName = modAttributeTypeDefinition.FullName;

            var definitions = mainModuleDefinition
                              .Types
                              .Where(x => CecilUtility.IsExistAttributeInGlobal(x, modAttributeFullName))
                              .ToArray();

            var classes = new List<TypeDefinition>();
            var enums = new List<TypeDefinition>();
            var interfaces = new List<TypeDefinition>();

            Definitions(in definitions, ref classes, ref enums, ref interfaces);
        }

        private static void Definitions
        (
            in TypeDefinition[] definitions,
            ref List<TypeDefinition> classes,
            ref List<TypeDefinition> enums,
            ref List<TypeDefinition> interfaces
        )
        {
            foreach (var definition in definitions)
            {
                if (definition.IsInterface)
                {
                    Debug.Log($"interface {definition.Name}");
                    
                    interfaces.Add(definition);
                    continue;
                }

                if (definition.IsEnum)
                {
                    Debug.Log($"enum {definition.Name}");
                    enums.Add(definition);
                    continue;
                }

                Debug.Log($"class {definition.Name}");
                classes.Add(definition);
            }
        }

        //todo すでに存在していたら上書き
        private static AssemblyDefinition CreateAssembly()
        {
            return AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Md", new Version(1, 0, 0)), "Mod", ModuleKind.Dll);
        }
    }
}
