using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Arg
    {
        private TypeSystem typeSystem;

        public Arg(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public void Define(MethodBody methodBody, TypeReference arrayType)
        {
            methodBody.Method.Parameters.Add(new ParameterDefinition(new ArrayType(arrayType)));
            methodBody.AddVariable(typeSystem.Int32);
        }
    }
}
