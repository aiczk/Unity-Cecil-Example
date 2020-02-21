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
            return Base<Operator, GenericInstanceMethod>(method, OpCodes.Call, Cast);
            
            Operator Cast(MemberReference genericInstanceMethod) => 
                (Operator) Enum.Parse(typeof(Operator), genericInstanceMethod.Name);
        }

        private List<MethodDefinition> NestedMethodToken(MethodDefinition targetMethod) => 
            Base<MethodDefinition>(targetMethod, OpCodes.Ldftn);

        private List<T> Base<T>(MethodDefinition method, OpCode opCode)
        {
            var operators = new List<T>();
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != opCode)
                    continue;

                var cast = (T) instruction.Operand;
                operators.Add(cast);
            }

            return operators;
        }
        
        private List<T> Base<T,TC>(MethodDefinition method, OpCode opCode, Func<TC,T> func)
        {
            var operators = new List<T>();
            foreach (var instruction in method.Body.Instructions)
            {
                if(instruction.OpCode != opCode)
                    continue;

                var cast = (TC) instruction.Operand;
                var result = func(cast);
                operators.Add(result);
            }

            return operators;
        }
    }
}