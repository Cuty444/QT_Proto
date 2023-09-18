using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QT
{
    
    [Serializable]
    [CreateAssetMenu(fileName = "CrossPatternTile", menuName = "2D/Tiles/CrossPatternTile")]
    public class CrossPatternTile : TileBase
    {
        [SerializeField] public Sprite Pattern1;
        [SerializeField] public Sprite Pattern2;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            var pos = position.x + position.y;
            
            if (pos % 2 == 0)
            {
                tileData.sprite = Pattern1;
            }
            else
            {
                tileData.sprite = Pattern2;
            }
        }
    }
}

