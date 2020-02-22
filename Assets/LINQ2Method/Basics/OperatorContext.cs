using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class LinqOperator
    {
        public Operator Operator { get; }
        public MethodDefinition NestedMethod { get; }
        
        public LinqOperator(MethodDefinition nestedMethod, Operator @operator)
        {
            NestedMethod = nestedMethod;
            Operator = @operator;
        }
    }
}