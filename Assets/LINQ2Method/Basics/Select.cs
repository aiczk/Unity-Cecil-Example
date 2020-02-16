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
        OperatorType ILinqOperator.Type => OperatorType.None;
        
        public Select(MethodDefinition funcMethod, For forLoop)
        {
            this.funcMethod = funcMethod.Body;
            this.forLoop = forLoop;
        }

        Instruction ILinqOperator.Next()
        {
            var returnType = funcMethod.Method.ReturnType;
            var variableType = forLoop.LocalDefinition.VariableType;
            if (returnType.Name != variableType.Name)
            {
                ldLoca = InstructionHelper.LdLoca(forLoop.LocalDefinition);
                return ldLoca;
            }
            
            converted = Convert(funcMethod);
            converted[0] = InstructionHelper.LdLoc(forLoop.LocalDefinition);

            return converted[0];
        }

        void ILinqOperator.Define(MethodBody method, Instruction jumpInstruction)
        {
            var processor = method.GetILProcessor();
            
            if (ldLoca != null)
            {
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
            var size = funcMethod.Instructions.Count - 1;
            var result = new Instruction[size];
            var instructions = funcMethod.Instructions;
            
            for (var i = 0; i < size; i++)
            {
                var instruction = instructions[i];
                var opCode = instruction.OpCode;

                if (opCode == OpCodes.Ldarg_1 || opCode == OpCodes.Ldarga_S)
                    continue;

                if (opCode == OpCodes.Ret)
                    continue;

                result[i] = instruction;
            }
            
            return result;
        }

    }
}