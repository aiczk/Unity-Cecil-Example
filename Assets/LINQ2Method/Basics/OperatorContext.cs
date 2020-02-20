using Mono.Cecil;

namespace LINQ2Method.Basics
{
    public class OperatorContext
    {
        public MethodDefinition Function { get; }
        public Operator OperatorType { get; }

        public OperatorContext(MethodDefinition function, Operator operatorType)
        {
            Function = function;
            OperatorType = operatorType;
        }
    }

}