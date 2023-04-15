using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Map;
using UnityEngine;

namespace QT.Map
{
    public class MapCellData : MonoBehaviour
    {
        [SerializeField] private Transform[] _doorTransforms;
        [SerializeField] private Transform[] _doorExitTransforms;

        private PlayerManager _playerManager;
        private RoomType RoomType;
        public GameObject Door;

        private void Awake()
        {
            for (int i = 0; i < _doorTransforms.Length; i++)
            {
                _doorTransforms[i].gameObject.SetActive(false);
                var door = Instantiate(Door, _doorTransforms[i]);
                door.transform.localPosition = Vector3.zero;
            }
            
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        public void OpenDoorDirection(MapDirection mapDirection)
        {
            _doorTransforms[0].gameObject.SetActive((mapDirection & MapDirection.Up) != 0);
            _doorTransforms[1].gameObject.SetActive((mapDirection & MapDirection.Down) != 0);
            _doorTransforms[2].gameObject.SetActive((mapDirection & MapDirection.Left) != 0);
            _doorTransforms[3].gameObject.SetActive((mapDirection & MapDirection.Right) != 0);
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

            Camera.main.GetComponent<PlayerChasingCamera>().SetBeforePosition( new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z));
        }
    }
}
