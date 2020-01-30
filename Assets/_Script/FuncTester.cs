using System;
using System.Linq;
using UnityEngine;

namespace _Script
 {
     public class FuncTester
     {
         public void Fnc<T>(T value,Func<T, T> func)
         {
             func(value);
             Debug.Log("OI");
         }
 
         private void Foo()
         {
             var enume = new[] { 1,2,4,5,6,7,76,2342};
         }

         private void Hoo()
         {
             var su = 0;
             var sua = 0;
             var sus = 0;
             var suf = 0;

             for (var i = 0; i < 10; i++)
             {
                 su += i;
             }
         }
     }
 }