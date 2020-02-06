using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LINQ2Method.Basics
{
    public class For
    {
        public Instruction LoopEnd { get; }
        public Instruction IncrementIndex { get; private set; }
        public Variable IndexVariable { get; private set; }
        
        private Instruction loopStart;
        private Instruction loopCheck;
        private TypeSystem typeSystem;

        public For(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            
            loopStart = Instruction.Create(OpCodes.Nop);
            LoopEnd = Instruction.Create(OpCodes.Nop);
        }

        public void Start(MethodBody methodBody, int initValue = 0)
        {
            IndexVariable = methodBody.AddVariable(typeSystem.Int32);
            loopCheck = InstructionHelper.LdLoc(IndexVariable);
            IncrementIndex = InstructionHelper.LdLoc(IndexVariable);
            var processor = methodBody.GetILProcessor();

            //i = n
            processor.Append(InstructionHelper.LdcI4(initValue));
            processor.Append(InstructionHelper.StLoc(IndexVariable));
            
            //i < n check
            processor.Emit(OpCodes.Br_S, loopCheck);
            
            //loop start
            processor.Append(loopStart);
        }
        
        public void End(MethodBody methodBody, Instruction loopCountInstruction)
        {
            var withInVariable = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            //loop end
            processor.Append(LoopEnd);
            
            //i++
            processor.Append(IncrementIndex);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Add);
            processor.Append(InstructionHelper.StLoc(IndexVariable));

            //i < n
            processor.Append(loopCheck);
            processor.Append(loopCountInstruction);
            processor.Emit(OpCodes.Clt);
            processor.Append(InstructionHelper.StLoc(withInVariable));
            
            //check within range
            processor.Append(InstructionHelper.LdLoc(withInVariable));
            processor.Emit(OpCodes.Brtrue_S, loopStart);
        }
    }
}
