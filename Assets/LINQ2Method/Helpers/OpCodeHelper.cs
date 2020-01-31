using Mono.Cecil.Cil;

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
                var lng when lng <= 127 && lng >= -128 => OpCodes.Ldc_I4_S,
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
    }
}