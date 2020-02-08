using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Helpers
{
    public static class OpCodeHelper
    {
        public static OpCode LdcI4(int length)
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
                var len when len <= sbyte.MaxValue && len >= sbyte.MinValue => OpCodes.Ldc_I4_S,
                _ => OpCodes.Ldc_I4,
            };
        }
        
        public static OpCode LdLoc(int index)
        {
            return index switch
            {
                0 => OpCodes.Ldloc_0,
                1 => OpCodes.Ldloc_1,
                2 => OpCodes.Ldloc_2,
                3 => OpCodes.Ldloc_3,
                _ => OpCodes.Ldloc_S
            };
        }

        public static OpCode StLoc(int index)
        {
            return index switch
            { 
                0 => OpCodes.Stloc_0,
                1 => OpCodes.Stloc_1,
                2 => OpCodes.Stloc_2,
                3 => OpCodes.Stloc_3,
                _ => OpCodes.Stloc_S
            };
        }
        
        public static OpCode LdElem(TypeReference arg)
        {
            return arg.Name switch
            {
                nameof(SByte) => OpCodes.Ldelem_I1,
                nameof(Int16) => OpCodes.Ldelem_I2,
                nameof(Int32) => OpCodes.Ldelem_I4,
                nameof(Int64) => OpCodes.Ldelem_I8,
                nameof(Byte) => OpCodes.Ldelem_U1,
                nameof(UInt16) => OpCodes.Ldelem_U2,
                nameof(UInt32) => OpCodes.Ldelem_U4,
                nameof(Single) => OpCodes.Ldelem_R4,
                nameof(Double) => OpCodes.Ldelem_R8,
                _ => OpCodes.Ldelem_Ref
            };
        }

        public static OpCode LdArg(int argIndex)
        {
            return argIndex switch
            {
                0 => OpCodes.Ldarg_0,
                1 => OpCodes.Ldarg_1,
                2 => OpCodes.Ldarg_2,
                3 => OpCodes.Ldarg_3,
                _ => OpCodes.Ldarg_S,
            };
        }
    }
}