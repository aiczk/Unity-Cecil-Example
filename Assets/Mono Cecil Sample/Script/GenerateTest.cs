using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Mono_Cecil_Sample.Script
{
    [InitializeOnLoad]
    public static class GenerateTest
    {
        static GenerateTest()
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
                var mainAssembly = CecilUtility.GetAssembly("Assembly-CSharp");
                var editorAssembly = CecilUtility.GetAssembly("MCI");
                var engineAssembly = EngineAssemblyDefinition();

                Process(mainAssembly, editorAssembly, engineAssembly);
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

        private static void Process(Assembly mainAssembly, Assembly editorAssembly, AssemblyDefinition engineAssemblyDefinition)
        {
            var readerParameters = new ReaderParameters
            {
                ReadWrite = true,
                InMemory = true,
                ReadingMode = ReadingMode.Immediate
            };
            
            var mainAssemblyDefinition = AssemblyDefinition.ReadAssembly(mainAssembly.Location, readerParameters);
            var mainModuleDefinition = mainAssemblyDefinition.MainModule;

            var editorAssemblyDefinition = AssemblyDefinition.ReadAssembly(editorAssembly.Location);
            var editorModuleDefinition = editorAssemblyDefinition.MainModule;

            var engineModuleDefinition = engineAssemblyDefinition.MainModule;

            var generateAttributeTypeDefinition = editorModuleDefinition.GetType("Mono_Cecil_Sample.Attributes", "GenerateAttribute");
            var generateAttributeFullName = generateAttributeTypeDefinition.FullName;
            
            var typeDefinitions = mainModuleDefinition
                                  .Types
                                  .Where(x => x.CustomAttributes.Any(a => a.AttributeType.FullName == generateAttributeFullName))
                                  .ToArray();

            foreach (var classTypeDefinition in typeDefinitions)
            {
                classTypeDefinition.Methods.Add(DefineAwake(mainModuleDefinition, engineModuleDefinition));
                CecilUtility.RemoveAttribute(classTypeDefinition, generateAttributeFullName);
            }
            
            mainModuleDefinition.Write(mainAssembly.Location);
        }

        private static MethodDefinition DefineAwake(ModuleDefinition mainModuleDefinition,ModuleDefinition engineModuleDefinition)
        {
            var awakeMethodDefinition = new MethodDefinition("Awake", MethodAttributes.Private, mainModuleDefinition.TypeSystem.Void);
            var awakeMethodBody = awakeMethodDefinition.Body;
            
            bool Predicate(MethodDefinition method) => method.Name == ".ctor" && method.Parameters.Count == 1 && method.Parameters[0].ParameterType.Name == "String";
            var gameObjectReference = mainModuleDefinition.ImportReference(engineModuleDefinition.GetType("UnityEngine", "GameObject").Methods.Single(Predicate));

            var processor = awakeMethodBody.GetILProcessor();

            processor.Append(Instruction.Create(OpCodes.Ldstr, "Mono Cecil Generated"));
            processor.Append(Instruction.Create(OpCodes.Newobj, gameObjectReference));
            processor.Append(Instruction.Create(OpCodes.Pop));
            processor.Append(Instruction.Create(OpCodes.Ret));

            return awakeMethodDefinition;
        }
    }
}