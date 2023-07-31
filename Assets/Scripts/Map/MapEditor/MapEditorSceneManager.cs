#if UNITY_EDITOR

using System.Collections;
using System.Linq;
using IngameDebugConsole;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEditor;
using UnityEngine;


namespace QT.Map
{
    public enum TestStartDoor
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public class MapEditorSceneManager : MonoBehaviour
    {
        public MapCellData Target { get; private set; }

        [HideInInspector] [SerializeField] private string _command;
        [HideInInspector] [SerializeField] private TestStartDoor _startDoor;
        private Player _player;

        public void StartGame(string command, TestStartDoor startDoor)
        {
            _command = command;
            _startDoor = startDoor;
            
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

            Vector2Int exit = Vector2Int.up;
            switch (_startDoor)
            {
                case TestStartDoor.Left:
                    exit = Vector2Int.left;
                    break;
                case TestStartDoor.Right:
                    exit = Vector2Int.right;
                    break;
                case TestStartDoor.Down:
                    exit = Vector2Int.down;
                    break;
                case TestStartDoor.Up:
                    exit = Vector2Int.up;
                    break;

            }
            
            Target.DoorExitDirection(exit);
            Target.CellDataSet(MapDirection.All, Vector2Int.zero, RoomType.Normal);
            Target.PlayRoom(Vector2Int.zero);

            
            // MapCell 로딩이 끝나는 시간을 고려해야 함...
            StartCoroutine(Util.UnityUtil.WaitForFunc(() =>
            {
                foreach (var command in _command.Split('\n'))
                {
                    DebugLogConsole.ExecuteCommand(command);
                }
            }, 0.5f));
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