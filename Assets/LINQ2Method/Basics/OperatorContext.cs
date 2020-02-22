using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class LinqOperator
    {
        public Operator Operator { get; }
        public MethodDefinition InnerMethod { get; }
        
        public LinqOperator(MethodDefinition innerMethod, Operator @operator)
        {
            InnerMethod = innerMethod;
            Operator = @operator;
        }
    }
}