using Mono_Cecil_Sample.Attributes;
using UnityEngine;

namespace _Script
{
    public class Foo : SingletonMonoBehaviour<Foo>
    {
        public string fooString = "FOOO";
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