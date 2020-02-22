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
        public IReadOnlyCollection<LinqOperator> Operators { get; }

        private TypeReference arg;
        private TypeReference returnType;

        public AnalysedMethod(IReadOnlyCollection<LinqOperator> operators)
        {
            Operators = operators;
        }

        private TypeReference GetArgType()
        {
            var firstOperator = Operators.First();
            var parameterDefinition = firstOperator.NestedMethod.Parameters.First();
            return parameterDefinition.ParameterType;
        }
    }
}