using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono_Cecil_Sample.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Script
{
    public class Fee : SingletonMonoBehaviour<Fee>
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