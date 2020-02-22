using System.Collections.Generic;
using Mono.Collections.Generic;
using UnityEngine;

namespace LINQ2Method.Helpers
{
    public static class CollectionHelper
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this Collection<T> collection) =>
            new ReadOnlyCollection<T>(collection);
    }
}
