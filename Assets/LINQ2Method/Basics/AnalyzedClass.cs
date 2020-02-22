using System.Collections.Generic;
using Mono.Cecil;

namespace LINQ2Method.Basics
{
    public class AnalyzedClass
    {
        public IReadOnlyCollection<TypeDefinition> OptimizeTypes { get; }
        public IReadOnlyCollection<MethodDefinition> OptimizeMethods { get; }

        public AnalyzedClass(IReadOnlyCollection<TypeDefinition> optimizeTypes, IReadOnlyCollection<MethodDefinition> optimizeMethods)
        {
            OptimizeTypes = optimizeTypes;
            OptimizeMethods = optimizeMethods;
        }
    }
}