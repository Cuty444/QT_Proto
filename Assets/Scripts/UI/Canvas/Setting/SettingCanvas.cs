using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.UI;
using QT.Util;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class SettingCanvas : UIPanel
    {
        [field:Space]
        [field:SerializeField] public TweenAnimator PopAnimator { get; private set; }
        [field:SerializeField] public TweenAnimator ReleaseAnimator { get; private set; }
        
        [field:Space]
        [field:SerializeField] public Slider MasterVolume { get; private set; }
        [field:SerializeField] public Slider BGMVolume { get; private set; }
        [field:SerializeField] public Slider SFXVolume { get; private set; }
        
        [field:Space]
        [field:SerializeField] public Slider ViveStrength { get; private set; }
        
        [field:Space]
        [field:SerializeField] public Button TitleButton { get; private set; }
        [field:SerializeField] public Button TutorialButton { get; private set; }
    }


    public class SettingCanvasModel : UIModelBase
    {
        public override UIType UIType => UIType.Popup;
        public override string PrefabPath => "Setting.prefab";
        
        private UIInputActions _inputActions;
        private SettingCanvas _settingCanvas;
        private SoundManager _soundManager;

        private float _lastTimeScale = 1;
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _settingCanvas = UIView as SettingCanvas;
            
            
            _soundManager = SystemManager.Instance.SoundManager;
            
            _soundManager.GetMasterVolume(out var volume);
            _settingCanvas.MasterVolume.value = volume;
            _settingCanvas.MasterVolume.onValueChanged.AddListener(_soundManager.SetMasterVolume);

            _soundManager.GetBGMVolume(out volume);
            _settingCanvas.BGMVolume.value = volume;
            _settingCanvas.BGMVolume.onValueChanged.AddListener(_soundManager.SetBGMVolume);
            
            _soundManager.GetSFXVolume(out volume);
            _settingCanvas.SFXVolume.value = volume;
            _settingCanvas.SFXVolume.onValueChanged.AddListener(_soundManager.SetSFXVolume);
            
            
            _inputActions = new UIInputActions();

            _inputActions.UI.Escape.started += (x) => ReleaseUI();
        }

        public override void Show()
        {
            _inputActions.Enable();

            _settingCanvas.StopAllCoroutines();

            base.Show();

            _soundManager.PlayOneShot(_soundManager.SoundData.UITabSFX);

            if (Time.timeScale != 0)
            {
                _lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }

            _settingCanvas.ReleaseAnimator.Pause();
            _settingCanvas.PopAnimator.ReStart();
        }

        public override void ReleaseUI()
        {
            _inputActions.Disable();
            _soundManager.VolumeSave();

            if (!_settingCanvas.gameObject.activeInHierarchy)
            {
                return;
            }

            _settingCanvas.StopAllCoroutines();

            Time.timeScale = _lastTimeScale;

            _settingCanvas.PopAnimator.Pause();
            _settingCanvas.ReleaseAnimator.ReStart();
            _settingCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(),
                _settingCanvas.ReleaseAnimator.SequenceLength));
        }
        
    }
}
