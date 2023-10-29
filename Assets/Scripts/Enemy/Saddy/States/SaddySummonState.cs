using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Summon)]
    public class SaddySummonState : FSMState<Saddy>
    {
        private readonly int SummonAnimHash = Animator.StringToHash("IsSummon");
        
        private Coroutine _summonSequence;

        private SoundManager _soundManager;
        
        private SaddyData _data;
        
        public SaddySummonState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        { 
            if (_ownerEntity.MapData.BossWave == null || !_ownerEntity.MapData.BossWave.IsAvailable)
            {
                _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
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
            
            yield return new WaitForSeconds(_data.SummonReadyTime);
            _ownerEntity.MapData.BossWave.Spawn();
            
            //_soundManager.PlayOneShot(_soundManager.SoundData.Boss_Roar, _ownerEntity.transform.position);
            
            yield return new WaitForSeconds(_data.SummonTime);
            
            _ownerEntity.Animator.SetBool(SummonAnimHash, false);
            
            _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
        }
        
    }
}
