using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Select : ILinqOperator
    {
        private For forLoop;
        private Instruction ldLoca;
        private Instruction[] converted;
        
        public Select(For forLoop)
        {
            this.forLoop = forLoop;
        }

        public Instruction Next(MethodBody funcMethod)
        {
            var returnType = funcMethod.Method.ReturnType;
            if (returnType.Name != forLoop.LocalDefinition.VariableType.Name)
            {
                ldLoca = InstructionHelper.LdLoca(forLoop.LocalDefinition);
                return ldLoca;
            }
            
            converted = Convert(funcMethod);
            return converted[0];
        }

        public void Define(MethodBody methodBody, MethodBody funcMethod)
        {
            var processor = methodBody.GetILProcessor();
            
            if (ldLoca != null)
            {
                processor.Append(ldLoca);
                forLoop.LocalDefinition = methodBody.AddVariable(funcMethod.Method.ReturnType);
            }
            
            if (converted == null)
                converted = Convert(funcMethod);

            foreach (var instruction in converted)
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