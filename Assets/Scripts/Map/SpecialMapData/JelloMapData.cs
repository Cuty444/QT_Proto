using System;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class JelloMapData : SpecialMapData
    {
        [field: SerializeField] public Transform MapCenter { get; private set; }

        // #if UNITY_EDITOR
        // private void OnDrawGizmos()
        // {
        //     if (MapCenter != null)
        //     {
        //         Gizmos.DrawWireSphere(MapCenter.position, 8);
        //     }
        // }
        // #endif
    }
}
