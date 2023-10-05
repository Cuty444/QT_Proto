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
        public UnityEvent DataMapLoadCompletedEvent { get; } = new();

        private bool _isMapLoad = false;
        
        public void GameOverOpen()
        {
        }
        
        public void LoadScene(int sceneIndex)
        {
            SystemManager.Instance.StartCoroutine(LoadAsynchronously(sceneIndex));
            
            SystemManager.Instance.UIManager.SetState(UIState.Loading);
        }
        
        private IEnumerator LoadAsynchronously(int sceneIndex)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            float StartLoadingTime = Time.time;
            
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                if (operation.progress < 0.9f)
                {
                    
                }
                else
                {
                    if (Time.time - StartLoadingTime > 2.5f && _isMapLoad)
                    {
                        operation.allowSceneActivation = true;
                        yield break;
                    }
                }
                yield return null;
            }
        }

        public void DataLoadCheck()
        {
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
    }
}
