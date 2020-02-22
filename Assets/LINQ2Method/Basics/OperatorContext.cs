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

    public class AnalysisResult
    {
        public TypeReference Arg { get; private set; }
        public TypeReference ReturnType { get; private set; }

        private IReadOnlyList<LinqOperator> linqOperators;

        public AnalysisResult(IReadOnlyList<LinqOperator> linqOperators)
        {
            this.linqOperators = linqOperators;
        }

        private TypeReference GetArgType()
        {
            var first = linqOperators[0];
            var type = first.NestedMethod.GenericParameters;

            foreach (var genericParameter in type)
            {
                Debug.Log(genericParameter);
            }

            return default;
        }
    }
}