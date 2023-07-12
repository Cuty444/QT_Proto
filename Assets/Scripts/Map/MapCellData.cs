using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

namespace QT.Map
{
    public class MapCellData : MonoBehaviour
    {
        [field: SerializeField] public Tilemap TilemapHardCollider { get; private set; }
        [field: SerializeField] public Tilemap TilemapTop { get; private set; }
        [field: SerializeField] public Tilemap Enemy { get; private set; }

        [SerializeField] private Transform[] _doorTransforms;
        [SerializeField] private Transform[] _doorExitTransforms;
        [SerializeField] private Transform _enemySpawnersTransform;

        [Header("카메라 외곽 제한 콜라이더")] [SerializeField]
        private Collider2D _collider2D;

        [Header("해당 맵 볼륨")] public VolumeProfile VolumeProfile;
        private RoomType _roomType;

        private PlayerManager _playerManager;
        private MapEnemySpawner _mapEnemySpawner;
        private List<DoorAnimator> _doorAnimators;
        private Vector2Int _position;

        private bool isDoorCreate;

        private void Awake()
        {
            _mapEnemySpawner = _enemySpawnersTransform.GetComponent<MapEnemySpawner>();
            _enemySpawnersTransform.gameObject.SetActive(false);
            for (int i = 0; i < _doorTransforms.Length; i++)
            {
                _doorTransforms[i].gameObject.SetActive(false);
                //var door = Instantiate(_door[i], _doorTransforms[i]);
                //door.transform.localPosition = Vector3.zero;
            }

            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapClearPosition.AddListener(RoomClear);
        }

        public void CellDataSet(MapDirection mapDirection,Vector2Int position,RoomType roomType)
        {
            _doorTransforms[0].gameObject.SetActive((mapDirection & MapDirection.Up) != 0);
            _doorTransforms[1].gameObject.SetActive((mapDirection & MapDirection.Down) != 0);
            _doorTransforms[2].gameObject.SetActive((mapDirection & MapDirection.Left) != 0);
            _doorTransforms[3].gameObject.SetActive((mapDirection & MapDirection.Right) != 0);
            _position = position;
            var fallObjects = GetComponentsInChildren<FallObject>();
            for (int i = 0; i < fallObjects.Length; i++)
            {
                fallObjects[i].CurrentPosition = _position;
            }
            if (TryGetComponent(out ShopMapData data))
            {
                data.MapPosition = position;
            }
            _roomType = roomType;
            NormalDoorCreate();
        }

        public void DoorExitDirection(Vector2Int enterDirection)
        {
            if (Vector2Int.up == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[1].position;
            }
            else if (Vector2Int.down == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[0].position;
            }
            else if (Vector2Int.left == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[2].position;
            }
            else if (Vector2Int.right == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[3].position;
            }
        }

        public void RoomPlay(Vector2Int position)
        {
            _mapEnemySpawner.SetPos(position);
            _playerManager.PlayerMapPass.Invoke(false);
            _enemySpawnersTransform.gameObject.SetActive(true);
        }

        public void SetCameraCollider2D()
        {
            _playerManager.PlayerDoorEnterCameraShapeChange.Invoke(_collider2D);
        }

        public void RoomClear(Vector2Int position)
        {
            if (_position == position)
            {
                for (int i = 0; i < _doorTransforms.Length; i++)
                {
                    _doorTransforms[i].GetComponentInChildren<DoorAnimator>().DoorOpen();
                }
            }

            SpecialDoorCreate();
        }

        private async void NormalDoorCreate()
        {
            string[] prefabPath = null;
            switch (_roomType)
            {
                case RoomType.GoldShop:
                case RoomType.HpShop:
                    prefabPath = Util.AddressablesDataPath.StoreDoorPaths;
                    break;
                case RoomType.Boss:
                    if (SystemManager.Instance.GetSystem<DungeonMapSystem>().GetFloor() < 2)
                    {
                        prefabPath = Util.AddressablesDataPath.DoorPaths;
                    }
                    else
                    {
                        prefabPath = Util.AddressablesDataPath.BossDoorPaths;
                    }
                    break;
                case RoomType.None:
                case RoomType.Normal:
                case RoomType.Start:
                default:
                    prefabPath = Util.AddressablesDataPath.DoorPaths;
                    break;
            }
            for (int i = 0; i < _doorTransforms.Length; i++)
            {
                var doorObject =
                    await SystemManager.Instance.ResourceManager.GetFromPool<DoorAnimator>(
                        prefabPath[i], _doorTransforms[i]);
                doorObject.transform.localPosition = Vector3.zero;
                if(_roomType == RoomType.Start)
                    doorObject.DoorOpen();
                if (i <= 1)
                {
                    doorObject.DoorUpDown((MapDirection)(1 << i));
                }
            }
        }

        private async void SpecialDoorCreate()
        {
            if (isDoorCreate)
                return;

            DungeonMapSystem dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            var data = dungeonMapSystem.DungeonMapData;
            int dirCount = 0;
            foreach (Vector2Int dir in QT.Util.UnityUtil.PathDirections)
            {
                RoomType nextRoomType = dungeonMapSystem.RoomCheck(_position - dir);
                if (nextRoomType == RoomType.GoldShop || nextRoomType == RoomType.HpShop)
                {
                    int beforeCount = dirCount;
                    if (dirCount == 2)
                    {
                        dirCount = 3;
                    }
                    else if (dirCount == 3)
                    {
                        dirCount = 2;
                    }
                    Destroy(_doorTransforms[dirCount].GetChild(0).gameObject);
                    var doorObject =
                        await SystemManager.Instance.ResourceManager.GetFromPool<DoorAnimator>(
                            Util.AddressablesDataPath.StoreDoorPaths[dirCount], _doorTransforms[dirCount]);
                    doorObject.transform.localPosition = Vector3.zero;
                    if (dirCount <= 1)
                    {
                        doorObject.DoorUpDown((MapDirection)(1 << dirCount));
                    }
                    dirCount = beforeCount;
                }
                if (nextRoomType == RoomType.Boss)
                {
                    if (SystemManager.Instance.GetSystem<DungeonMapSystem>().GetFloor() == 2)
                    {
                        int beforeCount = dirCount;
                        if (dirCount == 2)
                        {
                            dirCount = 3;
                        }
                        else if (dirCount == 3)
                        {
                            dirCount = 2;
                        }

                        Destroy(_doorTransforms[dirCount].GetChild(0).gameObject);
                        var doorObject =
                            await SystemManager.Instance.ResourceManager.GetFromPool<DoorAnimator>(
                                Util.AddressablesDataPath.BossDoorPaths[dirCount],
                                _doorTransforms[dirCount]); // TODO : 보스문 프리팹 설정해서 바꾸기
                        doorObject.transform.localPosition = Vector3.zero;
                        if (dirCount <= 1)
                        {
                            doorObject.DoorUpDown((MapDirection) (1 << dirCount));
                        }

                        dirCount = beforeCount;
                    }
                }
                dirCount++;
            }

            isDoorCreate = true;
        }
    }
}