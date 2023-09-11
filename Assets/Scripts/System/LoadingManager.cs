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
        public UnityEvent DataJsonLoadCompletedEvent { get; } = new();
        public UnityEvent DataMapLoadCompletedEvent { get; } = new();

        private LoadingCanvas _loadingCanvas;
        private GameOverCanvas _gameOverCanvas;
        private FadeCanvas _fadeCanvas;

        private bool _isJsonLoad = false;
        private bool _isMapLoad = false;
        public void Initialize()
        {
            if (SystemManager.Instance.UIManager != null)
            {
                _loadingCanvas = SystemManager.Instance.UIManager.GetUIPanel<LoadingCanvas>();
                _gameOverCanvas = SystemManager.Instance.UIManager.GetUIPanel<GameOverCanvas>();
                _fadeCanvas = SystemManager.Instance.UIManager.GetUIPanel<FadeCanvas>();
            }
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

        public void FloorLoadScene(int sceneIndex, Action func = null)
        {
            func?.Invoke();
            SystemManager.Instance.StartCoroutine(LoadAsynchronously(sceneIndex));
        }
        
        public void LoadFadeOutScene(int sceneIndex, Action func = null)
        {
            _fadeCanvas.FadeOut(() =>
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
                    if (Time.time - StartLoadingTime > 2.5f && IsAllLoad())
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

        public void DataLoadCheck()
        {
            DataJsonLoadCompletedEvent.AddListener(() =>
            {
                _isJsonLoad = true;
                SystemManager.Instance.UIManager.GetUIPanel<FadeCanvas>()?.FadeIn();
                Debug.Log("Json Data Load Completed");
            });
            DataMapLoadCompletedEvent.AddListener(() =>
            {
                _isMapLoad = true;
                Debug.Log("Map Data Load Completed");
            });
        }

        public void MapReLoad()
        {
            _isMapLoad = false;
        }

        private bool IsAllLoad()
        {
            return _isJsonLoad && _isMapLoad;
        }

        public bool IsJsonLoad()
        {
            return _isJsonLoad;
        }
    }
}
