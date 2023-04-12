using UnityEngine;

namespace QT
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (!_instance)
                    {
                        Debug.LogError("Problem during get " + typeof(T).ToString());
                    }
                    else
                    {
                        _instance._Init();
                    }
                }
                return _instance;
            }
        }

        public static bool IsAlive { get { return (_instance != null); } }


        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (_instance == null)
            {
                _instance = this as T;
                _instance._Init();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }


        protected virtual void _Init()
        {
        }

        protected virtual void _Release()
        {
        }

        private  void OnDestroy()
        {
            if (_instance == this)
            {
                _instance._Release();
                _instance = null;
            }
        }
    }
}