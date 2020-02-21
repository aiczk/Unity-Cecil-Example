using Mono.Cecil;
using Mono.Collections.Generic;

namespace LINQ2Method.Basics
{
    public class LinqOperator
    {
        public Operator Operator { get; }
        public MethodDefinition NestedFunction { get; }
        
        public LinqOperator(Operator @operator, MethodDefinition nestedFunction)
        {
            Operator = @operator;
            NestedFunction = nestedFunction;
        }
    }
}