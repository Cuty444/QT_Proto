using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Attack)]
    public class DullahanAttackState : FSMState<Dullahan>
    {
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        private const string BossAttackEffectPath = "Effect/Prefabs/FX_Boss_Slash.prefab";
        
        private Coroutine _atkSeqence;
        
        private SoundManager _soundManager;
        private DullahanData _data;
        
        public DullahanAttackState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.DullahanData;
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
            
            _ownerEntity.Animator.SetTrigger(AttackAnimHash);
            
            
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_BatAttack, _ownerEntity.transform.position);
            _ownerEntity.AttackImpulseSource.GenerateImpulse(0.5f);
            
            yield return new WaitForSeconds(_data.AttackBeforeDelay);
            
            var hitAbles = new List<IHitAble>();
            Vector2 pos = _ownerEntity.transform.position;
            HitAbleManager.Instance.GetInRange(pos, _data.AttackDistance, ref hitAbles);
            foreach (var hit in hitAbles)
            {
                if (hit != _ownerEntity)
                {
                    hit.Hit(hit.Position - pos, _data.AttackDamage);
                }
            }
            
            //SystemManager.Instance.ResourceManager.EmitParticle(BossAttackEffectPath, pos);
            
            yield return new WaitForSeconds(_data.AttackAfterDelay);
            
            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}