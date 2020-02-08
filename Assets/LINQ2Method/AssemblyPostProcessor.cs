using LINQ2Method.Basics;
using LINQ2Method.Helpers;
using Mono.Cecil;
using UnityEditor;

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
                var mainAssembly = AssemblyHelper.GetAssembly("Main").ToAssemblyDefinition(readerParams);
                Execute(mainAssembly);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(AssemblyDefinition mainAssembly)
        {
            var mainModule = mainAssembly.MainModule;
            
            var mainTestClassDefinition = mainModule.GetType("_Script", "FuncTester");

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

            var typeSystem = mainModule.TypeSystem;
            var methodDefinition = new MethodDefinition("TestMethod", MethodAttributes.Private, typeSystem.Void);
            mainTestClassDefinition.Methods.Add(methodDefinition);
            
            var forLoop = new For(typeSystem);
            var array = new Array(typeSystem);
            var where = new Where(typeSystem);
            var methodBody = methodDefinition.Body;
            var funcMethod = mainTestClassDefinition.NestedTypes[0].Methods[2];
            var paramType = funcMethod.Parameters[0].ParameterType;
            
            array.Define(methodBody, paramType);
            forLoop.Start(methodBody);
            forLoop.Local(methodBody, paramType);
            
            where.Define(methodBody, funcMethod.Body, forLoop, forLoop.LoopEnd);

            forLoop.End(methodBody, array.ElementLengthDefinition);
            
            InstructionHelper.Return(methodBody);
            mainModule.Write("Test.dll");
        }
    }
}
