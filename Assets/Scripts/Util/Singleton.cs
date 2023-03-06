using UnityEngine;

namespace QT
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        var obj = new GameObject(typeof(T).ToString());
                        _instance = obj.AddComponent<T>();

                        if (_instance == null)
                        {
                            Debug.Log("Problem during the creation of " + typeof(T).ToString());
                        }
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


        void Awake()
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

        // This function is called when the instance is used the first time
        // Put all the initializations you need here, as you would do in Awake
        protected virtual void _Init()
        {
            /* BLANK */
        }

        protected virtual void _Release()
        {
            /* BLANK */
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance._Release();
                _instance = null;
            }
        }
    }
}