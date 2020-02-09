using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Method
    {
        private TypeSystem typeSystem;
        private For forLoop;
        private Arg arg;

        public Method(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            forLoop = new For(typeSystem);
            arg = new Arg(typeSystem);
        }

        public MethodDefinition Create(string methodName, TypeReference returnType)
        {
            return new MethodDefinition(methodName, MethodAttributes.Private, returnType);
        }

        public void ForStart(MethodBody methodBody) => forLoop.Start(methodBody);
        public void ForEnd(MethodBody methodBody) => forLoop.End(methodBody, arg.ElementLengthDefinition);
    }
}