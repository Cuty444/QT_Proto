using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using QT.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace QT.Map
{
    public class MapCellData : MonoBehaviour
    {
        [field:Header("타일맵 관련")]
        [field: SerializeField] public Tilemap TilemapWall { get; private set; }
        [field: SerializeField] public TilemapRenderer TilemapHardCollider { get; private set; }
        [field: SerializeField] public Tilemap TilemapTop { get; private set; }
        [field: SerializeField] public Transform EnemyLayer { get; private set; }

        
        [FormerlySerializedAs("EnemyWaveStart")] [field:Header("적 웨이브")]
        public EnemyWave[] EnemyWaves;
        
        [Header("문")] 
        [SerializeField] private Transform[] _doorTransforms;
        [SerializeField] private Transform[] _doorExitTransforms;

        [Header("맵 볼륨")] public VolumeProfile VolumeProfile;
        [Header("카메라 크기")] public float CameraSize = 7;

        private MapFloorChanger _changer;
        
        private RoomType _roomType;

        private PlayerManager _playerManager;
        private DungeonMapSystem _dungeonMapSystem;
        
        private List<Door> _doorAnimators = new();
        private Vector2Int _cellPosition;
        private Vector2Int _doorEnterDirection;

        private bool _isPlaying;
        
        private void Awake()
        {
            _playerManager = SystemManager.Instance.PlayerManager;
            _dungeonMapSystem = SystemManager.Instance.GetSystem<DungeonMapSystem>();
            _playerManager.PlayerDoorEnter.AddListener((direction) =>
            {
                _doorEnterDirection = direction;
            });
            _playerManager.PlayerMapPosition.AddListener(PlayerMapEnter);
            _playerManager.PlayerMapTeleportPosition.AddListener(TeleportCellPosition);

            _changer = GetComponentInChildren<MapFloorChanger>();
        }

        private void OnDestroy()
        {
            _playerManager.PlayerMapPosition.RemoveListener(PlayerMapEnter);
            _playerManager.PlayerMapTeleportPosition.RemoveListener(TeleportCellPosition);
        }


        public void CellDataSet(MapDirection mapDirection,Vector2Int position,RoomType roomType)
        {
            _doorTransforms[0].gameObject.SetActive((mapDirection & MapDirection.Up) != 0);
            _doorTransforms[1].gameObject.SetActive((mapDirection & MapDirection.Down) != 0);
            _doorTransforms[2].gameObject.SetActive((mapDirection & MapDirection.Left) != 0);
            _doorTransforms[3].gameObject.SetActive((mapDirection & MapDirection.Right) != 0);
            
            _cellPosition = position;
            
            if (TryGetComponent(out ShopMapData data))
            {
                data.MapPosition = position;
            }
            _roomType = roomType;
            CreateDoors();
            
            gameObject.SetActive(false);
        }


        public void PlayRoom(Vector2Int position)
        {
            _changer?.Spawn();

            _isPlaying = true;
            _playerManager.PlayerMapPass.Invoke(false);

            if (EnemyWaves is {Length: > 0})
            {
                EnemyWaves[0].Spawn();
            }
            else
            {
                ClearRoom();
            }
        }

        public void ClearRoom()
        {
            _playerManager.PlayerMapClearPosition.Invoke(_cellPosition);
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Door_OpenSFX);
            _playerManager.PlayerMapPass.Invoke(true);

            foreach (var door in _doorAnimators)
            {
                door.DoorOpen();
            }

            _isPlaying = false;
        }
        

        private async void CreateDoors()
        {
            if (_dungeonMapSystem == null)
            {
                return;
            }
            
            _doorAnimators = new();
            
            for (int i = 0; i < _doorTransforms.Length; i++)
            {
                RoomType nextRoomType = _dungeonMapSystem.RoomCheck(_cellPosition - Util.UnityUtil.PathDirections[i]);

                if (nextRoomType == RoomType.None)
                {
                    nextRoomType = _roomType;
                }
                
                var path = Util.AddressablesDataPath.GetDoorPath(nextRoomType)[i];
                var doorObject = await SystemManager.Instance.ResourceManager.GetFromPool<Door>(path, _doorTransforms[i]);
                doorObject.transform.localPosition = Vector3.zero;

                doorObject.Init(Util.UnityUtil.PathDirections[i], ExitDoor);
                
                if (_roomType == RoomType.Start)
                {
                    doorObject.DoorOpen();
                }
                
                _doorAnimators.Add(doorObject);
            }
        }

        private void PlayerMapEnter(Vector2Int enterPosition)
        {
            if (_cellPosition != enterPosition)
                return;
            
            gameObject.SetActive(true);
            
            if(!_dungeonMapSystem.GetCellData(_cellPosition).IsClear)
                PlayRoom(enterPosition);
            
            DoorExitDirection(_doorEnterDirection);
            
            _playerManager.OnMapCellChanged.Invoke(VolumeProfile, CameraSize);
        }
        
        public void DoorExitDirection(Vector2Int enterDirection) // TODO : 여기 추후에 유니티 이벤트 부분 던전 매니저로 변경 필요
        {
            if (Vector2Int.up == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[1].position;
                _playerManager.Player.LastSafePosition = _doorExitTransforms[1].position;
            }
            else if (Vector2Int.down == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[0].position;
                _playerManager.Player.LastSafePosition = _doorExitTransforms[0].position;
            }
            else if (Vector2Int.left == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[2].position;
                _playerManager.Player.LastSafePosition = _doorExitTransforms[2].position;
            }
            else if (Vector2Int.right == enterDirection)
            {
                _playerManager.Player.transform.position = _doorExitTransforms[3].position;
                _playerManager.Player.LastSafePosition = _doorExitTransforms[3].position;
            }
        }

        private void ExitDoor(Vector2Int direction)
        {
            gameObject.SetActive(false);
            SystemManager.Instance.PlayerManager.PlayerDoorEnter.Invoke(direction);
        }

        private void TeleportCellPosition(Vector2Int position)
        {
            if (_cellPosition != position)
                return;
            _playerManager.Player.transform.position = _doorExitTransforms[1].position;
            _playerManager.Player.LastSafePosition = _doorExitTransforms[1].position;
        }
        
    }
}