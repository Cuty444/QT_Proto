#if UNITY_EDITOR

using System.Collections;
using QT.Core;
using QT.InGame;
using UnityEditor;
using UnityEngine;


namespace QT.Map
{
    public class MapEditorSceneManager : MonoBehaviour
    {
        public MapCellData Target { get; private set; }

        private string _command;
        private Player _player;

        public void StartGame(string command)
        {
            _command = command;
            
            EditorApplication.EnterPlaymode();
        }
        
        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            CheckTarget();

            SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener(OnPlayerCreated);
            
            StartCoroutine(Loading());
        }

        IEnumerator Loading()
        {
            yield return new WaitUntil(() => SystemManager.Instance.LoadingManager.IsJsonLoad());
            
            SystemManager.Instance.PlayerManager.CreatePlayer();
        }

        private void OnPlayerCreated(Player player)
        {
            _player = player;
            Target.DoorExitDirection(Vector2Int.up);
        }
        

        public void CheckTarget()
        {
            var datas = FindObjectsOfType<MapCellData>();

            if (datas.Length == 1)
            {
                Target = datas[0];
            }
            else if (datas.Length > 1)
            {
                foreach (var data in datas)
                {
                    if (data == Target)
                    {
                        DestroyImmediate(data.gameObject);
                    }
                }
            }
        }
    }
}
#endif