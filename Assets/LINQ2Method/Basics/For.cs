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
        
        private Instruction loopStart;
        private Instruction loopCheck;
        private TypeSystem typeSystem;
        private VariableDefinition indexVariable;
        private int forIndex;

        public For(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            
            loopStart = Instruction.Create(OpCodes.Nop);
            LoopEnd = Instruction.Create(OpCodes.Nop);
        }

        public void Start(MethodBody methodBody, int initValue = 0)
        {
            (forIndex,indexVariable) = methodBody.AddVariable(typeSystem.Int32);
            loopCheck = InstructionHelper.LdLoc(forIndex, indexVariable);
            IncrementIndex = InstructionHelper.LdLoc(forIndex, indexVariable);
            var processor = methodBody.GetILProcessor();

            //i = n
            processor.Append(InstructionHelper.LdcI4(initValue));
            processor.Append(InstructionHelper.StLoc(forIndex, indexVariable));
            
            //i < n check
            processor.Emit(OpCodes.Br_S, loopCheck);
            
            //loop start
            processor.Append(loopStart);
        }
        
        public void End(MethodBody methodBody, int loopCount)
        {
            var (withInIndex, withInVariable) = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            //loop end
            processor.Append(LoopEnd);
            
            //i++
            processor.Append(IncrementIndex);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Add);
            processor.Append(InstructionHelper.StLoc(forIndex, indexVariable));

            //i < n
            processor.Append(loopCheck);
            processor.Append(InstructionHelper.LdcI4(loopCount));
            processor.Emit(OpCodes.Clt);
            processor.Append(InstructionHelper.StLoc(withInIndex, withInVariable));
            
            //check within range
            processor.Append(InstructionHelper.LdLoc(withInIndex, withInVariable));
            processor.Emit(OpCodes.Brtrue_S, loopStart);
        }
    }
}
