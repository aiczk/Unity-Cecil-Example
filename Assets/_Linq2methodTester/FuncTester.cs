using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Script;
using LINQ2Method.Attributes;

namespace _Script
 {
     public class FuncTester
     {
         private List<int> collection = new List<int>();
         
         //[Optimize]
         private void Foo()
         {
             var enumerable = new[] {new Hoge(2), new Hoge(41)};
             var s = enumerable.Where(x => x.Index % 2 == 0).Where(x => x.Index > 114514).Select(x => x.Index);
         }
         
         //[Optimize]
         private void Doo()
         {
             var s = Dodo(new Hoge[4]);
         }
         
         private IEnumerable<int> Dodo(Hoge[] arr)
         {
             if (collection.Count < 0) 
                 collection.Clear();

             foreach (var i in arr)
             {
                 //do!
                 var ia = i.Index;
                 
                 collection.Add(ia);
             }

             return collection;
         }
     }

     public class Foo
     {
         private int[] enumerable = {1, 3, 4,};
         
         [Optimize]
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