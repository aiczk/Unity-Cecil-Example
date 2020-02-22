using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class LinqOperator
    {
        public Operator Operator { get; }
        public MethodDefinition NestedMethod { get; }
        
        public LinqOperator(Operator @operator, MethodDefinition nestedMethod)
        {
            Operator = @operator;
            NestedMethod = nestedMethod;
        }
    }

    public class AnalysedMethod
    {
        public TypeReference Parameter => arg ??= GetArgType();
        public TypeReference ReturnType => returnType;
        public IReadOnlyList<LinqOperator> Operators { get; }

        private TypeReference arg;
        private TypeReference returnType;

        public AnalysedMethod(IReadOnlyList<LinqOperator> linqOperators)
        {
            Operators = linqOperators;
        }

        private TypeReference GetArgType()
        {
            var firstOperator = Operators.First();
            var parameterDefinition = firstOperator.NestedMethod.Parameters.First();
            return parameterDefinition.ParameterType;
        }
    }
}