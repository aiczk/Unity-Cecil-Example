using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    //fixme 設計がクソ
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
            converted = InstructionHelper.ConvertFunc(funcMethod, forLoop, Convert);
            
            return converted[0];
        }

        void ILinqOperator.Define(MethodBody method, Instruction jumpInstruction)
        {
            var processor = method.GetILProcessor();

            converted ??= InstructionHelper.ConvertFunc(funcMethod, forLoop, Convert);

            var returnType = funcMethod.Method.ReturnType;
            if (!TypeReferenceEquals(returnType, forLoop.LocalDefinition.VariableType))
            {
                forLoop.LocalDefinition = method.AddVariableDefinition(returnType);
            }
            
            foreach (var instruction in converted)
            {
                if(instruction == null)
                    continue;
                
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(forLoop.LocalDefinition));
        }
        
        private Instruction Convert()
        {
            return TypeReferenceEquals(funcMethod.Method.ReturnType, forLoop.LocalDefinition.VariableType)
                ? InstructionHelper.LdLoca(forLoop.LocalDefinition)
                : InstructionHelper.LdLoc(forLoop.LocalDefinition);
        }

        private bool TypeReferenceEquals(TypeReference t0, TypeReference t1) => t0.Name == t1.Name;
    }
}