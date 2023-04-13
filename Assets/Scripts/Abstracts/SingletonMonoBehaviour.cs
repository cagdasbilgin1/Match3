using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.Abstracts
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected void SetSingletonThisGameObject(T instance)
        {
            if (Instance == null)
            {
                Instance = instance;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}