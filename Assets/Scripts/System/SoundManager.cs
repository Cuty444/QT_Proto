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

        private EventReference _currentBgm;
        [HideInInspector] public SoundPathData SoundData;
        
        private string bgmKey = "bgmKey";
        private string sfxKey = "sfxKey";
        public void Initialize(SoundPathData soundPathData)
        {
            _poolRootTransform = new GameObject("AudioSourcePoolRoot").transform;
            SoundData = soundPathData;
            //_masterBus = RuntimeManager.GetBus(SoundData.Bank[0]); //TODO : Bank 상의 후 넣어야함
            //_bgmBus = RuntimeManager.GetBus(SoundData.Bank[1]);
            //_sfxBus = RuntimeManager.GetBus(SoundData.Bank[2]);
            var bgm = GameObject.Instantiate(new GameObject(), _poolRootTransform);
            _bgmEmitter = bgm.AddComponent<StudioEventEmitter>();
            bgm.name = "BGMEmitter";
            //_masterBus = RuntimeManager.GetBus(SoundData.Bank[0]);
            _bgmBus = RuntimeManager.GetBus(SoundData.Bank[1]);
            _sfxBus = RuntimeManager.GetBus(SoundData.Bank[2]);
            GameObject.DontDestroyOnLoad(_poolRootTransform);
            if (PlayerPrefs.HasKey(bgmKey))
            {
                _bgmBus.setVolume(PlayerPrefs.GetFloat(bgmKey));
            }

            if (PlayerPrefs.HasKey(sfxKey))
            {
                _sfxBus.setVolume(PlayerPrefs.GetFloat(sfxKey));
            }
        }

        public void VolumeSave()
        {
            float value = 0f;
            _bgmBus.getVolume(out value);
            PlayerPrefs.SetFloat(bgmKey, value);
            _sfxBus.getVolume(out value);
            PlayerPrefs.SetFloat(sfxKey,value);
            PlayerPrefs.Save();
        }

        public void PlayOneShot(EventReference data,Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(data, position == default ? Camera.main.transform.position : position);
        }

        public void PlaySFX(EventReference data, Vector3 position = default, Transform parent = null)
        {
            if (!_sfxLoopDictionary.TryGetValue(data, out var sfx))
            {
                sfx = GameObject.Instantiate(new GameObject()).AddComponent<StudioEventEmitter>();

                #if UNITY_EDITOR
                sfx.name = data.Path;
                #endif

                _sfxLoopDictionary.Add(data, sfx);
            }
            
            sfx.transform.parent = parent;
            sfx.transform.localPosition = position;
            
            sfx.ChangeEvent(data);
            sfx.Play();
        }
        
        public void StopSFX(EventReference data,bool fadeOut = false)
        {
            if (_sfxLoopDictionary.TryGetValue(data, out var sfx))
            {
                sfx.AllowFadeout = fadeOut;
                sfx.Stop();
                sfx.transform.parent = _poolRootTransform;
            }
        }
        
        public void PlayBGM(EventReference data, bool isFade = false)
        {
            if (!_currentBgm.IsNull)
            {
                if (data.Guid == _currentBgm.Guid)
                    return;
            }

            ChangeBGM(data, isFade);
            _bgmEmitter.Play();
        }
        public void SetPauseBGM(bool pause) => _bgmEmitter.SetPause(pause);

        public void ChangeBGM(EventReference data,bool isFade = false)
        {
            _bgmEmitter.ChangeEvent(data,isFade);
            _currentBgm = data;
        }
        
        public void StopBGM(bool fadeOut = false)
        {
            _bgmEmitter.AllowFadeout = fadeOut;
            _bgmEmitter.Stop();
        }
        public void SetMasterVolume(float value) => _masterBus.setVolume(value);
        public void SetBGMVolume(float value) => _bgmBus.setVolume(value);
        public void SetSFXVolume(float value)
        {
            if (value > 0f)
            {
                _sfxBus.setMute(false);
                _sfxBus.setVolume(value);
            }
            else
            {
                _sfxBus.setMute(true);
            }
        }

        public void GetMasterVolume(out float value) => _masterBus.getVolume(out value);
        public void GetBGMVolume(out float value) => _bgmBus.getVolume(out value);
        public void GetSFXVolume(out float value) => _sfxBus.getVolume(out value);
    }
}
