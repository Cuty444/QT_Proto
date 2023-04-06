using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

using Object = UnityEngine.Object;

namespace QT
{
    public class ResourceManager
    {
        private Transform _poolRootTransform;
        
        private readonly Dictionary<string, Object> _cache = new ();
        private readonly Dictionary<Type, Stack<MonoBehaviour>> _pool = new ();
        
        public void Initialize()
        {
            _poolRootTransform = new GameObject("PoolRoot").transform;
            GameObject.DontDestroyOnLoad(_poolRootTransform);
        }

        public async UniTask<T> LoadAsset<T>(string path, bool isCaching) where T : Object
        {
            T asset;
            
            if (isCaching && _cache.TryGetValue(path, out var cached))
            {
                asset = cached as T;
            }
            else
            {
                Debug.Log(path);
                asset = await Addressables.LoadAssetAsync<T>(path);
            }

            return Object.Instantiate(asset);
        }

        public async UniTask<T> GetFromPool<T>(string path, Transform parent = null) where T : MonoBehaviour
        {
            T obj = null;

            if (_pool.TryGetValue(typeof(T), out var stack))
            {
                if (stack.Count > 0)
                {
                    obj = (T) stack.Pop();
                }
                else
                {
                    obj = (await LoadAsset<GameObject>(path, true)).GetComponent<T>();
                }
            }
            else
            {
                _pool.Add(typeof(T), new Stack<MonoBehaviour>());
                obj = (await LoadAsset<GameObject>(path, true)).GetComponent<T>();
            }

            if (obj != null)
            {
                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(true);
            }

            return obj;
        }

        public void ReleaseObject<T>(T obj) where T : MonoBehaviour
        {
            if (_pool.TryGetValue(typeof(T), out var pool))
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_poolRootTransform);
                pool.Push(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }
    }
}
