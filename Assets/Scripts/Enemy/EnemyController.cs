using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyCollision _enemyCollision;
    [SerializeField] private EnemyTrigger _enemyTrigger;
    [SerializeField] private EnemyHP _enemyHP;
    [SerializeField] private EnemyFSM _enemyFSM;
    [SerializeField] private PhysicsMaterial2D _physicsMaterial2D;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private TrailRenderer _trailRenderer;
    private BallMove _ballMove;
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
        _rigidbody2D.mass = 1;
        _rigidbody2D.sharedMaterial = _physicsMaterial2D;
        gameObject.layer = LayerMask.NameToLayer("BallHit");
        _ballMove = gameObject.AddComponent<BallMove>();
        gameObject.AddComponent<BallCollsion>();
    }

}
