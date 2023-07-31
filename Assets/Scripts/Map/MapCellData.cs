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
        [field: SerializeField] public GameObject EnemyLayer { get; private set; }

        [SerializeField] private Transform[] _doorTransforms;
        [SerializeField] private Transform[] _doorExitTransforms;
        [SerializeField] private Transform _enemySpawnersTransform;

        [Header("맵 볼륨")] public VolumeProfile VolumeProfile;
        private RoomType _roomType;

        private PlayerManager _playerManager;
        private MapEnemySpawner _mapEnemySpawner;
        private List<DoorAnimator> _doorAnimators;
        private Vector2Int _position;


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
            CreateDoors();
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
            _mapEnemySpawner?.SetPos(position);
            _playerManager.PlayerMapPass.Invoke(false);
            _enemySpawnersTransform.gameObject.SetActive(true);
        }


        public void RoomClear(Vector2Int position)
        {
            if (_position == position)
            {
                foreach (var door in _doorAnimators)
                {
                    door.DoorOpen();
                }
            }
        }


        private async void CreateDoors()
        {
            _doorAnimators = new();
            DungeonMapSystem dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            
            for (int i = 0; i < _doorTransforms.Length; i++)
            {
                RoomType nextRoomType = dungeonMapSystem.RoomCheck(_position - Util.UnityUtil.PathDirections[i]);

                if (nextRoomType == RoomType.None)
                {
                    nextRoomType = _roomType;
                }
                
                var path = Util.AddressablesDataPath.GetDoorPath(nextRoomType)[i];
                var doorObject = await SystemManager.Instance.ResourceManager.GetFromPool<DoorAnimator>(path, _doorTransforms[i]);
                doorObject.transform.localPosition = Vector3.zero;

                if (_roomType == RoomType.Start)
                {
                    doorObject.DoorOpen();
                }
                if (i <= 1)
                {
                    doorObject.DoorUpDown((MapDirection)(1 << i));
                }
                
                _doorAnimators.Add(doorObject);
            }
        }
        
    }
}