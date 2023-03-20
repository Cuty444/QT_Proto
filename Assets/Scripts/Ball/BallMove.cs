using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Ball
{

    public class BallMove : MonoBehaviour
    {
        #region StartData_Declaration

        private float _minVelocity;

        #endregion

        #region Global_Declaration

        private Rigidbody2D _rigidbody2D;

        private bool _swingBallHit = false;
        public float BulletSpeed { get; set; }

        #endregion


        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            GlobalDataSystem dataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            GetComponent<CircleCollider2D>().radius = dataSystem.BallTable.BallColliderRad;
            _rigidbody2D.drag = dataSystem.GlobalData.BallSpdDecelerationValue;
            _rigidbody2D.sharedMaterial.bounciness = dataSystem.GlobalData.BounceSpdReductionRate;
            _minVelocity = dataSystem.GlobalData.BallMinSpdDestroyed;
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

        public void EnemyToBallSpdCheck()
        {
            StartCoroutine(BallMinSpdCheck());
        }

        private IEnumerator BallMinSpdCheck()
        {
            yield return new WaitForSeconds(1f);
            WaitForSeconds wfs = new WaitForSeconds(0.3f);
            bool isShot = true;
            while (isShot)
            {
                if (_rigidbody2D.velocity.magnitude < _minVelocity)
                {
                    if (_swingBallHit)
                    {
                        _swingBallHit = false;
                        yield return wfs;
                        continue;
                    }

                    Destroy(gameObject);
                    isShot = false;
                }

                yield return null;
            }
        }

        public void SwingBallHit()
        {
            _swingBallHit = true;
        }
    }
}