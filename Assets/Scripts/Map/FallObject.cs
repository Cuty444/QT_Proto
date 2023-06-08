using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class FallDirectionData
    {
        public MapDirection Direction;
        public Vector2 Position;

        public FallDirectionData(MapDirection direction,Vector2 position)
        {
            Direction = direction;
            Position = position;
        }
    }
    public class FallObject : MonoBehaviour
    {
        [HideInInspector] public Vector2Int CurrentPosition;
        private BoxCollider2D _boxCollider2D;
        private PlayerManager _playerManager;
        private List<Enemy> _enterEnemyList = new List<Enemy>();
        
        private List<GameObject> _enterPawnList = new List<GameObject>();

        private bool isFallCheck = false;
        LayerMask layerMask => LayerMask.GetMask("Wall", "HardCollider","CharacterCollider");
        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapPosition.AddListener(position =>
            {
                isFallCheck = CurrentPosition == position;
            });
        }

        private void FixedUpdate()
        {
            if (!isFallCheck)
                return;

            PlayerCheck();
        }
        
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

        private void PlayerCheck()
        {
            var player = _playerManager.Player;
            BoxCollisionCheck(player.gameObject,player.transform.position);
            BoxExitCollisionCheck(player.gameObject,player.transform.position);
        }
        private void BoxCollisionCheck(GameObject pawn, Vector2 targetPosition)
        {
            float halfX = _boxCollider2D.size.x * 0.5f;
            float halfY = _boxCollider2D.size.y * 0.5f;
            if (DistanceCheck(transform.position.x,halfX,targetPosition.x))
            {
                if (DistanceCheck(transform.position.y, halfY, targetPosition.y))
                {
                    if (pawn.TryGetComponent(out Player player))
                    {
                        player.IsFall = true;
                        player.EnterFallObject = this;
                        _enterPawnList.Add(player.gameObject);
                    }
                }
            }
        } 
        
        private void BoxExitCollisionCheck(GameObject pawn, Vector2 targetPosition)
        {
            if (!_enterPawnList.Contains(pawn))
                return;
            float halfX = _boxCollider2D.size.x * 0.5f;
            float halfY = _boxCollider2D.size.y * 0.5f;
            bool isExit = DistanceCheck(transform.position.x,halfX,targetPosition.x);

            if (!DistanceCheck(transform.position.y, halfY, targetPosition.y) && isExit)
            {
                isExit = false;
            }

            if (!isExit)
            {
                if (pawn.TryGetComponent(out Player player))
                {
                    player.IsFall = false;
                    _enterPawnList.Remove(player.gameObject);
                }
            }
        }
        private bool DistanceCheck(float a,float half,float b)
        {
            return a - half <= b && a + half >= b;
       
        }

        public Vector2 FallingExitDistanceCheck(Vector2 position)
        {
            Dictionary<float, FallDirectionData> positionDictionary = new Dictionary<float, FallDirectionData>();
            Vector2 exitPosition = Vector2.zero;
            float halfX = _boxCollider2D.size.x * 0.5f;
            float halfY = _boxCollider2D.size.y * 0.5f;
            var boxPosition = transform.position;
            var halfPosition = new Vector2(boxPosition.x + halfX, boxPosition.y);
            positionDictionary.Add(Vector2.Distance(halfPosition, position), new FallDirectionData(MapDirection.Right,halfPosition));
            halfPosition = new Vector2(boxPosition.x - halfX, boxPosition.y);
            positionDictionary.Add(Vector2.Distance(halfPosition, position), new FallDirectionData(MapDirection.Left,halfPosition));
            halfPosition = new Vector2(boxPosition.x, boxPosition.y - halfY);
            positionDictionary.Add(Vector2.Distance(halfPosition, position), new FallDirectionData(MapDirection.Down,halfPosition));
            halfPosition = new Vector2(boxPosition.x, boxPosition.y + halfY);
            positionDictionary.Add(Vector2.Distance(halfPosition, position), new FallDirectionData(MapDirection.Up,halfPosition));
            do
            {

                float distance = Single.MaxValue;
                foreach (var data in positionDictionary)
                {
                    if (distance > data.Key)
                    {
                        distance = data.Key;
                    }
                }

                var confirmedData = positionDictionary[distance];
                float correction = 1f;
                RaycastHit2D hit = new RaycastHit2D();
                switch (confirmedData.Direction)
                {
                    case MapDirection.Up:
                        exitPosition = new Vector2(position.x, confirmedData.Position.y + correction);
                        hit = Physics2D.Raycast(exitPosition, Vector2.up, correction, layerMask);
                        break;
                    case MapDirection.Down:
                        exitPosition = new Vector2(position.x, confirmedData.Position.y - correction);
                        hit = Physics2D.Raycast(exitPosition, Vector2.down, correction, layerMask);
                        break;
                    case MapDirection.Left:
                        exitPosition = new Vector2(confirmedData.Position.x - correction, position.y);
                        hit = Physics2D.Raycast(exitPosition, Vector2.left, correction, layerMask);
                        break;
                    case MapDirection.Right:
                        exitPosition = new Vector2(confirmedData.Position.x + correction, position.y);
                        hit = Physics2D.Raycast(exitPosition, Vector2.right, correction, layerMask);
                        break;
                }


                if (hit.collider == null)
                {
                    break;
                }
                else
                {
                    positionDictionary.Remove(distance);
                }
            } while (true);

            return exitPosition;
        }
        
    }
}
