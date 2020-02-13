using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Select : ILinqOperator
    {
        private MethodBody funcMethod;
        private For forLoop;
        private Instruction ldLoca;
        private Instruction[] converted;
        
        public Select(MethodDefinition funcMethod, For forLoop)
        {
            this.funcMethod = funcMethod.Body;
            this.forLoop = forLoop;
        }

        Instruction ILinqOperator.Next()
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

        void ILinqOperator.Define(MethodBody method, Instruction jumpInstruction)
        {
            var processor = method.GetILProcessor();
            
            if (funcMethod.Method.ReturnType.Name != forLoop.LocalDefinition.VariableType.Name)
            {
                ldLoca ??= InstructionHelper.LdLoca(forLoop.LocalDefinition);
                
                processor.Append(ldLoca);
                forLoop.LocalDefinition = method.AddVariableDefinition(funcMethod.Method.ReturnType);
            }

            converted ??= Convert(funcMethod);
            
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