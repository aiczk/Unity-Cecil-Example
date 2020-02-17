using System.Collections.Generic;
using System.Linq;
using _Script;
using LINQ2Method.Attributes;

namespace _Script
 {
     public class FuncTester
     {
         //[Optimize]
         private void Foo()
         {
             var enumerable = new[] {new Hoge(2), new Hoge(41)};
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
     }

     public class Foo
     {
         [Optimize]
         private void Boo()
         {
             var enumerable = new[] {2, 41};

             var s = enumerable.Where(x => x % 2 == 0).Where(x => x > 10).Select(x => x * 2);
         }

         private int IntValue() => 123;
     }
 }