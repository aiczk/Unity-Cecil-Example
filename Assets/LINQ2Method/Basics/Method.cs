using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Method
    {
        private TypeDefinition classDefinition;
        private MethodDefinition methodDefinition;
        private MethodBody methodBody;
        private For forLoop;
        private Arg arg;

        public Method(TypeSystem typeSystem, TypeDefinition classDefinition)
        {
            this.classDefinition = classDefinition;
            forLoop = new For(typeSystem);
            arg = new Arg(typeSystem);
        }

        public void Create(string methodName, TypeReference returnType)
        {
            methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private, returnType);
            classDefinition.Methods.Add(methodDefinition);
            methodBody = methodDefinition.Body;
        }

        public void Start(TypeReference argType)
        {
            arg.Define(methodBody, argType);
            forLoop.Start(methodBody);
            forLoop.CreateLocal(methodBody, argType);
        }

        public void End()
        {
            forLoop.End(methodBody);
            InstructionHelper.Return(methodBody);
        }
    }
}