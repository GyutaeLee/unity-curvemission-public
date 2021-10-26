using UnityEngine;

namespace Util
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                    return _instance;
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}