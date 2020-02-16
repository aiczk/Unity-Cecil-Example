using System;
using System.Collections.Generic;
using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Method
    {
        public For MainLoop { get; }
        
        private MethodBody methodBody;
        private Queue<ILinqOperator> operators;
        private TypeReference argType;
        private TypeDefinition classDefinition;
        private MethodDefinition methodDefinition;
        private Arg arg;

        public Method(TypeSystem typeSystem, TypeDefinition classDefinition)
        {
            this.classDefinition = classDefinition;
            operators = new Queue<ILinqOperator>();
            MainLoop = new For(typeSystem);
            arg = new Arg();
        }

        public void Create(string methodName, TypeReference argsType, TypeReference returnType)
        {
            methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private, returnType);
            
            classDefinition.Methods.Add(methodDefinition);
            arg.Define(methodDefinition.Body, argsType);
            
            methodBody = methodDefinition.Body;
            argType = argsType;
        }
        
        public void Begin()
        {
            MainLoop.Start(methodBody);
            MainLoop.DefineLocal(methodBody, argType);
        }

        public void End()
        {
            MainLoop.End(methodBody);
            //todo return value
            InstructionHelper.Return(methodBody);
        }

        public void AddOperator(ILinqOperator linqOperator)
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
                
                if (linqOperator.Type == OperatorType.Jump)
                {
                    var nextOperator = operators.Count > 0 ? operators.Peek() : null;
                    var nextProcess = nextOperator == null ? MainLoop.LoopEnd : nextOperator.Next();
                    
                    linqOperator.Define(methodBody, nextProcess);
                    continue;
                }
                
                linqOperator.Define(methodBody, null);
            }
        }

        private void ResetReturnType(TypeReference argsType, ref TypeReference returnType)
        {
            //returnType = returnType.MakeGenericType(argsType);
        }
    }
}