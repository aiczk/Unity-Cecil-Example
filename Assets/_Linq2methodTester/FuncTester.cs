using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using _Script;
using LinqPatcher.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Script
 {
     public class FuncTester : MonoBehaviour
     {
         private Hoge[] enumerable = new Hoge[250];

         [Optimize]
         private void Optimize()
         {
             var result = enumerable.Select(x => x.Name).Where(x => x.Contains("t")).Select(x => x.Length);
         }
         
         private void NonOptimize()
         {
             var ss = enumerable.Select(x => x.Name).Where(x => x.Contains("t")).Select(x => x.Length);
         }
         
         private static int Test1(int[] array)
         {
             var sum = 0;
             foreach (var t in array)
             {
                 sum += t;
             }
             
             return sum;
         }

         private static unsafe int Test2(int[] array)
         {
             var sum = 0;
             fixed (int* pArray = &array[0])
             {
                 var ptr = pArray;
                 var length = array.Length;
                 for (var i = 0; i < length; i++, ptr++)
                 {
                     sum += *ptr;
                 }
             }
             return sum;
         }

         private static unsafe int Test3(int[] array)
         {
             var sum = 0;
             var length = array.Length;
             for (var i = 0; i < length; i++)
             {
                 fixed(int* ptr = &array[i])
                 {
                     sum += *ptr;
                 }
             }

             return sum;
         }
         
         private void Start()
         {
             for(var i = 0; i < enumerable.Length;i++)
                 enumerable[i] = new Hoge(Guid.NewGuid().ToString("N"));
             
             var array = new int[1000];
             var stopWatch = new Stopwatch();
             
             //stopWatch.Start();
             var sum1 = Test1(array);
             //stopWatch.Stop();
             //Debug.Log($"Not  : {stopWatch.Elapsed.ToString()}");
             
             stopWatch.Restart();
             NonOptimize();
             stopWatch.Stop();
             Debug.Log($"non : {stopWatch.Elapsed.ToString()}");

             stopWatch.Restart();
             Optimize();
             stopWatch.Stop();
             Debug.Log($"opt : {stopWatch.Elapsed.ToString()}");
         }
         
     }

     public static class Memoize
     {
         public static TValue ComputeIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> func)
         {
             if (!dictionary.TryGetValue(key, out var result))
             {
                 result = func(key);
                 dictionary.Add(key, result);
             }

             return result;
         }

         public class Alpha
         {
             private Dictionary<string, int> dic = new Dictionary<string, int>();

             public int GetResult(string key)
             {
                 if (!dic.TryGetValue(key, out var value))
                 {
                     var val = key.Length;
                     dic.Add(key, val);
                 }
                 return value;
             }
             
             //if(!TryGetValue(key,out var value))
             //Expression(次のローカルに代入している計算式)
             //dic.add()
             //return value
         }
     }

     public class Foo
     {
         private int[] enumerable = {1, 3, 4,};
         
         //[Optimize]
         private void Boo()
         {
             var s = enumerable
                 .Where(x => x % 2 == 0)
                 .Where(x => x > 10)
                 .Select(x => x * 2)
                 .Where(x => x < 2)
                 .Where(x => x > 11)
                 ;
         }
     }
 }