using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Helpers
{
    public static class InstructionHelper
    {
        public static Instruction LdcI4(int loopCount)
        {
            var ldcI4 = OpCodeHelper.LdcI4(loopCount);
            return ldcI4.Code switch
            {
                Code.Ldc_I4_S => Instruction.Create(ldcI4, (sbyte) loopCount),
                Code.Ldc_I4 => Instruction.Create(ldcI4, loopCount),
                _ => Instruction.Create(ldcI4)
            };
        }

        public static Instruction LdLoc(int index, VariableDefinition variable)
        {
            var ldLoc = OpCodeHelper.LdLoc(index);
            return ldLoc == OpCodes.Ldloc_S ? Instruction.Create(ldLoc, variable) : Instruction.Create(ldLoc);
        }

        public static Instruction StLoc(int index, VariableDefinition variable)
        {
            var stLoc = OpCodeHelper.StLoc(index);
            return stLoc.Equals(OpCodes.Stloc_S) ? Instruction.Create(stLoc, variable) : Instruction.Create(stLoc);
        }
        
        public static (int, VariableDefinition) AddVariable(this MethodBody methodBody, TypeReference reference)
        {
            var index = methodBody.Variables.Count;
            var variable = new VariableDefinition(reference);
            methodBody.Variables.Add(variable);
            return (index, variable);
        }

        public static T OperandValue<T>(Instruction instruction)
        where T : struct
        {
            T result;

            try
            {
                result = (T) instruction.Operand;
            }
            catch (InvalidCastException e)
            {
                if (e.Source != null)
                    Debug.LogError($"Cast Error! :{e.Source}");
                
                throw;
            }

            return result;
        }

        public static void Return(MethodBody methodBody) => methodBody.GetILProcessor().Emit(OpCodes.Ret);
    }
}
