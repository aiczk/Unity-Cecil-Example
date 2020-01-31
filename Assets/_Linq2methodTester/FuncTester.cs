using System;
using System.Linq;
using UnityEngine;

namespace _Script
 {
     public class FuncTester
     {
         public void Fnc<T>(T value,Func<T, bool> func)
         {
             func(value);
             Debug.Log("OI");
         }
 
         private void Foo()
         {
             Fnc(0, x => x != 10);
             Fnc(new Hoo(2),x => x == new Hoo(3));
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
         private int rdm;

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