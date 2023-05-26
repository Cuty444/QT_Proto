using System.Collections;
using System.Collections.Generic;
using QT.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QT
{
    public class SoundManager
    {
        private Transform _poolRootTransform;

        private AudioSource _audioSource;
        private const string AudioObjectPath = "Assets/Sound/Prefabs/AudioSource.prefab";
        private const string Wav = ".wav";

        private List<AsyncOperationHandle> _soundSourceList = new List<AsyncOperationHandle>();
        private Dictionary<string, AudioSource> _audioSourceDictionary = new Dictionary<string, AudioSource>();
        public void Initialize(AudioSource audioSource)
        {
            _poolRootTransform = new GameObject("AudioSourcePoolRoot").transform;
            GameObject.DontDestroyOnLoad(_poolRootTransform);
            _audioSource = audioSource;
        }


        public void PlayOneShot(string soundPath)
        {
            LoadAudioClip(soundPath);
        }

        public void RandomSoundOneShot(string soundPath, int max)
        {
            LoadAudioClip(soundPath + Random.Range(1, max + 1) + Wav);
        }

        public void ControlAudioPlay(string soundPath ,bool loop = false)
        {
            if (_audioSourceDictionary.TryGetValue(soundPath, out var value))
            {
                value.loop = loop;
                value.Play();
            }
            else
            {
                InstateAudioSource(soundPath);
            }
        }

        public void ControlAudioStop(string soundPath)
        {
            if (_audioSourceDictionary.ContainsKey(soundPath))
            {
                _audioSourceDictionary[soundPath].Stop();
            }
        }

        private async void InstateAudioSource(string soundPath,bool loop = false)
        {
            var audioSource = await SystemManager.Instance.ResourceManager.GetFromPool<AudioSource>(AudioObjectPath);
            audioSource.transform.SetParent(_poolRootTransform);
            Addressables.LoadAssetAsync<AudioClip>(soundPath).Completed += (obj) =>
            {
                audioSource.clip = obj.Result;
                audioSource.loop = loop;
                audioSource.Play();
                _audioSourceDictionary.Add(soundPath,audioSource);
            };
        }

        private void LoadAudioClip(string soundPath)
        {
            Addressables.LoadAssetAsync<AudioClip>(soundPath).Completed += (obj) =>
            {
                _soundSourceList.Add(obj);
                _audioSource.PlayOneShot(obj.Result);
            };
        }

        public void ReleaseAudio()
        {
            foreach (var data in _soundSourceList)
            {
                Addressables.Release(data);
            }
            _soundSourceList.Clear();
        }
    }
}
