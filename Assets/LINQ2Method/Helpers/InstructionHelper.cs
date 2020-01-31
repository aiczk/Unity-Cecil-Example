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

        public static Instruction LdLoc(int index)
        {
            var ldLoc = OpCodeHelper.LdLoc(index);
            return ldLoc == OpCodes.Ldloc_S ? Instruction.Create(ldLoc, index) : Instruction.Create(ldLoc);
        }

        public static Instruction StLoc(int index)
        {
            var stLoc = OpCodeHelper.StLoc(index);
            return stLoc.Equals(OpCodes.Stloc_S) ? Instruction.Create(stLoc, index) : Instruction.Create(stLoc);
        }
        
        public static int AddVariable(this MethodBody methodBody, TypeReference reference)
        {
            var index = methodBody.Variables.Count;
            methodBody.Variables.Add(new VariableDefinition(reference));
            return index;
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

        public static List<Instruction> Convert(MethodBody methodBody, MethodBody funcMethod)
        {
            var result = new List<Instruction>();
            TypeReference param1 = null;
            var index = 0;

            //todo cache
            foreach (var instruction in funcMethod.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldarg_1)
                {
                    if (param1 == null)
                    {
                        param1 = funcMethod.Method.Parameters[0].ParameterType;
                        index = methodBody.AddVariable(param1);
                    }
                    
                    result.Add(LdLoc(index));
                    continue;
                }

                result.Add(instruction);
            }

            var temporary = new List<Instruction>(result);
            
            foreach (var instruction in temporary)
            {
                var opCode = instruction.OpCode;

                if (opCode != OpCodes.Ldarg_1 && opCode != OpCodes.Ret) 
                    continue;
                
                result.Remove(instruction);
            }
            
            return result;
        }
    }
}
