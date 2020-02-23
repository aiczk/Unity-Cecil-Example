using Mono.Cecil;
using Mono.Collections.Generic;

namespace LINQ2Method.Basics
{
    public class AnalyzedClass
    {
        public ReadOnlyCollection<TypeDefinition> OptimizeTypes { get; }

        public AnalyzedClass(ReadOnlyCollection<TypeDefinition> optimizeTypes)
        {
            OptimizeTypes = optimizeTypes;
        }
    }
}