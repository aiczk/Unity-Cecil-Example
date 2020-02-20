using System.Collections.Generic;
using Mono.Cecil;

namespace LINQ2Method.Basics
{
    public class OperatorContext
    {
        public MethodDefinition Method { get; }
        public Operator Operator { get; }

        public OperatorContext(MethodDefinition method, Operator @operator)
        {
            Method = method;
            Operator = @operator;
        }
    }
}