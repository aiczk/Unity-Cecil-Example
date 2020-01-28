using LINQ2Method.Basics;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono_Cecil_Sample.Script;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace LINQ2Method
{
    [InitializeOnLoad]
    public static class FuncTest
    {
        static FuncTest()
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
                var readerParams = CecilUtility.ReadAndWrite;
                var mainAssembly = CecilUtility.GetAssembly("Assembly-CSharp").ToAssemblyDefinition(readerParams);
                Execute(mainAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(AssemblyDefinition mainAssembly)
        {
            var mainAssemblyModule = mainAssembly.MainModule;
            
            var mainTestClassDefinition = mainAssemblyModule.GetType("_Script", "FuncTester");
            
/*          foreach (var nestedType in mainTestClassDefinition.NestedTypes)
            {
                foreach (var method in nestedType.Methods)
                {
                    if (method.Name.Equals(".ctor") || method.Name.Equals(".cctor"))
                        continue;
                    
                    foreach (var instruction in method.Body.Instructions)
                    {
                        Debug.Log($"{method.Name} : {instruction}");
                    }
                    
                }
            }*/

            var methodDefinition = new MethodDefinition("TestMethod", MethodAttributes.Private, mainAssemblyModule.TypeSystem.Void);
            mainTestClassDefinition.Methods.Add(methodDefinition);

            var forLoop = new For(mainAssemblyModule.TypeSystem);
            
            forLoop.Start(methodDefinition.Body);
            forLoop.End(methodDefinition.Body, 5);

            //ForLoop(mainAssemblyModule.TypeSystem, methodDefinition.Body, 10);

            //mainAssemblyModule.Write("Test.dll");
        }
    }
}
