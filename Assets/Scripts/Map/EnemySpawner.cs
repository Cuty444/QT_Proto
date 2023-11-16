using System;
using System.Collections;
using QT.Core;
using QT.InGame;
using QT.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace QT.Map
{
    public class EnemySpawner : MonoBehaviour
    {
        private const float DefaultSpawnDelay = 0.3f;
        private const string SummonMarkPrefabPath = "Effect/Prefabs/FX_Summons_Emblem.prefab";
        private const string SummonPrefabPath = "Effect/Prefabs/FX_Summons_Teleport.prefab";
        
        public float SpawnDelay;
        public int EnemyId;
        
        [field:SerializeField] public IEnemy Target { get; private set; }
        
        private UnityAction _onDeadAction;

        private SoundManager _soundManager;

        private void Awake()
        {
            _soundManager = SystemManager.Instance.SoundManager;
        }

        public void Spawn(UnityAction onDeadAction = null)
        {
            if (Target != null)
            {
                return;
            }
            
            _onDeadAction = onDeadAction;
            StartCoroutine(SpawnProcess());
        }

        public void Kill()
        {
            Target?.Hit(Vector2.zero, 99999);
            Target = null;
            
            enabled = false;
        }

        private IEnumerator SpawnProcess()
        {
            var position = transform.position;
            
            SystemManager.Instance.ResourceManager.EmitParticle(SummonMarkPrefabPath, position);
            yield return new WaitForSeconds(SpawnDelay);

            SystemManager.Instance.ResourceManager.EmitParticle(SummonPrefabPath, position);
            _soundManager.PlayOneShot(_soundManager.SoundData.Monster_Spawn, position);

            yield return new WaitForSeconds(DefaultSpawnDelay);

            SpawnEnemy();
        }
        
        private async void SpawnEnemy()
        {
            var data = SystemManager.Instance.DataManager.GetDataBase<EnemyGameDataBase>().GetData(EnemyId);

            if (data == null)
            {
                Debug.LogError($"{EnemyId} EnemyData를 찾을 수 없습니다.");
                return;
            }

            Target = (await SystemManager.Instance.ResourceManager.GetFromPool<Transform>(data.PrefabPath, transform)).GetComponent<IEnemy>();
            
            Target.Initialization(EnemyId);
            Target.PrefabPath = data.PrefabPath;
            
            var targetTransform = (Target as MonoBehaviour).transform;
            
            targetTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            targetTransform.localScale = Vector3.one;

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
        public EnemyWave _wave;

        private void OnValidate()
        {
            _wave = transform.parent.GetComponent<EnemyWave>();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
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

            Gizmos.color = _wave.WaveColor;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"{EnemyId} {display}");
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            
            // Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            // if(agroRange > 0)
            //     Gizmos.DrawWireSphere(transform.position, agroRange);
        }
#endif
    }
}
