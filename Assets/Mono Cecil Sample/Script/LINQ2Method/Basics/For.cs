using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable once CheckNamespace
namespace LINQ2Method.Basics
{
    public class For
    {
        private TypeSystem typeSystem;
        private Instruction loopStart;
        private Instruction loopCheck;
        private int variablesCount;

        public For(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            
            loopStart = Instruction.Create(OpCodes.Nop);
        }

        public void Start(MethodBody methodBody, sbyte initValue = 0)
        {
            variablesCount = methodBody.Variables.Count;
            loopCheck = Linq2MethodHelper.LdLoc(variablesCount);
            
            methodBody.AddVariables(typeSystem.Int32, typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            //i = n
            processor.Append(Linq2MethodHelper.LdcI4(initValue));
            processor.Append(Linq2MethodHelper.StLoc(variablesCount));
            
            //i < n check
            processor.Emit(OpCodes.Br_S, loopCheck);
            
            //loop start
            processor.Append(loopStart);
        }

        public void End(MethodBody methodBody, sbyte loopCount)
        {
            var processor = methodBody.GetILProcessor();
            
            //loop end
            processor.Emit(OpCodes.Nop);

            //i++
            processor.Append(Linq2MethodHelper.LdLoc(variablesCount));
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Add);
            processor.Append(Linq2MethodHelper.StLoc(variablesCount));

            //i < n
            processor.Append(loopCheck);
            processor.Append(Linq2MethodHelper.LdcI4(loopCount));
            processor.Emit(OpCodes.Clt);
            processor.Append(Linq2MethodHelper.StLoc(variablesCount + 1));
            
            //trueならばloopを最初のnop命令に戻す
            processor.Append(Linq2MethodHelper.LdLoc(variablesCount + 1));
            processor.Emit(OpCodes.Brtrue_S, loopStart);
            
            processor.Emit(OpCodes.Ret);
        }
    }
}
