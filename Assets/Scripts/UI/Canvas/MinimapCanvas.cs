using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using UnityEngine;
using UnityEngine.UI;

namespace QT.UI
{
    public class MinimapCanvas : UIPanel
    {
        [SerializeField] private Transform _miniMapCellTransform;
        [SerializeField] private GameObject _miniMapOnOff;
        
        [SerializeField] private UITweenAnimator _popAnimator;
        [SerializeField] private UITweenAnimator _releaseAnimator;
        
        private const string CellPath = "Prefabs/Map/MiniMap/Cell.prefab";
        
        private PlayerManager _playerManager;
        
        private Dictionary<Vector2Int, MapDirection> _pathDirections = new Dictionary<Vector2Int, MapDirection>();

        private List<MiniMapCellData> _cellList = new List<MiniMapCellData>();
        private Dictionary<MiniMapCellData, Vector2> _cellMapDictionary = new Dictionary<MiniMapCellData, Vector2>();
        private List<GameObject> _cellMapList = new List<GameObject>();
        private bool IsPreviousActive;
        

        public override void Initialize()
        {
            _pathDirections.Add(Vector2Int.up,MapDirection.Up);
            _pathDirections.Add(Vector2Int.down,MapDirection.Down);
            _pathDirections.Add(Vector2Int.right,MapDirection.Left);
            _pathDirections.Add(Vector2Int.left,MapDirection.Right);
            SystemManager.Instance.ResourceManager.CacheAsset(CellPath);
            IsPreviousActive = true;
            _miniMapOnOff.SetActive(false);
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener((position) =>
            {
                MiniMapCellCenterPositionChange(position);
                if (!SystemManager.Instance.GetSystem<DungeonMapSystem>().GetCellData(position).IsClear)
                {
                    IsPreviousActive = false;
                    _miniMapOnOff.SetActive(false);
                }
                StartCoroutine(Util.UnityUtil.WaitForFunc(MapCreate,0.05f));
            });
            _playerManager.PlayerDoorEnter.AddListener((arg) =>
            {
                StartCoroutine(Util.UnityUtil.WaitForFunc(MapCreate,0.05f));
            });
            _playerManager.PlayerCreateEvent.AddListener((player) =>
            {
                StartCoroutine(Util.UnityUtil.WaitForFunc(MapCreate,0.05f));
            });
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                IsPreviousActive = true;
                _miniMapOnOff.SetActive(true);
                _popAnimator.ReStart();
            });
            
            SystemManager.Instance.UIManager.InventoryInputCheck.AddListener((isActive) =>
            {
                if (isActive)
                {
                    MapCreate();
                    if (IsPreviousActive)
                    {
                        _miniMapOnOff.SetActive(false);
                        _popAnimator.PlayBackwards();
                    }
                }
                else if(!isActive && IsPreviousActive)
                {
                    _miniMapOnOff.SetActive(true);
                    _popAnimator.ReStart();
                }
            });
        }
        


        public void CellClear()
        {
            foreach (var cell in _cellList)
            {
                cell.ListenerClear();
                //SystemManager.Instance.ResourceManager.ReleaseObject(CellPath, cell);
                Destroy(cell.gameObject);
            }
            _cellList.Clear();
            _cellMapDictionary.Clear();
        }
        
        public override void PostSystemInitialize()
        {
            gameObject.SetActive(true);
        }
        
        
        public async void CellCreate(Vector2Int createPos,MapDirection direction,int index,RoomType roomType)
        {
            Vector3 pos = new Vector3(createPos.x * 200f, createPos.y * -200f, 0f);
            var cell = await SystemManager.Instance.ResourceManager.GetFromPool<MiniMapCellData>(CellPath,_miniMapCellTransform);
            cell.name = cell.name +"_"+ index.ToString();
            cell.transform.localScale = Vector3.one;
            cell.transform.localPosition = pos;
            cell.SetRouteDirection(direction);
            cell.CellPos = createPos;
            switch (roomType)
            {
                case RoomType.None:
                    break;
                case RoomType.Normal:
                    break;
                case RoomType.Boss:
                    cell.name = cell.name + "_Boss";
                    break;
                case RoomType.GoldShop:
                case RoomType.HpShop:
                    cell.name = cell.name + "_Shop";
                    break;
                case RoomType.Start:
                    cell.name = cell.name + "_Start";
                    break;
                case RoomType.Stairs:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
            }
            cell.SetRoomType(roomType);
            cell.Setting();
            _cellList.Add(cell);
            _cellMapDictionary.Add(cell,pos);
        }

        public void MiniMapCellCenterPositionChange(Vector2Int position)
        {
            Vector3 pos = new Vector3(position.x * 200f, position.y * -200f, 0f);
            _miniMapCellTransform.transform.localPosition = -pos;
        }

        private void MapCreate()
        {
            foreach (var cell in _cellMapList)
            {
                Destroy(cell.gameObject);
            }
            _cellMapList.Clear();
            var pool = SystemManager.Instance.UIManager.GetUIPanel<UIDiaryCanvas>().MapTransform;
            foreach (var cell in _cellMapDictionary)
            {
                var obj = Instantiate(cell.Key.gameObject, pool);
                Button teleportButton = null;
                if (cell.Key.gameObject.TryGetComponent(out teleportButton))
                {
                    var copyButton = obj.GetComponent<Button>();
                    copyButton.onClick.AddListener(cell.Key.CellTeleportEvent);
                }
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = cell.Value;
                _cellMapList.Add(obj);
            }
        }
    }
}
