using Mono.Cecil.Cil;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LINQ2Method.Helpers
{
    public static class Linq2MethodHelper
    {
        public static Instruction ForOpCode(sbyte loopCount)
        {
            var opCode = LoadLength(loopCount);
            return opCode.Equals(OpCodes.Ldc_I4_S) ? Instruction.Create(opCode, loopCount) : Instruction.Create(opCode);
        }

        public static OpCode LoadLength(sbyte length)
        {
            return length switch
            {
                0 => OpCodes.Ldc_I4_0,
                1 => OpCodes.Ldc_I4_1,
                2 => OpCodes.Ldc_I4_2,
                3 => OpCodes.Ldc_I4_3,
                4 => OpCodes.Ldc_I4_4,
                5 => OpCodes.Ldc_I4_5,
                6 => OpCodes.Ldc_I4_6,
                7 => OpCodes.Ldc_I4_7,
                8 => OpCodes.Ldc_I4_8,
                _ => OpCodes.Ldc_I4_S
            };
        }
    }
}
