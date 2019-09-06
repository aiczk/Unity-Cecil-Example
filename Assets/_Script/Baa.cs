using Mono_Cecil_Sample.Attributes;
using UnityEngine;

namespace _Script
{
    [Mod]
    public class Baa : MonoBehaviour
    {
        void Start()
        {
        
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
            
        }
    }

    [Mod]
    public struct BaaStruct
    {
        public int Length { get; set; }
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
