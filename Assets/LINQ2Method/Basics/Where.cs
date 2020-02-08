using System.Collections.Generic;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using UnityEngine;

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
            
            foreach (var instruction in Convert(funcMethod, forLoop))
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            
            //false continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
        
        private static Instruction[] Convert(MethodBody funcMethod, For forLoop)
        {
            var size = funcMethod.Instructions.Count - 1;
            var instructions = new Instruction[size];

            for (var i = 0; i < size; i++)
            {
                ref var result = ref instructions[i];
                var instruction = funcMethod.Instructions[i];
                var opCode = instruction.OpCode;
                
                if (opCode == OpCodes.Ret)
                    continue;

                if (opCode == OpCodes.Ldarg_1 || opCode == OpCodes.Ldarga_S)
                {
                    result = InstructionHelper.LdLoca(forLoop.LocalDefinition);
                    continue;
                }

                result = instruction;
            }
            
            return instructions;
        }
    }
}