using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using FieldAttributes = Mono.Cecil.FieldAttributes;
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Mono_Cecil_Intro.Script
{
    [InitializeOnLoad]
    public static class CecilTest
    {
        static CecilTest()
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
                var mainAssembly = GetAssembly("Assembly-CSharp");
                var editorAssembly = GetAssembly("MCI");
                Process(mainAssembly, editorAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static Assembly GetAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);
            return assembly;
        }

        private static void Process(Assembly mainAssembly,Assembly editorAssembly)
        {
            var mainAssemblyDefinition = AssemblyDefinition.ReadAssembly(mainAssembly.Location, new ReaderParameters{ReadWrite = true});
            var mainModuleDefinition = mainAssemblyDefinition.MainModule;

            var editorAssemblyDefinition = AssemblyDefinition.ReadAssembly(editorAssembly.Location);
            var editorModuleDefinition = editorAssemblyDefinition.MainModule;
            var profileAttributeTypeDefinition = editorModuleDefinition.GetType("Mono_Cecil_Intro.Script", "ProfileAttribute");
            
            //注入対象の型の列挙
            foreach (var classTypeDefinition in mainModuleDefinition.Types.Where(ProfileMethodCheck).ToArray())
            {
                if (!TryInjectSamplerField(mainModuleDefinition, classTypeDefinition, out var samplerFieldDefinition))
                    continue;

                foreach (var method in classTypeDefinition.Methods.Where(ProfileAttributeCheck))
                {
                    InjectSamplerCode(mainModuleDefinition, method, samplerFieldDefinition);
                    RemoveProfileAttribute(method);
                }
            }
            
            //IO Exception
            mainModuleDefinition.Write(mainAssembly.Location);
            
            bool ProfileMethodCheck(TypeDefinition typeDefinition)
            {
                if (typeDefinition.Name == "<Module>")
                    return false;
                
                return typeDefinition
                       .Methods
                       .SelectMany(x => x.CustomAttributes)
                       .Any(x => x.AttributeType.FullName == profileAttributeTypeDefinition.FullName);
            }

            bool ProfileAttributeCheck(MethodDefinition methodDefinition)
            {
                return methodDefinition
                       .CustomAttributes
                       .Any(x => x.AttributeType.FullName == profileAttributeTypeDefinition.FullName);
            }
            
            void RemoveProfileAttribute(MethodDefinition methodDefinition)
            {
                foreach (var customAttribute in methodDefinition.CustomAttributes)
                {
                    if(customAttribute.AttributeType.FullName != "Mono_Cecil_Intro.Script.ProfileAttribute")
                        continue;
                    
                    methodDefinition.CustomAttributes.Remove(customAttribute);
                    break;
                }
            }
        }

        //Custom Samplerを注入するのを試みる。
        private static bool TryInjectSamplerField(ModuleDefinition mainModule, TypeDefinition classDefinition, out FieldDefinition samplerFieldDefinition)
        {
            samplerFieldDefinition = default;

            var awakeMethodDefinition = classDefinition.Methods.FirstOrDefault(x => x.Name == "Awake");
            if (awakeMethodDefinition == null)
                return false;

            var fieldName = "_sampler";
            var samplerName = "Custom Sampler";

            var samplerTypeDefinition = mainModule.ImportReference(typeof(CustomSampler));
            samplerFieldDefinition = new FieldDefinition(fieldName, FieldAttributes.Private, samplerTypeDefinition);
            
            classDefinition.Fields.Add(samplerFieldDefinition);
            
            var createMethodInfo = typeof(CustomSampler).GetMethod(nameof(CustomSampler.Create), new[] { typeof(string) });
            var createMethodReference = mainModule.ImportReference(createMethodInfo);

            var awakeMethodBody = awakeMethodDefinition.Body;
            var processor = awakeMethodBody.GetILProcessor();
            var awakeFirst = awakeMethodBody.Instructions.First();

            processor.InsertBefore(awakeFirst, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(awakeFirst, Instruction.Create(OpCodes.Ldstr, samplerName));
            processor.InsertBefore(awakeFirst, Instruction.Create(OpCodes.Call, createMethodReference));
            processor.InsertBefore(awakeFirst, Instruction.Create(OpCodes.Stfld, samplerFieldDefinition));

            return true;
        }

        private static void InjectSamplerCode(ModuleDefinition mainModule, MethodDefinition methodDefinition, FieldDefinition samplerFieldDefinition)
        {
            var beginMethod = typeof(CustomSampler).GetMethod(nameof(CustomSampler.Begin), Array.Empty<Type>());
            var endMethod = typeof(CustomSampler).GetMethod(nameof(CustomSampler.End), Array.Empty<Type>());

            var beginMethodReference = mainModule.ImportReference(beginMethod);
            var endMethodReference = mainModule.ImportReference(endMethod);

            var methodBody = methodDefinition.Body;
            var instructions = methodBody.Instructions;
            var processor = methodBody.GetILProcessor();
            var first = instructions.First();
            var last = instructions.Last();

            processor.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Ldfld, samplerFieldDefinition));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Callvirt, beginMethodReference));
            
            processor.InsertBefore(last, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(last, Instruction.Create(OpCodes.Ldfld, samplerFieldDefinition));
            processor.InsertBefore(last, Instruction.Create(OpCodes.Callvirt, endMethodReference));
        }
    }
}
