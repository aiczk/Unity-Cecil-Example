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
             var enume = new[]
             {
                 new Tester("ABC",100,321), 
                 new Tester("CUDA",100,321), 
                 new Tester("asdf",100,321), 
                 new Tester("Aghs",100,321), 
             };

             var s = enume.Where(x => x.Id == "ABC").Select(x => x.Damage++);
         }

         private void Hoo()
         {
             var su = 0;

             for (var i = 0; i < 10; i++)
             {
                 su += i;
             }
         }
     }

     public class Tester : IEquatable<Tester>
     {
         public float Hp;
         public int Damage;
         public string Id;

         public Tester(string id, int damage, float hp)
         {
             Id = id;
             Damage = damage;
             Hp = hp;
         }

         public bool Equals(Tester other) => other != null && other.Id == Id;
     }
 }