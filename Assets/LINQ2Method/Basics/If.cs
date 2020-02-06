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
            var checkVariable = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            
            foreach (var instruction in Convert(methodBody, funcMethod, forLoop))
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            
            //false continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
        
        private List<Instruction> Convert(MethodBody methodBody, MethodBody funcMethod, For forLoop)
        {
            var result = new List<Instruction>();
            TypeReference param1 = null;
            Variable variable = null;

            foreach (var instruction in funcMethod.Instructions)
            {
                var opCode = instruction.OpCode;
                
                if (opCode == OpCodes.Ldarg_1)
                {
                    if (param1 == null)
                    {
                        param1 = funcMethod.Method.Parameters[0].ParameterType;
                        variable = methodBody.AddVariable(param1);
                    }
                    
                    result.Add(Instruction.Create(OpCodes.Ldarg_1));
                    result.Add(InstructionHelper.LdLoc(forLoop.IndexVariable));
                    result.Add(Instruction.Create(OpCodes.Ldelem_Ref));
                    result.Add(InstructionHelper.StLoc(variable));
                    result.Add(InstructionHelper.LdLoc(variable));
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