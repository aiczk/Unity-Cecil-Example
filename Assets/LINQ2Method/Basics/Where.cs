using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Where
    {
        private TypeSystem typeSystem;
        
        public Where(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }
        
        public void Define(MethodBody methodBody, MethodBody funcMethod, For forLoop, Instruction nextProcess)
        {
            var checkVariable = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            
            foreach (var instruction in InstructionHelper.FuncConvert(funcMethod, forLoop))
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            
            //false continue
            //processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
    }
}