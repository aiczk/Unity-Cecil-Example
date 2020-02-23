using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class AnalyzedMethod
    {
        public TypeReference ParameterType => parameterType ??= GetArgType();
        public TypeReference ReturnType => returnType ??= GetReturnType();
        public IReadOnlyCollection<LinqOperator> Operators { get; }

        private TypeReference parameterType;
        private TypeReference returnType;

        public AnalyzedMethod(IReadOnlyCollection<LinqOperator> operators)
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
            var lastOperator = Operators.Last();
            var methodReturnType = lastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}