using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Map
{
    public class MapEnemySpawner : MonoBehaviour
    {
        private Enemy.Enemy[] _enemy;

        private void Awake()
        {
            _enemy = GetComponentsInChildren<Enemy.Enemy>();
        }
    }
}
