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
        public List<MethodDefinition> NestedMethods(TypeDefinition targetClass)
        {
            var operators = new List<MethodDefinition>();
            foreach (var nestedType in targetClass.NestedTypes)
            {
                foreach (var method in nestedType.Methods)
                {
                    if(method.Name == ".ctor" || method.Name == ".cctor")
                        continue;
                    
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if(instruction.OpCode != OpCodes.Call)
                            continue;
                    
                        var instructionStr = instruction.Operand.ToString();
                        var linqOperator = ParseOperator(instructionStr, @"(?<=<).+?(?=>)");

                        if(linqOperator == Operator.None)
                            continue;

                        operators.Add(method);
                    }
                }
            }

            return operators;
        }
        
        public List<LinqOperator> DefineOperator(MethodDefinition targetMethod, List<MethodDefinition> nestMethods)
        {
            var operators = new List<LinqOperator>();
            foreach (var instruction in targetMethod.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldftn)
                {
                    var operand = (MethodDefinition) instruction.Operand;
                    Debug.Log(operand.Name);
                }
                continue;
                if(instruction.OpCode != OpCodes.Call)
                    continue;
                    
                var instructionStr = instruction.Operand.ToString();
                var linqOperator = ParseOperator(instructionStr, @"(?<=::).+?(?=<)");
                    
                if(linqOperator == Operator.None)
                    continue;
                
                var nestMethod = Find(targetMethod, nestMethods);
                operators.Add(new LinqOperator(linqOperator, nestMethod.NestedFunction));
            }
            return operators;
        }

        public List<MethodDefinition> TargetMethods(TypeDefinition targetClass, string attributeName)
        {
            var methods = new List<MethodDefinition>();
            foreach (var method in targetClass.Methods)
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

        private LinqOperator Find(MethodDefinition targetMethod, List<MethodDefinition> operators)
        {
            var methodName = targetMethod.Name;
            LinqOperator result = null;
            
            foreach (var linqOperator in operators)
            {
                var functionName = linqOperator.Name;

                functionName = Regex.Match(functionName, @"(?<=<).+?(?=>)").Value;
                if(functionName == methodName)
                    continue;

                //result = linqOperator;
                break;
            }

            return result;
        }

        private Operator ParseOperator(string str,string matchPattern)
        {
            str = Regex.Match(str, matchPattern).Value;

            if (str == string.Empty)
                return Operator.None;

            return (Operator) Enum.Parse(typeof(Operator), str);
        }
    }
}