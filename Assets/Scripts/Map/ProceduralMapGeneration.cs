using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QT.Map
{
    public class ProceduralMapGeneration : MonoBehaviour
    {
        [SerializeField] private int _mapWidth = 11;
        [SerializeField] private int _mapHeight = 7;
        [Range(0.0f,1.0f)]
        [SerializeField] private float _manyPathCorrection = 1.0f;

        private void Awake()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            QT.Util.RandomSeed.SeedSetting();
            Vector2 startPos = new Vector2(_mapWidth / 2, _mapHeight / 2);
            
        }

        private void RoomCreate(Vector2 pos)
        {
            
        }
    }
}
