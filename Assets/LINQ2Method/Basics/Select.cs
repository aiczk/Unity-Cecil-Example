using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Select
    {
        public Instruction Next { get; private set; }

        public void Define(MethodBody methodBody, MethodBody funcMethod, For forLoop)
        {
            var processor = methodBody.GetILProcessor();
            foreach (var instruction in InstructionHelper.FuncConvert(funcMethod, forLoop))
            {
                processor.Append(instruction);
            }
            processor.Append(InstructionHelper.StLoc(forLoop.LocalDefinition));
        }
    }
}