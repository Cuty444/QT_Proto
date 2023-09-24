using System;
using System.Collections;
using System.Collections.Generic;
using QT.Util;
using UnityEngine;

namespace QT.Map
{
    public class EnemyWave : MonoBehaviour
    {
        private const float SpawnDelay = 0.2f;
        
        public MapCellData CellData;
        public EnemyWave NextWave;
        
        private EnemySpawner[] _spawners;
        
        private int _spawnedCount;

        private void Awake()
        {
            _spawners = GetComponentsInChildren<EnemySpawner>();
            _spawners.Shuffle();
        }

        public void Spawn()
        {
            _spawnedCount = _spawners.Length;
            for (var i = 0; i < _spawners.Length; i++)
            {
                var spawner = _spawners[i];
                spawner.SpawnDelay = SpawnDelay * (i + 1);
                spawner.Spawn(OnDead);
            }

            if (_spawnedCount <= 0)
            {
                OnDead();
            }
        }

        private void OnDead()
        {
            _spawnedCount -= 1;
          
            Debug.Log((_spawnedCount));

            if (_spawnedCount <= 0)
            {
                if (NextWave == null)
                {
                    CellData?.ClearRoom();
                }
                else
                {
                    NextWave.Spawn();
                }
            }
        }
        
      #if UNITY_EDITOR
        
        public Color WaveColor;
        
        
        private void OnDrawGizmos()
        {
            _spawners = GetComponentsInChildren<EnemySpawner>();

            foreach (var spawner in _spawners)
            {
                
            }
        }
        
#endif
    }
}
