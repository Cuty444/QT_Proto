using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.UI;
using QT.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace QT
{
    public class LoadingManager
    {
        public UnityEvent DataAllLoadCompletedEvent { get; } = new();

        private LoadingCanvas _loadingCanvas;
        private GameOverCanvas _gameOverCanvas;
        private FadeCanvas _fadeCanvas;

        private bool _isAllLoad = false;
        public void Initialize()
        {
            _loadingCanvas = SystemManager.Instance.UIManager.GetUIPanel<LoadingCanvas>();
            _gameOverCanvas = SystemManager.Instance.UIManager.GetUIPanel<GameOverCanvas>();
            _fadeCanvas = SystemManager.Instance.UIManager.GetUIPanel<FadeCanvas>();
            DataAllLoadCompletedEvent.AddListener(() => _isAllLoad = true);
        }

        public void GameOverOpen()
        {
            SystemManager.Instance.StartCoroutine(UnityUtil.WaitForFunc(()=>
                _fadeCanvas.AutoFadeOutIn(() =>
                {
                    _gameOverCanvas.OnOpen();
                }), 3f));
        }
        
        public void LoadScene(int sceneIndex, Action func = null)
        {
            _fadeCanvas.AutoFadeOutIn(() =>
            {
                func?.Invoke();
                SystemManager.Instance.StartCoroutine(LoadAsynchronously(sceneIndex));
            });
        }
        
        private IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            float StartLoadingTime = Time.time;
            _loadingCanvas.OnOpen();
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                if (operation.progress < 0.9f)
                {
                    
                }
                else
                {
                    if (Time.time - StartLoadingTime > 2.5f && _isAllLoad)
                    {
                        operation.allowSceneActivation = true;
                        _fadeCanvas.AutoFadeOutIn(() =>
                        {
                            _loadingCanvas.OnClose();
                        });
                        yield break;
                    }
                }
                yield return null;
            }
        }
    }
}
