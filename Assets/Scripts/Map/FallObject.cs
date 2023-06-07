using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    public class FallObject : MonoBehaviour
    {
        private BoxCollider2D _boxCollider2D;
        private PlayerManager _playerManager;
        private List<Enemy> _enterEnemyList = new List<Enemy>();
        // private void Awake()
        // {
        //     _boxCollider2D = GetComponent<BoxCollider2D>();
        //     _playerManager = SystemManager.Instance.PlayerManager;
        //
        // }
        //
        // private void FixedUpdate()
        // {
        //     EnemyCheck();
        // }
        //
        // private void EnemyCheck()
        // {
        //     var enemyList = _playerManager.Player._hitableList;
        //     for (int i = 0; i < enemyList.Count; i++)
        //     { 
        //         BoxCollisionCheck(enemyList[i],enemyList[i].Position);
        //     }
        //
        //     for (int i = 0; i < _enterEnemyList.Count; i++)
        //     {
        //         BoxExitCollisionCheck(_enterEnemyList[i],_enterEnemyList[i].transform.position);
        //     }
        // }
        //
        // private void BoxCollisionCheck(Enemy enemy, Vector2 targetPosition)
        // {
        //     float halfX = _boxCollider2D.size.x * 0.5f;
        //     float halfY = _boxCollider2D.size.y * 0.5f;
        //     if (DistanceCheck(transform.position.x,halfX,targetPosition.x))
        //     {
        //         if (DistanceCheck(transform.position.y, halfY, targetPosition.y))
        //         {
        //             enemy.IsFall = true;
        //             _enterEnemyList.Add(enemy);
        //         }
        //     }
        // }
        //
        // private void BoxExitCollisionCheck(Enemy enemy, Vector2 targetPosition)
        // {
        //     float halfX = _boxCollider2D.size.x * 0.5f;
        //     float halfY = _boxCollider2D.size.y * 0.5f;
        //     if (!DistanceCheck(transform.position.x,halfX,targetPosition.x))
        //     {
        //         enemy.IsFall = false;
        //         _enterEnemyList.Remove(enemy);
        //     }
        //     if (!DistanceCheck(transform.position.y, halfY, targetPosition.y))
        //     {
        //         enemy.IsFall = false;
        //         _enterEnemyList.Remove(enemy);
        //     }
        // }
        //
        // private bool DistanceCheck(float a,float half,float b)
        // {
        //     return a - half <= b && a + half >= b;
        //
        // }
    }
}
