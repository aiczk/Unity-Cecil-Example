using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public class ContextFactory
    {
        private static readonly string MatchPattern = @"(?<=::).+?(?=<)";
        
        public List<OperatorContext> CreateOperatorContext(TypeDefinition optimizeClass)
        {
            List<OperatorContext> contexts = new List<OperatorContext>();
            
            foreach (var classDefinition in optimizeClass.NestedTypes)
            {
                if(!classDefinition.IsClass)
                    continue;

                foreach (var method in classDefinition.Methods)
                {
                    var methodName = method.Name;
                    var linqOperator = ParseOperator(methodName);
                    var operatorContext = new OperatorContext(method, linqOperator);
                    contexts.Add(operatorContext);
                }
            }

            return contexts;
        }

        public List<Operator> CreateContext(TypeDefinition optimizeClass)
        {
            var contexts = new List<Operator>();

            foreach (var method in optimizeClass.Methods)
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if(instruction.OpCode != OpCodes.Call)
                        continue;

                    var str = instruction.Operand.ToString();
                    var linqOperator = ParseOperator(str);
                    
                    if(linqOperator == Operator.None)
                        continue;
                    
                    contexts.Add(linqOperator);
                }

            }
            
            return contexts;
        }

        private Operator ParseOperator(string str)
        {
            str = Regex.Match(str, MatchPattern).Value;

            if (str == string.Empty)
                return Operator.None;

            return (Operator) Enum.Parse(typeof(Operator), str);
        }
    }
}