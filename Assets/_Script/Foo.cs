using Mono_Cecil_Intro.Script;
using UnityEngine;

namespace _Script
{
    public class Foo : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        [Profile]
        private void Update()
        {
            var bytes = new byte[512];
            bytes[0] = 0;
        }
    }
}