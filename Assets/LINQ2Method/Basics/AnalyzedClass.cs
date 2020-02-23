using System.Collections.Generic;
using Mono.Cecil;

namespace LINQ2Method.Basics
{
    public class AnalyzedClass
    {
        public IReadOnlyCollection<TypeDefinition> OptimizeTypes { get; }

        public AnalyzedClass(IReadOnlyCollection<TypeDefinition> optimizeTypes)
        {
            OptimizeTypes = optimizeTypes;
        }
    }
}