using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        public override void PostSystemInitialize()
        {
            OnOpen();
            FadeIn();
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
            StartCoroutine(FadeCanvasGroup(_canvasGroup, 1.0f, 0.0f, _fadeInOutTime,()=>
            {
                _backGroundObject.SetActive(false);
                func?.Invoke();
            }));
        }

        public void FadeOut(Action func = null)
        {
            _backGroundObject.SetActive(true);
            StartCoroutine(FadeCanvasGroup(_canvasGroup, 0.0f, 1.0f, _fadeInOutTime,func));
        }
        
        IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration,Action func = null)
        {
            float startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                cg.alpha = Mathf.Lerp(start, end, (Time.time - startTime) / duration);
                yield return null;
            }
            cg.alpha = end;
            func?.Invoke();
        }

    }
}
