using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Smash)]
    public class DullahanSmashState : FSMState<Dullahan>
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");

        private const string SmashEffectPath = "Effect/Prefabs/FX_Boss_Smash.prefab";
        
        private List<EnemyAtkGameData> _atkList;
        private Coroutine _atkSeqence;
        
        private SoundManager _soundManager;
        
        public DullahanSmashState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.DullahanData.SmashAtkId);
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _atkSeqence = _ownerEntity.StartCoroutine(AttackSequence());
        }
        
        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.StopCoroutine(_atkSeqence);
        }
        
        private IEnumerator AttackSequence()
        {
            Vector2 dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);

            var side = _ownerEntity.SetDir(dir,2);
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.ShootPoints[Mathf.RoundToInt(side)];
            
            foreach (var data in _atkList)
            {
                if (data.BeforeDelay > 0)
                {
                    _ownerEntity.Animator.SetTrigger(AttackAnimHash);

                    yield return new WaitForSeconds(data.BeforeDelay);

                    _soundManager.PlayOneShot(_soundManager.SoundData.Boss_BatAttack, _ownerEntity.transform.position);
                    
                    SystemManager.Instance.ResourceManager.EmitParticle(SmashEffectPath,
                        _ownerEntity.Shooter.ShootPoint.position);
                    
                    _ownerEntity.AttackImpulseSource.GenerateImpulse(0.7f);
                }

                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType, ProjectileOwner.Boss);

                if (data.AfterDelay > 0)
                {
                    yield return new WaitForSeconds(data.AfterDelay);
                }
            }

            _ownerEntity.Animator.ResetTrigger(AttackAnimHash);
            
            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}