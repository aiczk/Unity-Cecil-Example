using System;
using System.Collections.Generic;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class MethodBuilder
    {
        public For MainLoop { get; }
        
        private MethodBody methodBody;
        private Queue<ILinqOperator> operators;
        private TypeReference argType;
        private MethodDefinition methodDefinition;
        private Arg arg;

        public MethodBuilder(TypeSystem typeSystem)
        {
            operators = new Queue<ILinqOperator>();
            MainLoop = new For(typeSystem);
            arg = new Arg();
        }

        public void Create(TypeDefinition targetClass, string methodName, TypeReference paramsType, TypeReference returnType)
        {
            methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private, returnType);
            
            targetClass.Methods.Add(methodDefinition);
            arg.Define(methodDefinition.Body, paramsType);
            
            methodBody = methodDefinition.Body;
            argType = paramsType;
        }
        
        public void Begin()
        {
            MainLoop.Start(methodBody);
            MainLoop.DefineLocal(methodBody, argType);
        }

        public void End()
        {
            MainLoop.End(methodBody);

            InstructionHelper.Return(methodBody);
        }

        public void AppendOperator(ILinqOperator linqOperator)
        {
            if(linqOperator == null)
                throw new NullReferenceException();
            
            operators.Enqueue(linqOperator);
        }

        public void BuildOperator()
        {
            var count = operators.Count;
            for (var i = 0; i < count; i++)
            {
                var linqOperator = operators.Dequeue();
                
                if (linqOperator.Type == JumpType.Jump)
                {
                    var nextOperator = operators.Count > 0 ? operators.Peek() : null;
                    var nextProcess = nextOperator == null ? MainLoop.LoopEnd : nextOperator.Next();
                    
                    linqOperator.Define(methodBody, nextProcess);
                    continue;
                }
                
                linqOperator.Define(methodBody, null);
            }
        }
    }
}