using System.Collections.Generic;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Method
    {
        public For ForLoop { get; }
        public MethodBody Body { get; private set; }

        private Queue<(Operator, ILinqOperator)> operators;
        private TypeSystem typeSystem;
        private TypeReference paramType;
        private TypeDefinition classDefinition;
        private MethodDefinition methodDefinition;
        private Arg arg;

        public Method(TypeSystem typeSystem, TypeDefinition classDefinition)
        {
            this.typeSystem = typeSystem;
            this.classDefinition = classDefinition;
            operators = new Queue<(Operator, ILinqOperator)>();
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

        public void AddOperator(Operator linq, ILinqOperator linqOperator)
        {
            operators.Enqueue((linq, linqOperator));
        }

        public void Build()
        {
            var loop = operators.Count;
            for (var i = 0; i < loop; i++)
            {
                var (linq, linqOperator) = operators.Dequeue();

                if (linq == Operator.Where)
                {
                    var peek = operators.Peek();
                    linqOperator.Define(Body, peek.Item2.Next());
                    continue;
                }

                linqOperator.Define(Body, null);
            }
        }
    }
}