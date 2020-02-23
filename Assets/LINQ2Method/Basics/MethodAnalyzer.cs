using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LINQ2Method.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class MethodAnalyzer
    {
        private TypeSystem typeSystem;

        public MethodAnalyzer(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public AnalyzedMethod Analyze(MethodDefinition method)
        {
            MethodDefinition nestedMethodToken = null;
            Operator operatorType = Operator.None;
            
            var operators = new Collection<LinqOperator>();

            foreach (var instruction in method.Body.Instructions)
            {
                var opCode = instruction.OpCode;

                if (opCode != OpCodes.Ldftn && opCode != OpCodes.Call)
                    continue;

                if (opCode == OpCodes.Ldftn)
                {
                    nestedMethodToken = GetToken<MethodDefinition>(instruction);
                    continue;
                }

                if (opCode == OpCodes.Call)
                {
                    var operatorMethodToken = GetToken<GenericInstanceMethod>(instruction);
                    operatorType = (Operator) Enum.Parse(typeof(Operator), operatorMethodToken.Name);
                }

                if (nestedMethodToken == null || operatorType == Operator.None)
                    continue;

                var linqOperator = new LinqOperator(nestedMethodToken, operatorType);
                operators.Add(linqOperator);

                nestedMethodToken = null;
                operatorType = Operator.None;
            }

            return new AnalyzedMethod(operators.ToReadOnlyCollection());

            T GetToken<T>(Instruction instruction) => (T) instruction.Operand;
        }

        public ILinqOperator OperatorFactory(LinqOperator linqOperator, MethodBuilder methodBuilder)
        {
            ILinqOperator op;
            switch (linqOperator.Operator)
            {
                case Operator.Where:
                    op = new Where(typeSystem, linqOperator.NestedMethod, methodBuilder.MainLoop);
                    break;
                
                case Operator.Select:
                    op = new Select(linqOperator.NestedMethod, methodBuilder.MainLoop);
                    break;
                
                default:
                    op = null;
                    break;
            }

            return op;
        }
    }
}