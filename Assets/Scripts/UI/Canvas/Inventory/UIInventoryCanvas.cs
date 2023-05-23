using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.Core.Map;
using UnityEngine;

namespace QT.UI
{
    public class UIInventoryCanvas : UIPanel
    {
        [SerializeField] private Transform _miniMapCellTransform;
        [SerializeField] private GameObject _miniMapOnOff;
        
        private PlayerManager _playerManager;
        
        private Dictionary<Vector2Int, MapDirection> _pathDirections = new Dictionary<Vector2Int, MapDirection>();
        private MapData _mapData;

        private List<MiniMapCellData> _cellList = new List<MiniMapCellData>();

        private Vector2Int _currentPlayerPosition; // TODO : DungeonMapSystem으로 옮겨야함
        public override void Initialize()
        {
     
        }

    }
}
