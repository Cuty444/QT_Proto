using System;
using QT.Map;
using UnityEngine;

namespace QT
{
    public class JelloMapData : SpecialMapData,IBossClearEvent
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
        [SerializeField] private GameObject _caseterNPC;

        private void Start()
        {
            _caseterNPC.gameObject.SetActive(false);
        }

        public void BossClear()
        {
            _caseterNPC.gameObject.SetActive(true);
        }
    }
}
