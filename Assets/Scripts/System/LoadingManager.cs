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
        private const float WaitTime = 1.5f;
        
        public bool IsMapLoad { get; private set; }

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
            operation.allowSceneActivation = false;
            
            yield return new WaitForSeconds(WaitTime);
            
            operation.allowSceneActivation = true;
        }

        public void MapReLoad()
        {
            Debug.Log("맵 로드 시작");
            IsMapLoad = false;
        }

        public void IsMapLoaded()
        {
            IsMapLoad = true;
            Debug.Log("맵 로드 완료");
        }
    }
}
