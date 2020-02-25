using System.Collections.Generic;
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
            var methodBody = method.Body;
            var list = new List<Instruction>();
            var flag = false;
            foreach (var instruction in methodBody.Instructions)
            {
                var opCode = instruction.OpCode;

                //field
                if (opCode == OpCodes.Ldarg_0)
                {
                    var next = instruction.Next;

                    if (next.OpCode == OpCodes.Ldfld)
                    {
                        flag = true;
                    }
                }

                //new
//                if (opCode == OpCodes.Ldloc_0 || opCode == OpCodes.Ldloc_1 || 
//                    opCode == OpCodes.Ldloc_2 || opCode == OpCodes.Ldloc_3 ||
//                    opCode == OpCodes.Ldloc_S)
//                {
//                    var next = instruction.Next;
//
//                    if (next.OpCode != OpCodes.Ldsfld)
//                    {
//                        flag = true;
//                    }
//                }

                if (opCode == OpCodes.Stloc_0 || opCode == OpCodes.Stloc_1 ||
                    opCode == OpCodes.Stloc_2 || opCode == OpCodes.Stloc_3 ||
                    opCode == OpCodes.Stloc_S)
                {
                    var next = instruction.Previous;

                    if (next.OpCode == OpCodes.Call)
                    {
                        flag = false;
                        list.Add(instruction);
                    }
                    
                }
                
                if(!flag)
                    continue;
                
                list.Add(instruction);
            }

            foreach (var instruction in list)
            {
                methodBody.Instructions.Remove(instruction);
            }
        }

        public void Replace(MethodDefinition callMethod)
        {
            //stloc.Nを覚えておく。
            //
        }
    }

    public enum CallType
    {
        Field,
        Argument,
        Local
    }
}