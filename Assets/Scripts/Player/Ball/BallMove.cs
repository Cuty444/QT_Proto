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

    public Rigidbody2D rb;

    private float _minVelocity;
    private bool _swingBallHit = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
        rb.drag = dataSystem.GlobalData.BallSpdDecelerationValue;
        //transform.localScale = new Vector3(dataSystem.BallTable.BallRad,dataSystem.BallTable.BallRad, transform.localScale.z);
        //GetComponent<TrailRenderer>().startWidth = dataSystem.BallTable.BallRad;
        rb.sharedMaterial.bounciness = dataSystem.GlobalData.BounceSpdReductionRate;
        _minVelocity = dataSystem.GlobalData.BallMinSpdDestroyed;
        SystemManager.Instance.GetSystem<PlayerSystem>().BatSwingBallHitEvent.AddListener(SwingBallHitEvent);
        ForceChange();
        StartCoroutine(BallMinSpdCheck());
    }

    public void ForceChange()
    {
        rb.velocity = Vector2.zero;
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        float xVelocity = Mathf.Cos(angle);
        float yVelocity = Mathf.Sin(angle);
        Vector2 dir = new Vector2(xVelocity, yVelocity);
        rb.AddForce(dir * BulletSpeed);
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

    private IEnumerator BallMinSpdCheck()
    {
        yield return new WaitForSeconds(1f);
        while (IsShot)
        {
            if (rb.velocity.magnitude < _minVelocity)
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
