﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Perform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
/*        var array = new[]{1, 4, 5, 2, 5, 2, 3, 4};
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();
        Foo(array);
        stopWatch.Stop();
        Debug.Log(stopWatch.Elapsed.ToString());
        
        stopWatch.Restart();
        CC(array);
        stopWatch.Stop();
        Debug.Log(stopWatch.Elapsed.ToString());*/

        FSA();
    }
    
    private void FSA()
    {
        var array = new Foo[1000000];

        for (var i = 0; i < 1000000; i++)
        {
            array[i] = new Foo(i);
        }

        var stopWatch = new Stopwatch();

        stopWatch.Start();
        for (var i = 0; i < 1000000; i++)
        {
            var s = array.Where(x => x.Index % 2 == 0).Select(x => x.Index);
        }
        stopWatch.Stop();
        Debug.Log($"LINQ :{stopWatch.ElapsedMilliseconds.ToString()}");
            
        stopWatch.Reset();
        stopWatch.Start();
        for (var i = 0; i < 1000000; i++)
        {
            ref var s = ref array[i];

            if (s.Index % 2 == 0)
            {
                var ind = s.Index;
                _ = ind;
            }
        }
        stopWatch.Stop();
        Debug.Log($"METHOD :{stopWatch.ElapsedMilliseconds.ToString()}");
    }

    private class Foo
    {
        public int Index { get; }

        public Foo(int index)
        {
            Index = index;
        }
    }

    private void Fofo(int[] arr)
    {
        var result = arr.Where(x => x == 0).Select(x => x + 1);
    }
         
    private void CC(int[] arr)
    {
        int result = 0;
        for(var i = 0; i < arr.Length; i++)
        {
            result = arr[i];
            if(result == 0)
            {
                result += 1; 
            }
        }
    }
}
