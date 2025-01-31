﻿using Mono_Cecil_Sample.Attributes;
using UnityEngine;

namespace _Script
{
    public class Baa : MonoBehaviour
    {
        [Inject]
        private Fee fee = default;
        
        void Start()
        {
            Debug.Log(fee.fooString);
        }

        void Update()
        {
        
        }
    }

    [Mod]
    public static class BarExtension
    {
        public static void Log()
        {
            Debug.Log("Log");
        }
    }

    [Mod]
    public struct BaaStruct
    {
        public int Length;

        public bool IsOpen()
        {
            return true;
        }
    }

    [Mod]
    public interface IBaaInterface
    {
        void Change();
    }

    [Mod]
    public enum BaaEnum
    {
        a,
        b,
        c,
    }
}
