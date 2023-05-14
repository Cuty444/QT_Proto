using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Level
{
    [CreateAssetMenu(fileName = "LevelTestingData",menuName = "Level/LevelData")]
    public class LevelTestingData : ScriptableObject
    {
        [Serializable]
        public class TileData
        {
            public Vector2Int Position;
            public Vector3 PivotPositon
            {
                get => new Vector3(Position.x + 0.5f, Position.y + 0.5f, 0f);
            }
            public LayerMask LayerMask;
            
        }

        public List<TileData> Tiles = new List<TileData>();
    }
}
