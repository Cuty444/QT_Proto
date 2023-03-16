using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using QT.Data;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public float BulletSpeed { get; set; }
    public float BulletRange { get; set; }
    public bool IsShot { get; set; }

    private Rigidbody2D _rigidbody2D;

    private float _minVelocity;
    private bool _swingBallHit = false;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        GetComponent<CircleCollider2D>().radius = dataSystem.BallTable.BallColliderRad;
        _rigidbody2D.drag = dataSystem.GlobalData.BallSpdDecelerationValue;
        //transform.localScale = new Vector3(dataSystem.BallTable.BallRad,dataSystem.BallTable.BallRad, transform.localScale.z);
        //GetComponent<TrailRenderer>().startWidth = dataSystem.BallTable.BallRad;
        _rigidbody2D.sharedMaterial.bounciness = dataSystem.GlobalData.BounceSpdReductionRate;
        _minVelocity = dataSystem.GlobalData.BallMinSpdDestroyed;
        SystemManager.Instance.GetSystem<PlayerSystem>().BatSwingBallHitEvent.AddListener(SwingBallHitEvent);
        if (gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            ForceChange();
            StartCoroutine(BallMinSpdCheck());
        }
    }

    public void ForceChange()
    {
        _rigidbody2D.velocity = Vector2.zero;
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        float xVelocity = Mathf.Cos(angle);
        float yVelocity = Mathf.Sin(angle);
        Vector2 dir = new Vector2(xVelocity, yVelocity);
        _rigidbody2D.AddForce(dir * BulletSpeed);
    }

    private void LateUpdate()
    {
        if (!IsShot)
            return;

        //if (BulletRange <= Vector2.Distance(StartPosition, transform.position))
        //{
        //    Destroy(gameObject);
        //}
    }

    public void EnemyToBallSpdCheck()
    {
        StartCoroutine(BallMinSpdCheck());
    }

    private IEnumerator BallMinSpdCheck()
    {
        yield return new WaitForSeconds(1f);
        while (IsShot)
        {
            if (_rigidbody2D.velocity.magnitude < _minVelocity)
            {
                if (_swingBallHit)
                {
                    _swingBallHit = false;
                    yield return new WaitForSeconds(0.3f);
                    continue;
                }
                SystemManager.Instance.GetSystem<PlayerSystem>().BallMinSpdDestroyedEvent.Invoke(gameObject);
                Destroy(gameObject);
                IsShot = false;
            }

            yield return null;
        }
    }

    private void SwingBallHitEvent()
    {
        _swingBallHit = true;
    }
}
