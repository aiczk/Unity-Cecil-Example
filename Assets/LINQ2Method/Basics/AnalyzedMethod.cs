using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class AnalyzedMethod
    {
        public TypeReference ParameterType => parameterType ??= GetArgType();
        public TypeReference ReturnType => returnType ??= GetReturnType();
        public ReadOnlyCollection<LinqOperator> Operators { get; }

        private TypeReference parameterType;
        private TypeReference returnType;

        public AnalyzedMethod(ReadOnlyCollection<LinqOperator> operators)
        {
            Operators = operators;
        }

        private TypeReference GetArgType()
        {
            var firstOperator = Operators.First();
            var parameterDefinition = firstOperator.NestedMethod.Parameters[0];
            return parameterDefinition.ParameterType;
        }
        
        private TypeReference GetReturnType()
        {
            //todo ハードコーディング
            //select以外にもあるはず。
            var lastOperator = Operators.Last(x => x.Operator == Operator.Select);
            var methodReturnType = lastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}