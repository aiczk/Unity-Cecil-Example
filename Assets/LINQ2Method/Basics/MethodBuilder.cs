using System;
using System.Collections.Generic;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace LINQ2Method.Basics
{
    public class MethodBuilder
    {
        internal For MainLoop { get; }

        private ModuleDefinition mainModule;
        private MethodBody methodBody;
        private CacheCollection cacheCollection;
        private Queue<ILinqOperator> operators;
        private TypeReference argType;
        private MethodDefinition method;
        private Arg arg;

        public MethodBuilder(ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            this.mainModule = mainModule;
            cacheCollection = new CacheCollection(systemModule, mainModule);
            operators = new Queue<ILinqOperator>();
            MainLoop = new For(systemModule.TypeSystem);
            arg = new Arg();
        }

        public void Create(TypeDefinition targetClass, string methodName, TypeReference paramsType, TypeReference returnType)
        {
            var iEnumerable = mainModule.ImportReference(typeof(IEnumerable<>)).MakeGenericInstanceType(returnType);
            method = new MethodDefinition(methodName, MethodAttributes.Private, iEnumerable);
            targetClass.Methods.Add(method);
            
            arg.Define(method.Body, paramsType);
            cacheCollection.InitField(targetClass, $"linq_{methodName}", returnType);
            
            methodBody = method.Body;
            argType = paramsType;
        }
        
        public void Begin()
        {
            cacheCollection.Define(methodBody);
            MainLoop.Start(methodBody);
            MainLoop.DefineLocal(methodBody, argType);
        }

        public void End()
        {
            cacheCollection.AddValue(methodBody, MainLoop.LocalDefinition);
            MainLoop.End(methodBody);
            cacheCollection.ReturnValue(methodBody);
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
                    var nextProcess = nextOperator == null ? cacheCollection.Add : nextOperator.Next();
                    
                    linqOperator.Define(methodBody, nextProcess);
                    continue;
                }
                
                linqOperator.Define(methodBody, null);
            }
        }

        public void Replace(MethodDefinition methodDefinition)
        {
            var replacer = new MethodReplacer(methodDefinition);
        }
    }
}