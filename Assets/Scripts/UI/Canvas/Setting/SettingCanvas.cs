using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
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
        public override UIType UIType => UIType.Panel;
        public override string PrefabPath => "Setting.prefab";

        private SettingCanvas _settingCanvas;
        
        
        public override void OnCreate(UIPanel view)
        {
            base.OnCreate(view);
            _settingCanvas = UIView as SettingCanvas;
        }
        
        public override void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Loading:
                    Show();
                    break;
                default:
                    ReleaseUI();
                    break;
            }
        }

        // public override void Show()
        // {
        //     base.Show();
        //     SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.LoadingBGM);
        //     
        //     _settingCanvas.StopAllCoroutines();
        //     _settingCanvas.CanvasGroup.DOFade(1, _settingCanvas.FadeInOutTime).SetEase(Ease.InQuad);
        // }
        //
        // public override void ReleaseUI()
        // {
        //     if (!_settingCanvas.gameObject.activeInHierarchy)
        //     {
        //         return;
        //     }
        //     
        //     _settingCanvas.CanvasGroup.DOFade(0, _settingCanvas.FadeInOutTime).SetEase(Ease.InQuad);;
        //     
        //     _settingCanvas.StopAllCoroutines();
        //     _settingCanvas.StartCoroutine(UnityUtil.WaitForFunc(() => base.ReleaseUI(), _settingCanvas.FadeInOutTime));
        // }
        
    }
}
