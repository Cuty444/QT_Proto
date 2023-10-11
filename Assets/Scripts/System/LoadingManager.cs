using System.Collections;
using QT.Core;
using QT.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QT
{
    public class LoadingManager
    {
        private const float WaitTime = 1.5f;
        
        public bool IsMapLoad { get; private set; }

        public LoadingManager()
        {
            IsMapLoad = false;
            
            SceneManager.sceneLoaded += OnSceneUnloaded;
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
        
        private void OnSceneUnloaded(Scene scene, LoadSceneMode mode)
        {
            SystemManager.Instance.ResourceManager.AllReleasedObject();
        }
    }
}
