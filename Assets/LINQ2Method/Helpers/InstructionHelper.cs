using LINQ2Method.Basics;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable once ReturnTypeCanBeEnumerable.Global

namespace LINQ2Method.Helpers
{
    public static class InstructionHelper
    {
        public static Instruction LdcI4(int value)
        {
            var ldcI4 = OpCodeHelper.LdcI4(value);
            return ldcI4.Code switch
            {
                Code.Ldc_I4_S => Instruction.Create(ldcI4, (sbyte) value),
                Code.Ldc_I4 => Instruction.Create(ldcI4, value),
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
        
        public static Instruction LdElem(TypeReference typeReference) => 
            Instruction.Create(OpCodeHelper.LdElem(typeReference));

        public static Instruction LdArg(int argIndex)
        {
            var ldArg = OpCodeHelper.LdArg(argIndex);
            return ldArg.Equals(OpCodes.Ldarg_S) ? Instruction.Create(ldArg, argIndex) : Instruction.Create(ldArg);
        }
        
        public static VariableDefinition AddVariableDefinition(this MethodBody methodBody, TypeReference reference)
        {
            var variable = new VariableDefinition(reference);
            methodBody.Variables.Add(variable);
            return variable;
        }
        
        public static GenericParameter AsGenericParameter(this TypeReference provider) => 
            new GenericParameter(provider);
        
        public static GenericParameter AsGenericParameter(this TypeReference provider, string name) => 
            new GenericParameter(name, provider);

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
                    res = LdLoc(forLoop.LocalDefinition);
                    continue;
                }
                
                if (opCode == OpCodes.Ret)
                    continue;

                res = instruction;
            }
            
            return result;
        }

        public static void Return(MethodBody methodBody) => methodBody.GetILProcessor().Emit(OpCodes.Ret);
    }
}
