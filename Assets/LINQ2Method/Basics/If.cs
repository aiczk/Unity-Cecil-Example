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
        
        public void Create(MethodBody methodBody, MethodBody funcMethod, For forLoop, Instruction nextProcess)
        {
            var checkIndex = methodBody.AddVariable(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            
            foreach (var instruction in InstructionHelper.Convert(methodBody, funcMethod))
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkIndex));
            processor.Append(InstructionHelper.LdLoc(checkIndex));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, nextProcess);
            
            //false continue
            processor.Emit(OpCodes.Br_S, forLoop.Increment);
        }
    }
}