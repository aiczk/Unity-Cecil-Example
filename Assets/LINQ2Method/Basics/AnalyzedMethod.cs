using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace LINQ2Method.Basics
{
    public class AnalyzedMethod
    {
        public TypeReference ParameterType => arg ??= GetArgType();
        public TypeReference ReturnType => returnType;
        public IReadOnlyCollection<LinqOperator> Operators { get; }

        private TypeReference arg;
        private TypeReference returnType;

        public AnalyzedMethod(IReadOnlyCollection<LinqOperator> operators)
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