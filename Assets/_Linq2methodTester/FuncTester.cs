using System;
using System.Collections.Generic;
using System.Linq;
using LINQ2Method.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Script
 {
     public class FuncTester
     {
         [Optimize]
         private void Foo()
         {
             var enumerable = new [] {new Hoo(2), new Hoo(41) };

             var s = enumerable.Where(x => x.Index % 2 == 0).Where(x => x.Index > 114514).Select(x => x.Index);
         }

         private IEnumerable<int> Dodo(int[] arr)
         {
             IEnumerable<int> a = new int[arr.Length];

             foreach (var i in arr)
             {
                 //do!
                 
                 if(i == 0)
                     continue;
             }

             return a;
         }

         private void CC(int[] arr)
         {
             int length = arr.Length;
             for(var i = 0; i < arr.Length; i++)
             {
                 var loc = arr[i];
                 
                 if(loc % 2 != 0)
                     continue;
                 
                 if(loc > 114514)
                     continue;

                 var s = loc;
                 Debug.Log("H");
                 //do
             }

         }
     }

     public class Hoo : IEquatable<Hoo>
     {
         public int Index;

         public Hoo(int index)
         {
             this.Index = index;
         }

         public bool Equals(Hoo other)
         {
             return other != null && other.Index == Index;
         }

         public override bool Equals(object obj)
         {
             if (ReferenceEquals(null, obj)) return false;
             if (ReferenceEquals(this, obj)) return true;
             if (obj.GetType() != this.GetType()) return false;
             return Equals((Hoo) obj);
         }

         public override int GetHashCode()
         {
             throw new NotImplementedException();
         }

         public static bool operator ==(Hoo left, Hoo right)
         {
             return Equals(left, right);
         }

         public static bool operator !=(Hoo left, Hoo right)
         {
             return !Equals(left, right);
         }
     }
 }