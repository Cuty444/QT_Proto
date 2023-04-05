using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace QT
{
    public class ResourceManager
    {
        private Dictionary<string, Object> _cache = new ();

        public async UniTask<T> LoadAsset<T>(string path, bool isCaching) where T : Object
        {
            T asset;
            
            if (isCaching && _cache.TryGetValue(path, out var cached))
            {
                asset = cached as T;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
            
                await handle.Task;
            
                asset = handle.Result;
            }

            return Object.Instantiate(asset);
        }
    }
}
