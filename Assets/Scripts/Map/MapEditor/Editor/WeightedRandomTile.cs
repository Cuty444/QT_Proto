using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QT
{
    [Serializable]
    public struct WeightedSprite 
    {
        public Sprite Sprite;
        public int Weight;
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "WeightedRandomTile", menuName = "2D/Tiles/WeightedRandomTile")]
    public class WeightedRandomTile : TileBase
    {
        [SerializeField] public WeightedSprite[] Sprites;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            
            if (Sprites == null || Sprites.Length <= 0) return;
            
            var oldState = Random.state;

            Random.InitState(position.x * position.y);

            var cumulativeWeight = 0;
            foreach (var spriteInfo in Sprites)
            {
                cumulativeWeight += spriteInfo.Weight;
            }
            
            var randomWeight = Random.Range(0, cumulativeWeight);
            
            foreach (var spriteInfo in Sprites) 
            {
                randomWeight -= spriteInfo.Weight;
                if (randomWeight < 0) 
                {
                    tileData.sprite = spriteInfo.Sprite;    
                    break;
                }
            }
            
            Random.state = oldState;
        }
    }
}

