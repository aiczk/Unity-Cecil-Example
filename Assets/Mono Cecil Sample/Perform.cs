using System.Collections;
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
        var array = new[]{1, 4, 5, 2, 5, 2, 3, 4};
        var stopWatch = new Stopwatch();
        
        stopWatch.Start();
        Foo(array);
        stopWatch.Stop();
        Debug.Log(stopWatch.Elapsed.ToString());
        
        stopWatch.Restart();
        CC(array);
        stopWatch.Stop();
        Debug.Log(stopWatch.Elapsed.ToString());
    }
    
    private void Foo(int[] arr)
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
