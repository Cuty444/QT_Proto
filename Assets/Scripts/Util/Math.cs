using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QT.Util
{
    public static class Math
    {
        public static float floatNormalization(float value,float max,float min)
        {
            return (value - min) / (max - min);
        }
    }
}
