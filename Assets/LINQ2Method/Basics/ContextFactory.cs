using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace LINQ2Method.Basics
{
    public class ContextFactory
    {
        private TypeDefinition optimizeClass;

        public ContextFactory(TypeDefinition optimizeClass)
        {
            this.optimizeClass = optimizeClass;
        }

        public List<LinqOperator> MethodAnalysis(MethodDefinition targetMethod)
        {
            var calledOperators = CalledOperatorToken(targetMethod);
            var nestedMethods = NestedMethodToken(targetMethod);
            var operators = new List<LinqOperator>();
            
            for (int i = 0; i < calledOperators.Count; i++)
            {
                var c = calledOperators[i];
                var l = nestedMethods[i];

                var linqOperator = new LinqOperator(c, l);
                operators.Add(linqOperator);
            }

            return operators;
        }
        
        private List<Operator> CalledOperatorToken(MethodDefinition method)
        {
            if (method.IsConstructor)
                return null;
            
            var operators = new List<Operator>();
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != OpCodes.Call)
                    continue;

                var genericInstanceMethod = (GenericInstanceMethod) instruction.Operand;
                var linqOperator = (Operator) Enum.Parse(typeof(Operator), genericInstanceMethod.Name);
                operators.Add(linqOperator);
            }

            return operators;
        }
        
        private List<MethodDefinition> NestedMethodToken(MethodDefinition targetMethod)
        {
            var operators = new List<MethodDefinition>();
            foreach (var instruction in targetMethod.Body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Ldftn)
                    continue;
                
                var method = (MethodDefinition) instruction.Operand;
                operators.Add(method);
            }
            return operators;
        }

        public List<MethodDefinition> ProcessMethods(string attributeName)
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
    }
}