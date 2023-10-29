using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Map;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Summon)]
    public class DullahanSummonState : FSMState<Dullahan>
    {
        private readonly int SummonAnimHash = Animator.StringToHash("IsSummon");
        
        private const string SummonEffectPath = "Effect/Prefabs/FX_Boss_Howling.prefab";
        
        private Coroutine _summonSequence;
        
        private SoundManager _soundManager;
        private DullahanData _data;
        
        private EnemyWave _bossWave = null;
        
        public DullahanSummonState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.DullahanData;
            
#if UNITY_EDITOR

            if (DungeonManager.Instance is DungeonManagerDummy)
            {
                _bossWave = GameObject.FindObjectOfType<BossMapData>(true).BossWave;
                return;
            }
            
#endif
            
            var mapCellData = DungeonManager.Instance.GetCurrentMapCellData();
            if(mapCellData != null && mapCellData.SpecialMapData != null)
            {
                _bossWave = (mapCellData.SpecialMapData as BossMapData).BossWave;
            }
        }

        public override void InitializeState()
        {
            if (_bossWave == null || _bossWave.IsAvailable == false)
            {
                _ownerEntity.ChangeState(Dullahan.States.Normal);
                return;
            }
            
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _summonSequence = _ownerEntity.StartCoroutine(SummonSequence());
        }
        
        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.StopCoroutine(_summonSequence);
        }
        
        private IEnumerator SummonSequence()
        {
            _ownerEntity.Animator.SetBool(SummonAnimHash, true);
            
            _bossWave.Spawn();
            
            yield return new WaitForSeconds(_data.SummonReadyTime);
            
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Roar, _ownerEntity.transform.position);
            SystemManager.Instance.ResourceManager.EmitParticle(SummonEffectPath, _ownerEntity.CenterTransform.position);
            
            yield return new WaitForSeconds(_data.SummonTime);
            
            _ownerEntity.Animator.SetBool(SummonAnimHash, false);
            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}