using Boo.Lang;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class If
    {
        private TypeSystem typeSystem;
        
        public If(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }
        
        public void Define(MethodBody methodBody, MethodBody funcMethod, For forLoop, Instruction nextProcess)
        {
            var (checkIndex, variable) = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            
            foreach (var instruction in Convert(methodBody, funcMethod))
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkIndex, variable));
            processor.Append(InstructionHelper.LdLoc(checkIndex, variable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            
            //false continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
        
        private List<Instruction> Convert(MethodBody methodBody, MethodBody funcMethod)
        {
            var result = new List<Instruction>();
            TypeReference param1 = null;
            Instruction type = null;

            foreach (var instruction in funcMethod.Instructions)
            {
                var opCode = instruction.OpCode;
                
                if (opCode == OpCodes.Ldarg_1)
                {
                    if (param1 == null)
                    {
                        param1 = funcMethod.Method.Parameters[0].ParameterType;
                        var (index, variable) = methodBody.AddVariable(param1);
                        type = InstructionHelper.LdLoc(index, variable);
                    }
                    
                    //ldLoc前にはstLocが欲しい
                    result.Add(type);
                    continue;
                }
                
                if(opCode == OpCodes.Ret)
                    continue;

                result.Add(instruction);
            }
            
            return result;
        }
    }
}