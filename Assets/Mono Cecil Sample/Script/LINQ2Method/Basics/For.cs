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

        public For(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            
            loopStart = Instruction.Create(OpCodes.Nop);
            loopCheck = Instruction.Create(OpCodes.Ldloc_0);
        }

        public void Start(MethodBody methodBody)
        {
            methodBody.Variables.Add(new VariableDefinition(typeSystem.Int32));
            methodBody.Variables.Add(new VariableDefinition(typeSystem.Boolean));

            var processor = methodBody.GetILProcessor();

            //i = 0
            processor.Emit(OpCodes.Ldc_I4_0);
            processor.Emit(OpCodes.Stloc_0);
            
            //i < nが条件を満たしているかチェックするためにi < nへジャンプする
            processor.Emit(OpCodes.Br_S, loopCheck);
            
            processor.Append(loopStart);
        }

        public void End(MethodBody methodBody, sbyte loopCount)
        {
            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Nop);

            //i++
            processor.Emit(OpCodes.Ldloc_0);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Add);
            processor.Emit(OpCodes.Stloc_0);

            //i < n
            processor.Append(loopCheck);
            processor.Append(Linq2MethodHelper.ForOpCode(loopCount));
            processor.Emit(OpCodes.Clt);
            processor.Emit(OpCodes.Stloc_1);
            
            //trueならばloopを最初のnop命令に戻す
            processor.Emit(OpCodes.Ldloc_1);
            processor.Emit(OpCodes.Brtrue_S, loopStart);
            
            processor.Emit(OpCodes.Ret);
        }
    }
}
