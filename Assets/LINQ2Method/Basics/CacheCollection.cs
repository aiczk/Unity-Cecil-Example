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
        private ModuleDefinition systemModule;
        private ModuleDefinition mainModule;

        private FieldDefinition collection;
        private GenericInstanceType iEnumerable;
        
        private MethodReference getCount;
        private MethodReference clear;

        public CacheCollection(ModuleDefinition systemModule, ModuleDefinition mainModule)
        {
            this.systemModule = systemModule;
            this.mainModule = mainModule;
            typeSystem = systemModule.TypeSystem;
            
            var methods = systemModule.GetType("System.Collections.ObjectModel", "Collection`1").Methods;
            getCount = mainModule.ImportReference(methods.Single(x => x.Name == "get_Count"));
            clear = mainModule.ImportReference(methods.Single(x => x.Name == "Clear"));
        }

        public void InitField(TypeDefinition targetClass, string fieldName, TypeReference argument)
        {
            var collectionInstance = Import("System.Collections.ObjectModel", "Collection`1").MakeGenericInstanceType(argument);
            iEnumerable = Import("System.Collections.Generic", "IEnumerable`1").MakeGenericInstanceType(argument);

            collection = new FieldDefinition(fieldName, FieldAttributes.Private, collectionInstance);
            targetClass.Fields.Add(collection);
        }

        public void Define(MethodBody methodBody)
        {
            var boolean = methodBody.AddVariableDefinition(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            var nop = Instruction.Create(OpCodes.Nop);

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
            var variable = methodBody.AddVariableDefinition(iEnumerable);
            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, collection);
            processor.Append(InstructionHelper.StLoc(variable));

            var fa = InstructionHelper.LdLoc(variable);
            processor.Emit(OpCodes.Br_S, fa);
            processor.Append(fa);
        }

        private TypeReference Import(string nameSpace, string name)
        {
            var type = systemModule.GetType(nameSpace, name);
            var result = mainModule.ImportReference(type);
            
            return result;
        }
    }
}