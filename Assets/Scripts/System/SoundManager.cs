using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace QT.Sound
{

    public class SoundManager
    {
        private StudioEventEmitter _bgmEmitter;
        
        private Bus _masterBus;
        private Bus _bgmBus;
        private Bus _sfxBus;

        private Dictionary<EventReference, StudioEventEmitter> _sfxLoopDictionary =
            new Dictionary<EventReference, StudioEventEmitter>();

        private Transform _poolRootTransform;

        [HideInInspector] public SoundPathData SoundData;
        public void Initialize()
        {
            _poolRootTransform = new GameObject("AudioSourcePoolRoot").transform;
            Addressables.LoadAssetAsync<SoundPathData>("Assets/Data/SoundPathData.asset").Completed += (obj) =>
            {
                SoundData = obj.Result;
                //_masterBus = RuntimeManager.GetBus(SoundData.Bank[0]); //TODO : Bank 상의 후 넣어야함
                //_bgmBus = RuntimeManager.GetBus(SoundData.Bank[1]);
                //_sfxBus = RuntimeManager.GetBus(SoundData.Bank[2]);
            };
            var bgm = GameObject.Instantiate(new GameObject(), _poolRootTransform);
            _bgmEmitter = bgm.AddComponent<StudioEventEmitter>();
            bgm.name = "BGMEmitter";
            GameObject.DontDestroyOnLoad(_poolRootTransform);
        }


        public void PlayOneShot(EventReference data,Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(data, position == default ? Camera.main.transform.position : position);
        }

        public void PlaySFX(EventReference data)
        {
            if (_sfxLoopDictionary.TryGetValue(data, out var value))
            {
                value.ChangeEvent(data);
                value.Play();
            }
            else
            {
                var sfx = GameObject.Instantiate(new GameObject(), _poolRootTransform);
                sfx.name = data.Path;
                var sfxEmitter = sfx.AddComponent<StudioEventEmitter>();
                _sfxLoopDictionary.Add(data,sfxEmitter);
                sfxEmitter.ChangeEvent(data);
                sfxEmitter.Play();
            }
        }
        
        public void StopSFX(EventReference data,bool fadeOut = false)
        {
            if (_sfxLoopDictionary.ContainsKey(data))
            {
                _sfxLoopDictionary[data].AllowFadeout = fadeOut;
                _sfxLoopDictionary[data].Stop();
            }
        }

        public void PlayBGM(EventReference data, bool isFade = false)
        {
            ChangeBGM(data, isFade);
            _bgmEmitter.Play();
        }
        public void SetPauseBGM(bool pause) => _bgmEmitter.SetPause(pause);

        public void ChangeBGM(EventReference data,bool isFade = false)
        {
            _bgmEmitter.ChangeEvent(data,isFade);
        }
        
        public void StopBGM(bool fadeOut = false)
        {
            _bgmEmitter.AllowFadeout = fadeOut;
            _bgmEmitter.Stop();
        }
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);
        public void SetBGMVolume(float value) => _bgmBus.setVolume(value);
        public void SetSFXVolume(float value) => _sfxBus.setVolume(value);
    }
}
