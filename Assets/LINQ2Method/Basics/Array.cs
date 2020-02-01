using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class Array
    {
        private TypeSystem typeSystem;
        
        public Array(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public void Create(MethodBody methodBody, TypeReference arrayType)
        {
            methodBody.Method.Parameters.Add(new ParameterDefinition(new ArrayType(arrayType)));
            var (lengthIndex, variable) = methodBody.AddVariable(typeSystem.Int32);

            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Ldarg_1);
            processor.Emit(OpCodes.Ldlen);
            processor.Emit(OpCodes.Conv_I4);
            processor.Append(InstructionHelper.StLoc(lengthIndex, variable));
        }
    }
}
