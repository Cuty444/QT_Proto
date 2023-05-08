using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Level;
using Unity.VisualScripting;
using UnityEngine;

namespace QT
{
    public class LevelTestManager : MonoSingleton<LevelTestManager>
    {
        [SerializeField] private LevelTool _levelTool;
        [SerializeField] private Transform _tileTransform;
        private const string tilePath = "Prefabs/Map/Tile.prefab";


        private void Awake()
        {
            SystemManager.Instance.LoadingManager.DataAllLoadCompletedEvent.AddListener(() =>
            {
                SystemManager.Instance.PlayerManager.OnPlayerCreate();
            });
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
            tile.gameObject.layer = (int)Math.Log(tileData.LayerMask,2);
            tile.SpriteRenderer.color = _levelTool.GetTileColor(tileData.LayerMask);
            if (tile.gameObject.layer != LayerMask.NameToLayer("Default"))
            {
                tile.AddComponent<BoxCollider2D>();
            }
        }
    }
}
