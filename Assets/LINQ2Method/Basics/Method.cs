using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Method
    {
        public For ForLoop { get; }
        public MethodBody Body { get; private set; }

        private TypeSystem typeSystem;
        private TypeReference paramType;
        private TypeDefinition classDefinition;
        private MethodDefinition methodDefinition;
        private Arg arg;

        public Method(TypeSystem typeSystem, TypeDefinition classDefinition)
        {
            this.typeSystem = typeSystem;
            this.classDefinition = classDefinition;
            ForLoop = new For(typeSystem);
            arg = new Arg();
        }

        public void Create(string methodName, TypeReference argType, TypeReference returnType)
        {
            methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private, returnType);
            classDefinition.Methods.Add(methodDefinition);
            Body = methodDefinition.Body;
            paramType = argType;
        }

        public void Start()
        {
            arg.Define(Body, paramType);
            ForLoop.Start(Body);
            ForLoop.CreateLocal(Body, paramType);
        }

        public void End()
        {
            ForLoop.End(Body);
            
            //todo return value
            InstructionHelper.Return(Body);
        }
    }
}