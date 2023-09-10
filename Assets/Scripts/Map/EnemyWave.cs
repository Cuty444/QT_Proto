using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Map
{
    public class EnemyWave : MonoBehaviour
    {
        public MapCellData CellData;
        public EnemyWave NextWave;
        public float SpawnDelay;
        
        private EnemySpawner[] _spawners;
        
        private int _spawnedCount;

        private void Awake()
        {
            _spawners = GetComponentsInChildren<EnemySpawner>();
        }

        public void Spawn()
        {
            _spawnedCount = _spawners.Length;
            for (var i = 0; i < _spawners.Length; i++)
            {
                var spawner = _spawners[i];
                spawner.SpawnDelay = SpawnDelay * i;
                spawner.Spawn(OnDead);
            }

            if (_spawnedCount <= 0)
            {
                OnDead();
            }
        }

        private void OnDead()
        {
            _spawnedCount--;
            
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
