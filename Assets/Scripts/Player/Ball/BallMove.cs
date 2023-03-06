using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public float BulletSpeed { get; set; }
    public float BulletRange { get; set; }
    public bool IsShot { get; set; }

    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ForceChange();
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

    private void Update()
    {
        if (!IsShot)
            return;
        //if (BulletRange <= Vector2.Distance(StartPosition, transform.position))
        //{
        //    Destroy(gameObject);
        //}
    }
}
