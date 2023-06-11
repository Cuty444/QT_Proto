using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.UI
{
    public class FadeCanvas : UIPanel
    {
        private CanvasGroup _canvasGroup;

        [SerializeField] private GameObject _backGroundObject;
        [SerializeField] private float _fadeInOutTime = 0.5f;
        public override void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener((arg) =>
            {
                GetComponent<Canvas>().worldCamera = Camera.main;
            });
        }

        public override void PostSystemInitialize()
        {
            OnOpen();
        }

        public void AutoFadeOutIn(Action firstFunc, Action lastFunc = null,float time = 0f)
        {
            FadeOut(() =>
            {
                firstFunc?.Invoke();
                StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() =>
                {
                    FadeIn(lastFunc);
                }, time));
            });
        }
        
        public void FadeIn(Action func = null)
        {
            StartCoroutine(UnityUtil.FadeCanvasGroup(_canvasGroup, 1.0f, 0.0f, _fadeInOutTime,()=>
            {
                _backGroundObject.SetActive(false);
                func?.Invoke();
                SystemManager.Instance.PlayerManager.FadeInCanvasOut.Invoke();
            }));
        }

        public void FadeOut(Action func = null)
        {
            _backGroundObject.SetActive(true);
            StartCoroutine(UnityUtil.FadeCanvasGroup(_canvasGroup, 0.0f, 1.0f, _fadeInOutTime,func));
        }
        

    }
}
