using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public class EnemySpawner : MonoBehaviour
    {
        public int EnemyId;
        
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, EnemyId.ToString());
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
        #endif
    }
}
