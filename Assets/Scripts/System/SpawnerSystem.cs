using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Spawner
{

    public class SpawnerSystem : SystemBase
    {
        [SerializeField] private GameObject _enemyObject;
        [SerializeField] private Transform _enemyParentTransform;
        [SerializeField] private float _enemySpawnDelay = 10f;
        private SpriteRenderer[] enemy;

        public override void OnInitialized()
        {
            enemy = GetComponentsInChildren<SpriteRenderer>();
            StartCoroutine(QT.Util.UnityUtil.WaitForFunc(() => StartCoroutine(EnemySpawnerRepeat(_enemySpawnDelay)),0.1f));
        }

        private void EnemySpawn()
        {
            for (int i = 0; i < enemy.Length; i++)
            {
                Instantiate(_enemyObject, enemy[i].transform.position, Quaternion.identity, _enemyParentTransform);
                enemy[i].enabled = false;
            }
        }

        IEnumerator EnemySpawnerRepeat(float delay)
        {
            WaitForSeconds wfs = new WaitForSeconds(delay);
            while (true)
            {
                EnemySpawn();
                yield return wfs;
            }
        }
    }
}
