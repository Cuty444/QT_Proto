using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT
{
    public class EnemySpawner : MonoBehaviour
    {
        public int EnemyId;
        
        public Enemy Target { get; private set; }

        private void Start()
        {
            SetTarget();
        }

        private async void SetTarget()
        {
            var data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(EnemyId);

            if (data == null)
            {
                Debug.LogError($"{EnemyId} EnemyData를 찾을 수 없습니다.");
                return;
            }

            Target = await SystemManager.Instance.ResourceManager.GetFromPool<Enemy>(data.PrefabPath, transform);
            
            Target.initialization(EnemyId);
            
            Target.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Target.transform.localScale = Vector3.one;
        }

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
