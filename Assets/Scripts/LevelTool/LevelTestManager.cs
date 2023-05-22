using System;
using QT.Core;
using QT.InGame;
using QT.Level;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace QT
{
    public class LevelTestManager : MonoSingleton<LevelTestManager>
    {
        [SerializeField] private LevelTool _levelTool;
        [SerializeField] private Transform _tileTransform;
        [SerializeField] private Transform _enemyTransform;
        private const string tilePath = "Prefabs/Map/Tile.prefab";

        private Player _player = null;
        private Vector2 _spawnPosition = Vector2.zero;
        public Transform EnemyTransform => _enemyTransform;

        
        private Enemy[] _enemies;
        private void Awake()
        {
            _enemyTransform.gameObject.SetActive(false);
            PlayerManager _playerManager = SystemManager.Instance.PlayerManager;
            SystemManager.Instance.LoadingManager.DataAllLoadCompletedEvent.AddListener(() =>
            {
                _playerManager.CreatePlayer();
            });
            _playerManager.PlayerCreateEvent.AddListener(PlayerCreateCheck);
        }
        
        private void Start()
        {
            foreach (var tile in _levelTool.Data.Tiles)
            {
                TileCreate(tile);
            }
            
        }

        private async void TileCreate(LevelTestingData.TileData tileData)
        {
            var tile = await SystemManager.Instance.ResourceManager.GetFromPool<TileData>(tilePath,_tileTransform);
            tile.transform.position = tileData.PivotPositon;
            int layerValue = (int) Math.Log(tileData.LayerMask, 2);
            if (layerValue == 8)
            {
                if (_player)
                {
                    _player.transform.position = tileData.PivotPositon;
                }
                else
                {
                    _spawnPosition = tileData.PivotPositon;
                }
                layerValue = 0;
            }
            tile.gameObject.layer = layerValue;
            tile.SpriteRenderer.color = _levelTool.GetTileColor(tileData.LayerMask);
            if (tile.gameObject.layer != LayerMask.NameToLayer("Default"))
            {
                tile.gameObject.AddComponent<BoxCollider2D>();
            }
        }

        private void PlayerCreateCheck(Player player)
        {
            _player = player;
            if (_spawnPosition != Vector2.zero)
            {
                _player.transform.position = _spawnPosition;
            }

            //foreach (var enemy in _enemies)
            //{
            //    Instantiate(enemy.gameObject, enemy.transform.position, enemy.transform.rotation, _enemyTransform);
            //    enemy.gameObject.SetActive(false);
            //}
            _enemyTransform.gameObject.SetActive(true);
        }
    }
}
