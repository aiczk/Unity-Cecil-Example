using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LINQ2Method.Helpers
{
    public static class Linq2MethodHelper
    {
        public static Instruction LdcI4(sbyte loopCount)
        {
            var ldcI4 = OpCodeHelper.LdcI4(loopCount);
            return ldcI4.Equals(OpCodes.Ldc_I4_S) ? Instruction.Create(ldcI4, loopCount) : Instruction.Create(ldcI4);
        }

        public static (Instruction stLoc, Instruction ldLoc) LocalInstructions(int index)
        {
            var ldLoc = LdLoc(index);
            var stLoc = StLoc(index);
            return (stLoc, ldLoc);
        }
        
        public static Instruction LdLoc(int index)
        {
            var ldLoc = OpCodeHelper.LdLoc(index);
            return ldLoc.Equals(OpCodes.Ldloc_S) ? Instruction.Create(ldLoc, index) : Instruction.Create(ldLoc);
        }

        public static Instruction StLoc(int index)
        {
            var stLoc = OpCodeHelper.StLoc(index);
            return stLoc.Equals(OpCodes.Stloc_S) ? Instruction.Create(stLoc, index) : Instruction.Create(stLoc);
        }
        
        public static void AddVariables(this MethodBody methodBody, params TypeReference[] references)
        {
            var variables = methodBody.Variables;
            foreach (var reference in references)
            {
                variables.Add(new VariableDefinition(reference));
            }
        }
        
        public static T Operand<T>(Instruction instruction)
        where T : struct
        {
            T result;

            try
            {
                result = (T) instruction.Operand;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Cast Error.");
                throw;
            }

            return result;
        }
    }
}
