using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.UI;

namespace QT.Splash
{
    public class SplashCanvas : UIPanel
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _logoCanvansGroup;
        [SerializeField] private Image[] _logoImages;
        [SerializeField] private float _fadeInOutTime = 0.5f;

        private int index = 0;
        public override void OnOpen()
        {
            base.OnOpen();
            index = 0;
            SplashStart();
        }

        private void SplashStart()
        {
            StartCoroutine(AutoFade());
        }
        
        private IEnumerator AutoFade()
        {
            float startTime = Time.time;
            while (_logoImages.Length > index)
            {
                _logoImages[index].gameObject.SetActive(true);
                startTime = Time.time;
                while (Time.time - startTime < _fadeInOutTime)
                {
                    _logoCanvansGroup.alpha = Mathf.Lerp(0f, 1f, (Time.time - startTime) / _fadeInOutTime);
                    yield return null;
                }
                _logoCanvansGroup.alpha = 1f;
                startTime = Time.time;
                while (Time.time - startTime < _fadeInOutTime)
                {
                    _logoCanvansGroup.alpha = Mathf.Lerp(1f, 0f, (Time.time - startTime) / _fadeInOutTime);
                    yield return null;
                }
                _logoCanvansGroup.alpha = 0f;
                _logoImages[index++].gameObject.SetActive(false);
            }
            SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().RestartAnimation();
            startTime = Time.time;
            while (Time.time - startTime < 0.5f)
            {
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, (Time.time - startTime) / 0.5f);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.MainBGM);
            OnClose();
            //SystemManager.Instance.UIManager.GetUIPanel<TitleCanvas>().RestartAnimation();
        }
    }
}
