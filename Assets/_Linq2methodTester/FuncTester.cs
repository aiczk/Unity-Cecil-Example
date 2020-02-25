using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using _Script;
using LINQ2Method.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Script
 {
     public class FuncTester : MonoBehaviour
     {
         private Hoge[] enumerable = new Hoge[1000000];

         [Optimize]
         private void Optimize()
         {
             var ff = enumerable.Select(x => x.Index).Where(x => x % 2 == 0).Select(x => x * 3);
         }
         
         private void NonOptimize()
         {
             var ff = enumerable.Select(x => x.Index).Where(x => x % 2 == 0).Select(x => x * 3);
         }
         
         private void Time()
         {
             var stopWatch = new Stopwatch();
             stopWatch.Start();
             for (var i = 0; i < 1000000; i++)
             {
                 Optimize();
             }
             stopWatch.Stop();
             Debug.Log($"OPTIMIZE: {stopWatch.ElapsedMilliseconds.ToString()}");
             
             stopWatch.Reset();
             stopWatch.Start();
             for (var i = 0; i < 1000000; i++)
             {
                 NonOptimize();
             }
             stopWatch.Stop();
             Debug.Log($"NON OPTIMIZE: {stopWatch.ElapsedMilliseconds.ToString()}");
         }

         private void Start()
         {
             Time();
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