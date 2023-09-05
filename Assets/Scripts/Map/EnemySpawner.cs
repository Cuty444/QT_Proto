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
            string display = "";
            float agroRange = 0;


            if (EditorSystemManager.Instance.DataManager.IsInitialized)
            {
                var data = EditorSystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(EnemyId);

                if (data != null)
                {
                    display = data.Name;
                    agroRange = data.AgroRange;
                }
            }

            Gizmos.color = Color.red;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"{EnemyId} {display}");
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            
            // Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            // if(agroRange > 0)
            //     Gizmos.DrawWireSphere(transform.position, agroRange);
        }
#endif
    }
}
