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
        
        public AnalyzedMethod Analyze(MethodDefinition targetMethod)
        {
            var calledOperators = CalledOperatorTokens(targetMethod);
            var nestedMethods = NestedMethodTokens(targetMethod);
            var operators = new List<LinqOperator>();	

            for (var i = 0; i < calledOperators.Count; i++)
            {	
                var calledOperator = calledOperators[i];	
                var nestedMethod = nestedMethods[i];	
                var linqOperator = new LinqOperator(nestedMethod, calledOperator);	

                operators.Add(linqOperator);	
            }

            return new AnalyzedMethod(operators);
        }
        
        private ReadOnlyCollection<Operator> CalledOperatorTokens(MethodDefinition method)	
        {	
            var operators = new Collection<Operator>();	
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != OpCodes.Call)
                    continue;


                var genericInstanceMethod = GetToken<GenericInstanceMethod>(instruction);
                var result = Cast(genericInstanceMethod);
                operators.Add(result);	
            }

            return operators.ToReadOnlyCollection();
            
            Operator Cast(MemberReference genericInstanceMethod) => (Operator) Enum.Parse(typeof(Operator), genericInstanceMethod.Name);
        }
        
        private ReadOnlyCollection<MethodDefinition> NestedMethodTokens(MethodDefinition method) 
        {	
            var operators = new Collection<MethodDefinition>();	
            foreach (var instruction in method.Body.Instructions)	
            {	
                if(instruction.OpCode != OpCodes.Ldftn)	
                    continue;

                var cast = GetToken<MethodDefinition>(instruction);
                operators.Add(cast);
            }


            return operators.ToReadOnlyCollection();
        }
        
        private T GetToken<T>(Instruction instruction) => (T) instruction.Operand;

        public ILinqOperator Generate(LinqOperator linqOperator, Method method)
        {
            ILinqOperator op;
            switch (linqOperator.Operator)
            {
                case Operator.Where:
                    op = new Where(typeSystem, linqOperator.NestedMethod, method.MainLoop);
                    break;
                case Operator.Select:
                    op = new Select(linqOperator.NestedMethod, method.MainLoop);
                    break;
                default:
                    op = null;
                    break;
            }

            return op;
        }
    }
}