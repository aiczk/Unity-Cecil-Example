using System;
using System.Collections.Generic;
using LINQ2Method.Basics;
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

        public static Instruction LdLoc(VariableDefinition definition)
        {
            var ldLoc = OpCodeHelper.LdLoc(definition.Index);
            return ldLoc == OpCodes.Ldloc_S ? Instruction.Create(ldLoc, definition) : Instruction.Create(ldLoc);
        }
        
        public static Instruction LdLoca(VariableDefinition definition)
        {
            if (definition.VariableType.IsValueType)
                return Instruction.Create(OpCodes.Ldloca_S, definition);
            
            var ldLoc = OpCodeHelper.LdLoc(definition.Index);
            return ldLoc == OpCodes.Ldloc_S ? Instruction.Create(ldLoc, definition) : Instruction.Create(ldLoc);
        }

        public static Instruction StLoc(VariableDefinition definition)
        {
            var stLoc = OpCodeHelper.StLoc(definition.Index);
            return stLoc.Equals(OpCodes.Stloc_S) ? Instruction.Create(stLoc, definition) : Instruction.Create(stLoc);
        }
        
        public static Instruction LdElem(TypeReference typeReference)
        {
            var ldElem = OpCodeHelper.LdElem(typeReference);
            return Instruction.Create(ldElem);
        }

        public static Instruction LdArg(int argIndex)
        {
            var ldArg = OpCodeHelper.LdArg(argIndex);
            return ldArg.Equals(OpCodes.Ldarg_S) ? Instruction.Create(ldArg, argIndex) : Instruction.Create(ldArg);
        }
        
        public static VariableDefinition AddVariable(this MethodBody methodBody, TypeReference reference)
        {
            var index = methodBody.Variables.Count;
            var variable = new VariableDefinition(reference);
            methodBody.Variables.Add(variable);
            return variable;
        }
        
        public static Instruction[] FuncConvert(MethodBody funcMethod, For forLoop)
        {
            var size = funcMethod.Instructions.Count - 1;
            var result = new Instruction[size];
            var instructions = funcMethod.Instructions;

            for (var i = 0; i < size; i++)
            {
                ref var res = ref result[i];
                var instruction = instructions[i];
                var opCode = instruction.OpCode;

                if (opCode == OpCodes.Ldarg_1 || opCode == OpCodes.Ldarga_S)
                {
                    res = InstructionHelper.LdLoca(forLoop.LocalDefinition);
                    continue;
                }
                
                if (opCode == OpCodes.Ret)
                    continue;

                res = instruction;
            }
            
            return result;

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
