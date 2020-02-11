﻿using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class Where : ILinqOperator
    {
        private For forLoop;
        private MethodBody funcMethod;
        private TypeSystem typeSystem;
        private Instruction[] converted;
        
        public Where(TypeSystem typeSystem, MethodDefinition funcMethod, For forLoop)
        {
            this.typeSystem = typeSystem;
            this.funcMethod = funcMethod.Body;
            this.forLoop = forLoop;
        }

        public Instruction Next()
        {
            converted = InstructionHelper.FuncConvert(funcMethod, forLoop);
            return converted[0];
        }
        
        public void Define(MethodBody method, Instruction jumpInstruction)
        {
            var checkVariable = method.AddVariable(typeSystem.Boolean);
            var processor = method.GetILProcessor();

            if (converted == null)
                converted = InstructionHelper.FuncConvert(funcMethod, forLoop);
            
            foreach (var instruction in converted)
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, jumpInstruction);
            //continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
    }
}