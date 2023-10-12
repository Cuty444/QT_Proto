using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using Object = UnityEngine.Object;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace QT
{
    public class ResourceManager
    {
        private readonly Dictionary<string, Object> _cache = new ();
        private readonly Dictionary<string, Stack<Component>> _pool = new ();
        
        private Transform _poolRootTransform;
        
        public void Initialize()
        {
            _pool.Clear();
            _cache.Clear();
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
            if (_cache.ContainsKey(path))
            {
                return;
            }
            
            var asset = await Addressables.LoadAssetAsync<Object>(path);
            _cache.TryAdd(path, asset);
        }
        
        public async UniTask<T> LoadAsset<T>(string path, bool isCaching, Transform parent = null) where T : Object
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
                return Object.Instantiate(await Addressables.LoadAssetAsync<T>(path), parent);
            }
            catch (Exception e)
            {
                Debug.LogError($"에셋 로드에 실패했습니다. Path : {path}\n\n{e.Message}");
            }

            return null;
        }
        
        public async UniTask<IList<T>> LoadAssets<T>(IList<IResourceLocation> locations) where T : Object
        {
            var tasks = new List<UniTask<T>>();

            foreach (var location in locations)
            {
                tasks.Add(Addressables.LoadAssetAsync<T>(location).ToUniTask());
            }

            var assets= await UniTask.WhenAll(tasks);

            return assets;
        }


        public async UniTask<T> GetFromPool<T>(string path, Transform parent = null) where T : Component
        {
            T obj = null;

            if (_pool.TryGetValue(path, out var stack))
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
                _pool.Add(path, new Stack<Component>());
                (await LoadAsset<GameObject>(path, true))?.TryGetComponent(out obj);
            }

            if (obj != null)
            {
                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(true);
            }

            return obj;
        }

        public void ReleaseObject<T>(string path, T obj) where T : Component
        {
            if (_pool.TryGetValue(path, out var pool))
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_poolRootTransform);
                pool.Push(obj);
            }
            else
            {
                if (obj == null)
                    return;
                Object.Destroy(obj.gameObject);
            }
        }
        
        public async UniTaskVoid ReleaseObjectWithDelay<T>(string path, T obj, float duration) where T : Component
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            ReleaseObject(path, obj);
        }

        public void AllReleasedObject()
        {
            GameObject.Destroy(_poolRootTransform.gameObject);
            Initialize();

            Resources.UnloadUnusedAssets();
        }

        public async UniTaskVoid EmitParticle(string path, Vector2 position, float rotation = 0, Transform parent = null)
        {
            var particle = await GetFromPool<ParticleSystem>(path, parent);
            
            particle.transform.localPosition = position;

            if (rotation != 0)
            {
                particle.transform.rotation = Quaternion.Euler(-90, 0, rotation);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(particle.main.duration));

            ReleaseObject(path, particle);
        }
        
        public async UniTaskVoid EmitParticle(string path, Vector2 position,Quaternion rotations, Transform parent = null)
        {
            var particle = await GetFromPool<ParticleSystem>(path, parent);
            
            particle.transform.localPosition = position;

            particle.transform.localRotation = rotations;
            
            await UniTask.Delay(TimeSpan.FromSeconds(particle.main.duration));

            ReleaseObject(path, particle);
        }
        
        public async UniTask<IList<IResourceLocation>> GetLocations(string assetLabel)
        {
            var handle = await Addressables.LoadResourceLocationsAsync(assetLabel);
            return handle;
        }
        
        public async UniTaskVoid LoadSpriteRenderer(string spritePath,SpriteRenderer spriteRenderer)
        {
            try
            {
                var loadSprite = await Addressables.LoadAssetAsync<Sprite>(spritePath);
                spriteRenderer.sprite = loadSprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"스프라이트 로드에 실패했습니다. Path : {spritePath}\n\n{e.Message}");
            }
            
        }

        public async UniTaskVoid LoadUIImage(string spritePath, Image image)
        {
            try
            {
                var loadSprite = await Addressables.LoadAssetAsync<Sprite>(spritePath);
                image.sprite = loadSprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"스프라이트 로드에 실패했습니다. Path : {spritePath}\n\n{e.Message}");
            }

        }

    }
}
