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
        private TypeDefinition optimizeClass;

        public MethodAnalyzer(TypeSystem typeSystem, TypeDefinition optimizeClass)
        {
            this.typeSystem = typeSystem;
            this.optimizeClass = optimizeClass;
        }
        
        //todo ここに置くべきではないので移植する
        public Collection<MethodDefinition> OptimizeMethods(string attributeName)
        {
            var methods = new Collection<MethodDefinition>();
            foreach (var method in optimizeClass.Methods)
            {
                if (!method.HasCustomAttributes)
                    continue;
                    
                foreach (var customAttribute in method.CustomAttributes)
                {
                    var attributeType = customAttribute.AttributeType;

                    if (attributeType.Name != attributeName)
                        continue;
                    
                    methods.Add(method);
                    break;
                }
            }

            return methods;
        }

        //todo ここに置くべきではないので移植する
        public ILinqOperator Generate(LinqOperator linqOperator, Method method)
        {
            ILinqOperator op;
            switch (linqOperator.Operator)
            {
                case Operator.Where:
                    op = new Where(typeSystem, linqOperator.InnerMethod, method.MainLoop);
                    break;
                case Operator.Select:
                    op = new Select(linqOperator.InnerMethod, method.MainLoop);
                    break;
                default:
                    op = null;
                    break;
            }

            return op;
        }

        public AnalyzedMethod Analyze(MethodDefinition method)
        {
            var operators = new List<LinqOperator>();
            MethodDefinition nestedMethodToken = null;
            var operatorType = Operator.None;
            
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
                    continue;
                }

                if (nestedMethodToken == null || operatorType == Operator.None) 
                    continue;
                
                var linqOperator = new LinqOperator(nestedMethodToken, operatorType);
                operators.Add(linqOperator);

                nestedMethodToken = null;
                operatorType = Operator.None;
            }

            return new AnalyzedMethod(operators);
            
            T GetToken<T>(Instruction instruction) => (T) instruction.Operand;
        }
    }
}