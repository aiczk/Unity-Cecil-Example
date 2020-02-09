﻿using Mono.Cecil.Cil;

namespace LINQ2Method.Basics
{
    public interface ILinqOperator
    {
        Instruction Next(MethodBody func);
    }
    
    public enum LinqOperators
    {
        Where,
        Select,
        SelectMany,
        Take,
        Skip,
        TakeWhile,
        SkipWhile,
        Join,
        GroupJoin,
        Concat,
        OrderBy,
        ThenBy,
        OrderByDescending,
        ThenByDescending,
        Reverse,
        GroupBy,
        Distinct,
        Union,
        Intersect,
        Except,
        AsEnumerable,
        ToArray,
        ToList,
        ToDictionary,
        ToLookUp,
        OfType,
        Cast,
        SequenceEqual,
        First,
        FirstOrDefault,
        Last,
        LastOrDefault,
        Single,
        SingleOrDefault,
        ElementAt,
        ElementAtOrDefault,
        DefaultIfEmpty,
        Range,
        Repeat,
        Empty,
        All,
        Any,
        Contains,
        Count,
        LongCount,
        Sum,
        Min,
        Max,
        Average,
        Aggregate
    }
}