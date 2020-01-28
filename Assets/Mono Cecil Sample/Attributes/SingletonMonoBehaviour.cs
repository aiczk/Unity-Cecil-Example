using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mono_Cecil_Sample.Attributes
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour 
    {
        private static T instance;
    
        public static T Instance 
        {
            get 
            {
                if (instance != null) 
                    return instance;
            
                instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                    return instance;

                if (instance != null) 
                    return instance;
            
                var singleton = new GameObject
                {
                    name = $"{typeof(T)} (singleton)"
                };
                
                instance = singleton.AddComponent<T>();
                DontDestroyOnLoad(singleton);
            
                return instance;
            }
        
            private set => instance = value;
        }

        private void OnDestroy () => Instance = null;
    }

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject 
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                return _instance;
            }
        }
    }
}