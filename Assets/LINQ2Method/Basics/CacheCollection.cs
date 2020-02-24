using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class CacheCollection
    {
        private TypeSystem typeSystem;
        private ModuleDefinition moduleDefinition;

        private GenericInstanceType collection;
        private TypeReference genericArgument;
        private MethodDefinition getCount;
        private MethodDefinition clear;
        private Instruction nop; 

        public CacheCollection(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
            typeSystem = moduleDefinition.TypeSystem;
            
            var methods = moduleDefinition.GetType("System.Collections.ObjectModel", "Collection`1").Methods;
            getCount = methods.Single(x => x.Name == "get_Count");
            clear = methods.Single(x => x.Name == "Clear");
            nop = Instruction.Create(OpCodes.Nop);
        }

        public void InitField(TypeDefinition classDefinition, TypeReference argument)
        {
            collection = moduleDefinition.GenericInstanceType(typeof(Collection<>), argument);
            var field = new FieldDefinition("linq_collection", FieldAttributes.Private, collection);
            classDefinition.Fields.Add(field);
            genericArgument = argument;
        }

        public void Define(MethodBody methodBody)
        {
            var boolean = methodBody.AddVariableDefinition(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            //if(collection.Count < 0)
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, collection);
            
            //get_count()
            processor.Emit(OpCodes.Callvirt, getCount);
            processor.Emit(OpCodes.Ldc_I4_0);
            processor.Emit(OpCodes.Clt);
            processor.Append(InstructionHelper.StLoc(boolean));
            processor.Append(InstructionHelper.LdLoc(boolean));
            processor.Emit(OpCodes.Brfalse_S, nop);
            
            //collection.Clear();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, collection);
            
            //clear()
            processor.Emit(OpCodes.Callvirt, clear);
            
            processor.Emit(OpCodes.Nop);
            processor.Append(nop);
        }

        public void ReturnValue(MethodBody methodBody)
        {
            var iEnumerable = moduleDefinition.GenericInstanceType(typeof(IEnumerable<>), genericArgument);
            var variable = methodBody.AddVariableDefinition(iEnumerable);
            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, collection);
            processor.Append(InstructionHelper.StLoc(variable));
            processor.Emit(OpCodes.Br_S);
            processor.Append(InstructionHelper.LdLoc(variable));
        }
    }
}