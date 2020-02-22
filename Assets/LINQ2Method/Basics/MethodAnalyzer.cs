using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
        public List<MethodDefinition> OptimizeMethods(string attributeName)
        {
            var methods = new List<MethodDefinition>();
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
        
        public AnalysedMethod Analyse(MethodDefinition targetMethod)
        {
            var calledOperators = CalledOperatorToken(targetMethod);
            var nestedMethods = NestedMethodToken(targetMethod);
            var operators = new List<LinqOperator>();
            
            for (var i = 0; i < calledOperators.Count; i++)
            {
                var calledOperator = calledOperators[i];
                var nestedMethod = nestedMethods[i];
                var linqOperator = new LinqOperator(calledOperator, nestedMethod);
                
                operators.Add(linqOperator);
            }

            return new AnalysedMethod(operators);
        }
        
        private List<Operator> CalledOperatorToken(MethodDefinition method)
        {
            var operators = new List<Operator>();
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != OpCodes.Call)
                    continue;

                var genericInstanceMethod = (GenericInstanceMethod) instruction.Operand;
                var result = Cast(genericInstanceMethod);
                operators.Add(result);
            }

            return operators;
            
            Operator Cast(MemberReference genericInstanceMethod) => (Operator) Enum.Parse(typeof(Operator), genericInstanceMethod.Name);
        }

        private List<MethodDefinition> NestedMethodToken(MethodDefinition method)
        {
            var operators = new List<MethodDefinition>();
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != OpCodes.Ldftn)
                    continue;

                var cast = (MethodDefinition) instruction.Operand;
                operators.Add(cast);
            }

            return operators;
        }
    }
}