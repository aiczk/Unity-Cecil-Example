using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Where
    {
        private For forLoop;
        private TypeSystem typeSystem;
        private Instruction[] converted;
        
        public Where(TypeSystem typeSystem, For forLoop)
        {
            this.typeSystem = typeSystem;
            this.forLoop = forLoop;
        }

        public Instruction Next(MethodBody funcMethod)
        {
            converted = InstructionHelper.FuncConvert(funcMethod, forLoop);
            return converted[0];
        }
        public void Define(MethodBody methodBody, MethodBody funcMethod, Instruction nextProcess)
        {
            var checkVariable = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            if (converted == null)
                converted = InstructionHelper.FuncConvert(funcMethod, forLoop);
            
            foreach (var instruction in converted)
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            //processor.Emit(OpCodes.Brfalse_S, forLoop.LoopEnd);
            
            //false continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
    }
}