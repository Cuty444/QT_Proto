using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Ball;

namespace QT.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        #region Inspector_Definition
        [SerializeField] private EnemyCollision _enemyCollision;
        [SerializeField] private EnemyTrigger _enemyTrigger;
        [SerializeField] private EnemyHP _enemyHP;
        [SerializeField] private EnemyFSM _enemyFSM;
        [SerializeField] private EnemyAttack _enemyAttack;
        [SerializeField] private PhysicsMaterial2D _physicsMaterial2D;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private TrailRenderer _trailRenderer;
        #endregion
        #region Global_Declaration
        private BallMove _ballMove;
        #endregion

        public void EnemyToBall()
        {
            Destroy(_enemyHP);
            _ballMove.EnemyToBallSpdCheck();
            _trailRenderer.enabled = true;
        }

        public void EnemyStunSet()
        {
            Destroy(_enemyCollision);
            Destroy(_enemyTrigger.gameObject);
            Destroy(_enemyFSM);
            Destroy(_enemyAttack);
            _rigidbody2D.mass = 1;
            _rigidbody2D.sharedMaterial = _physicsMaterial2D;
            _rigidbody2D.velocity = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("BallHit");
            _ballMove = gameObject.AddComponent<BallMove>();
            gameObject.AddComponent<BallAttack>();
        }

    }
}