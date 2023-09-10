using System;
using QT.Core;
using QT.InGame;
using QT.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace QT.Map
{
    public class EnemySpawner : MonoBehaviour
    {
        private const float ReleaseTime = 2.0f;
        
        public float SpawnDelay;
        public int EnemyId;
        
        public Enemy Target { get; private set; }
        
        private string _enemyPrefabPath;
        private UnityAction _onDeadEvent;
        
        private void Start()
        {
            Spawn();
        }

        public void Spawn(UnityAction onDeadEvent = null)
        {
            StartCoroutine(Util.UnityUtil.WaitForFunc(SpawnEnemy, SpawnDelay));
        }

        private async void SpawnEnemy()
        {
            var data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(EnemyId);

            if (data == null)
            {
                Debug.LogError($"{EnemyId} EnemyData를 찾을 수 없습니다.");
                return;
            }

            _enemyPrefabPath = data.PrefabPath;
            
            Target = await SystemManager.Instance.ResourceManager.GetFromPool<Enemy>(_enemyPrefabPath, transform);
            
            Target.initialization(EnemyId);
            
            Target.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Target.transform.localScale = Vector3.one;

            Target.OnDamageEvent.AddListener(OnDeadEvent);
        }

        private void OnDeadEvent(Vector2 dir, float power, AttackType attackType)
        {
            if (!Target.IsDead)
            {
                return;
            }
            
            Target.OnDamageEvent.RemoveListener(OnDeadEvent);
            Target = null;
            
            _onDeadEvent?.Invoke();
          
            //StartCoroutine(UnityUtil.WaitForFunc(()=>SystemManager.Instance.ResourceManager.ReleaseObject(_enemyPrefabPath, Target),ReleaseTime));
        }
        

#if UNITY_EDITOR

        [NonSerialized] 
        public Color _waveColor;

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
