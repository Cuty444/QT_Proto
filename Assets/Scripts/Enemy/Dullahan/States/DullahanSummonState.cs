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
        
        private Coroutine _summonSequence;
        
        private SoundManager _soundManager;
        private DullahanData _data;
        
        private EnemyWave _bossWave = null;
        
        public DullahanSummonState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.DullahanData;
            
#if UNITY_EDITOR

            if (DungeonManager.Instance == null)
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
            if (_bossWave == null && _bossWave.IsAvailable == false)
            {
                _ownerEntity.ChangeState(Dullahan.States.Normal);
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
            
            yield return new WaitForSeconds(_data.SummonTime);
            
            _ownerEntity.Animator.SetBool(SummonAnimHash, false);
            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}