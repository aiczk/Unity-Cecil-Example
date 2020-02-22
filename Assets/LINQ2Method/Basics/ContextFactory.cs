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

        public ILinqOperator Gen(LinqOperator linqOperator, TypeSystem typeSystem, Method method)
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

        public List<LinqOperator> AnalysisMethod(MethodDefinition targetMethod)
        {
            var calledOperators = CalledOperatorToken(targetMethod);
            var nestedMethods = NestedMethodToken(targetMethod);
            var operators = new List<LinqOperator>();
            
            for (int i = 0; i < calledOperators.Count; i++)
            {
                var calledOperator = calledOperators[i];
                var nestedMethod = nestedMethods[i];
                var linqOperator = new LinqOperator(calledOperator, nestedMethod);
                
                operators.Add(linqOperator);
            }

            return operators;
        }
        
        private List<Operator> CalledOperatorToken(MethodDefinition method)
        {
            return AnalysisBase<Operator, GenericInstanceMethod>(method, OpCodes.Call, Cast);
            
            Operator Cast(MemberReference genericInstanceMethod) => 
                (Operator) Enum.Parse(typeof(Operator), genericInstanceMethod.Name);
        }

        private List<MethodDefinition> NestedMethodToken(MethodDefinition targetMethod) => 
            AnalysisBase<MethodDefinition>(targetMethod, OpCodes.Ldftn);

        private List<T> AnalysisBase<T>(MethodDefinition method, OpCode opCode)
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
        
        private List<T> AnalysisBase<T,TC>(MethodDefinition method, OpCode opCode, Func<TC,T> func)
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