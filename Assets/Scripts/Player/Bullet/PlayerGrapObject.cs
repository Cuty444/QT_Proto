using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapObject : MonoBehaviour
{
    public Vector2 StartPos { get; set; }
    public Transform EndPos { get; set; }
    public float GrapTime { get; set; }
    float currentTime = 0f;
    private void FixedUpdate()
    {
        if (currentTime >= GrapTime)
            return;
        currentTime += Time.deltaTime;
        if (currentTime >= GrapTime)
        {
            currentTime = GrapTime;
        }
        GetTimeScalePostion(currentTime);
        if(currentTime >= GrapTime)
        {
            transform.parent = EndPos;
            transform.localPosition = Vector2.zero;
        }
    }

    private Vector2 GetGrapSpeed(Vector2 EndPos, Vector2 StartPos, float time)
    {
        return new Vector2((EndPos.x - StartPos.x) / time, (EndPos.y - StartPos.y) / time);
    }

    private float FloatToNormalization(float value,float min,float max)
    {
        return (value - min) / (max - min);
    }

    public void GetTimeScalePostion(float time)
    {
        transform.position = StartPos;
        Vector2 EndPosCheck = new Vector2(EndPos.transform.position.x, EndPos.transform.position.y);
        Debug.Log("GrapSpeed : " + GetGrapSpeed(EndPosCheck, StartPos, GrapTime));
        Debug.Log("* °ª : " + GetGrapSpeed(EndPosCheck, StartPos, GrapTime) * GrapTime);
        transform.Translate(GetGrapSpeed(EndPosCheck, StartPos, GrapTime) * time);
    }
}
