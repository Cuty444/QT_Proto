using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QT.Util
{
    public static class UnityUtil
    {
        public static IEnumerator WaitForFunc(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func.Invoke();
        }
    }
    public static class Math
    {
        public static float floatNormalization(float value,float max,float min)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// 목표 위치를 바라보는 각도 값 구하기
        /// </summary>
        /// <param name="originalPos">시작 좌표</param>
        /// <param name="targetPos">목표 좌표</param>
        /// <returns></returns>
        public static float GetDegree(Vector2 originalPos, Vector2 targetPos)
        {
            float angleRadian = Mathf.Atan2(targetPos.y - originalPos.y, targetPos.x - originalPos.x);
            return 180 / Mathf.PI * angleRadian;
        }

        public static Vector2 ZAngleToGetDirection(Transform transform)
        {
            float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }
    }
}
