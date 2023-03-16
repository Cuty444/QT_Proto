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
        /// ��ǥ ��ġ�� �ٶ󺸴� ���� �� ���ϱ�
        /// </summary>
        /// <param name="originalPos">���� ��ǥ</param>
        /// <param name="targetPos">��ǥ ��ǥ</param>
        /// <returns></returns>
        public static float GetDegree(Vector2 originalPos, Vector2 targetPos)
        {
            float angleRadian = Mathf.Atan2(targetPos.y - originalPos.y, targetPos.x - originalPos.x);
            return 180 / Mathf.PI * angleRadian;
        }
    }
}
