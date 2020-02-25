using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class MethodReplacer
    {
        private MethodDefinition method;
        
        public MethodReplacer(MethodDefinition method)
        {
            this.method = method;
        }

        public void RemoveSection()
        {
            var ldLocIndex = -1;
            var stLocIndex = -1;
            
            var methodBody = method.Body;

            for (var i = 0; i < methodBody.Instructions.Count; i++)
            {
                var instruction = methodBody.Instructions[i];
                var opCode = instruction.OpCode;

                if (opCode == OpCodes.Ldloc_0 || opCode == OpCodes.Ldloc_1 || 
                    opCode == OpCodes.Ldloc_2 || opCode == OpCodes.Ldloc_3 ||
                    opCode == OpCodes.Ldloc_S)
                {
                    var next = instruction.Next;

                    if (next.OpCode != OpCodes.Ldsfld)
                        continue;

                    ldLocIndex = i;
                }

                if (opCode == OpCodes.Call)
                {
                    var next = instruction.Next;

                    if (next.OpCode != OpCodes.Stloc_0 && next.OpCode != OpCodes.Stloc_1 &&
                        next.OpCode != OpCodes.Stloc_2 && next.OpCode != OpCodes.Stloc_3 &&
                        next.OpCode != OpCodes.Stloc_S) 
                        continue;
                    
                    stLocIndex = i;
                }
            }

            for (var i = ldLocIndex; i < stLocIndex; i++) 
                methodBody.Instructions.RemoveAt(i);
        }
    }
}