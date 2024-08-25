using UnityEngine;

namespace Utils
{
    public class SingletonMonoBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {    
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }
    
        protected SingletonMonoBase()
        {
        
        }
    }
}
