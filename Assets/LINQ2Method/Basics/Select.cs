using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Select
    {
        public void Define(MethodBody methodBody, MethodBody funcMethod, For forLoop)
        {
            var processor = methodBody.GetILProcessor();

            var returnType = funcMethod.Method.ReturnType;
            
            if (!returnType.Name.Equals(forLoop.LocalDefinition.VariableType.Name))
            {
                processor.Append(InstructionHelper.LdLoca(forLoop.LocalDefinition));
                forLoop.LocalDefinition = methodBody.AddVariable(returnType);
            }

            foreach (var instruction in Convert(funcMethod))
            {
                if(instruction == null)
                    continue;
                
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(forLoop.LocalDefinition));
        }

        private static Instruction[] Convert(MethodBody funcMethod)
        {
            var size = funcMethod.Instructions.Count;
            var result = new Instruction[size];
            var instructions = funcMethod.Instructions;
            

            for (var i = 0; i < size; i++)
            {
                ref var res = ref result[i];
                var instruction = instructions[i];
                var opCode = instruction.OpCode;

                if (opCode == OpCodes.Ldarg_1 || opCode == OpCodes.Ldarga_S)
                    continue;

                if (opCode == OpCodes.Ret)
                    continue;

                res = instruction;
            }
            
            return result;

        }

    }
}