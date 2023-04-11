using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.UI
{
    public class LoadingCanvas : UIPanel
    {
        private CanvasGroup _canvasGroup;
        private Animation _animation;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        public void FadeIn()
        {
        }

        public void FadeOut()
        {
            
        }
        
        IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
        {
            float startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                cg.alpha = Mathf.Lerp(start, end, (Time.time - startTime) / duration);
                yield return null;
            }
            cg.alpha = end;
        }

    }
}
