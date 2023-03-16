using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float BulletSpeed { get; set; }
    public float BulletRange { get; set; }
    public Vector2 StartPosition { get; set; }
    public bool IsShot { get; set; }
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Shot();
    }

    private void Shot()
    {
        if (!IsShot)
            return;
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;

        _rigidbody2D.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * BulletSpeed;
        if (BulletRange <= Vector2.Distance(StartPosition,transform.position))
        {
            Destroy(gameObject);
        }
    }
}
