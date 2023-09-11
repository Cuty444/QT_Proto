using System;
using QT.Core;
using QT.InGame;
using QT.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace QT.Map
{
    public class EnemySpawner : MonoBehaviour
    {
        public float SpawnDelay;
        public int EnemyId;
        
        [field:SerializeField] public Enemy Target { get; private set; }
        
        private UnityAction _onDeadAction;
        
        
        public void Spawn(UnityAction onDeadAction = null)
        {
            _onDeadAction = onDeadAction;
            StartCoroutine(UnityUtil.WaitForFunc(SpawnEnemy, SpawnDelay));
        }

        private async void SpawnEnemy()
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
            Target.PrefabPath = data.PrefabPath;

            enabled = true;
        }

        private void Update()
        {
            if (Target == null)
            {
                return;
            }

            if (Target.IsDead)
            {
                _onDeadAction?.Invoke();
                Target = null;
                
                enabled = false;
            }
        }


#if UNITY_EDITOR

        [NonSerialized] 
        public Color _waveColor;

        private void OnValidate()
        {
            _waveColor = transform.parent.GetComponent<EnemyWave>().WaveColor;
        }

        private void OnDrawGizmos()
        {
            if (Target != null)
            {
                return;
            }
            
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

            Gizmos.color = _waveColor;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"{EnemyId} {display}");
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            
            // Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            // if(agroRange > 0)
            //     Gizmos.DrawWireSphere(transform.position, agroRange);
        }
#endif
    }
}
