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
         public void Fnc<T>(T value,Func<T, bool> func)
         {
             func(value);
         }
 
         [Optimization]
         private void Foo()
         {
             var enumerable = new [] {(2), (41) };

             var s = enumerable.Where(x => x.GetHashCode() < 10).Select(x => Random.Range(0,10));
             
             Fnc(9, x => x != 10);
             Fnc(new Hoo(2),x => x == new Hoo(3));
         }
         
         private void CC(int[] arr)
         {
             for(var i = 0; i < arr.Length; i++)
             {
                 var loc = arr[i];

                 if (loc.GetHashCode() < 10)
                 {
                     
                 }

                 //loc = Random.Range(0, 10);
             }
         }

         private void Hoo()
         {
             // ReSharper disable once NotAccessedVariable
#pragma warning disable 219
             var su = 0;
             var sua = 0;
             var sus = 0;
             var suf = 0;
#pragma warning restore 219

             for (var i = 0; i < 10; i++)
             {
                 su += i;
             }
         }
     }

     public class Hoo : IEquatable<Hoo>
     {
         public int rdm;

         public Hoo(int rdm)
         {
             this.rdm = rdm;
         }

         public bool Equals(Hoo other)
         {
             return other != null && other.rdm == rdm;
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