using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Object = UnityEngine.Object;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace QT
{
    public class ResourceManager
    {
        private Transform _poolRootTransform;
        
        private readonly Dictionary<string, Object> _cache = new ();
        private readonly Dictionary<Type, Stack<Component>> _pool = new ();
        
        public void Initialize()
        {
            _poolRootTransform = new GameObject("PoolRoot").transform;
            GameObject.DontDestroyOnLoad(_poolRootTransform);
        }

        public async UniTaskVoid CacheAssets(string[] paths)
        {
            foreach (var path in paths)
            {
                CacheAsset(path);
            }
        }
        
        public async UniTaskVoid CacheAsset(string path)
        {
            var asset = await Addressables.LoadAssetAsync<Object>(path);
            _cache.TryAdd(path, asset);
        }
        
        public async UniTask<T> LoadAsset<T>(string path, bool isCaching) where T : Object
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            if (isCaching && _cache.TryGetValue(path, out var cached))
            {
                return Object.Instantiate(cached as T);
            }
            
            try
            {
                return Object.Instantiate(await Addressables.LoadAssetAsync<T>(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"에셋 로드에 실패했습니다. Path : {path}\n\n{e.Message}");
            }

            return null;
        }
        
        public async UniTask<IList<T>> LoadAssets<T>(IList<IResourceLocation> locations) where T : Object
        {
            var assets = new List<T>();

            foreach (var location in locations)
            {
                var asset = await Addressables.LoadAssetAsync<T>(location).Task;

                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }


        public async UniTask<T> GetFromPool<T>(string path, Transform parent = null) where T : Component
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
                _pool.Add(typeof(T), new Stack<Component>());
                (await LoadAsset<GameObject>(path, true))?.TryGetComponent(out obj);
            }

            if (obj != null)
            {
                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(true);
            }

            return obj;
        }

        public void ReleaseObject<T>(T obj) where T : Component
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
        
        public async UniTask<IList<IResourceLocation>> GetLocations(string assetLabel)
        {
            var handle = await Addressables.LoadResourceLocationsAsync(assetLabel);
            return handle;
        }
        

    }
}
