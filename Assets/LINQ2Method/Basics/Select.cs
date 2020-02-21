using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Select : ILinqOperator
    {
        private MethodBody funcMethod;
        private For forLoop;
        private Instruction[] converted;
        OperatorType ILinqOperator.Type => OperatorType.None;
        
        public Select(MethodDefinition funcMethod, For forLoop)
        {
            this.funcMethod = funcMethod.Body;
            this.forLoop = forLoop;
        }

        Instruction ILinqOperator.Next()
        {
            converted = InstructionHelper.ConvertFunction(funcMethod, forLoop);
            return converted[0];
        }

        void ILinqOperator.Define(MethodBody method, Instruction jumpInstruction)
        {
            var processor = method.GetILProcessor();
            var returnType = funcMethod.Method.ReturnType;
            
            if (!TypeReferenceEquals(returnType, forLoop.LocalDefinition.VariableType))
                forLoop.LocalDefinition = method.AddVariableDefinition(returnType);

            converted ??= InstructionHelper.ConvertFunction(funcMethod, forLoop);
            
            foreach (var instruction in converted)
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(forLoop.LocalDefinition));
        }
        
        private bool TypeReferenceEquals(TypeReference t0, TypeReference t1) => t0.Name == t1.Name;
    }
}