using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable once CheckNamespace
namespace LINQ2Method.Basics
{
    public class Field<T>
    {
        private ModuleDefinition moduleDefinition;
        public int Index { get; private set; }
        
        public Field(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        public void Emit(MethodBody methodBody)
        {
            Index = methodBody.AddVariable(moduleDefinition.ImportReference(typeof(T)));
            var processor = methodBody.GetILProcessor();
        }
    }
}