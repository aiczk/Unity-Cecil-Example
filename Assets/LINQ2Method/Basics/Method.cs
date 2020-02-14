using System.Collections.Generic;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Method
    {
        public For ForLoop { get; }
        public MethodBody Body { get; private set; }

        private Queue<ILinqOperator> operators;
        private TypeReference argType;
        private TypeDefinition classDefinition;
        private MethodDefinition methodDefinition;
        private Arg arg;

        public Method(TypeSystem typeSystem, TypeDefinition classDefinition)
        {
            this.classDefinition = classDefinition;
            operators = new Queue<ILinqOperator>();
            ForLoop = new For(typeSystem);
            arg = new Arg();
        }

        public void Create(string methodName, TypeReference argsType, TypeReference returnType)
        {
            //ResetReturnType(argsType, ref returnType);
            methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private, returnType);

            classDefinition.Methods.Add(methodDefinition);
            arg.Define(methodDefinition.Body, argsType);
            
            Body = methodDefinition.Body;
            argType = argsType;
        }

        public void Begin()
        {
            ForLoop.Start(Body);
            ForLoop.DefineLocal(Body, argType);
        }

        public void End()
        {
            ForLoop.End(Body);
            //todo return value
            InstructionHelper.Return(Body);
        }

        public void AddOperator(ILinqOperator linqOperator) => operators.Enqueue(linqOperator);

        public void Build()
        {
            for (var i = 0; i < operators.Count; i++)
            {
                var linqOperator = operators.Dequeue();
                
                if ((linqOperator as Where) != null)
                {
                    ILinqOperator nextOperator = null;
                    
                    if (operators.Count != 0) 
                        nextOperator = operators.Peek();
                    
                    var nextProcess = nextOperator == null ? ForLoop.IncrementIndex : nextOperator.Next();
                    linqOperator.Define(Body, nextProcess);
                    continue;
                }
                
                linqOperator.Define(Body, null);
            }
        }

        private void ResetReturnType(TypeReference argsType, ref TypeReference returnType)
        {
            //returnType = returnType.MakeGenericType(argsType);
        }
    }
}