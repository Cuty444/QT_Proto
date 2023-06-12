using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Dead)]
    public class DullahanDeadState : FSMState<Dullahan>
    {
        private readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");
        private const string BossEndEffectPath = "Effect/Prefabs/FX_Sunising_Sun.prefab";
        private const string BossDeadEffectPath = "Effect/Prefabs/FX_Boss_Dead.prefab";
        
        private float _time;
        private bool _explosion;
        
        public DullahanDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _time = 0;
            _explosion = false;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, true);

            SystemManager.Instance.ResourceManager.EmitParticle(BossEndEffectPath, _ownerEntity.CenterTransform.position);
            _ownerEntity.DeadImpulseSource.GenerateImpulse(1);
        }

        public override void UpdateState()
        {
            if (_explosion)
            {
                return;
            }
            
            _time += Time.deltaTime;

            if (_time > 5)
            {
                SystemManager.Instance.ResourceManager.EmitParticle(BossDeadEffectPath, _ownerEntity.CenterTransform.position);
                _ownerEntity.ExplosionImpulseSource.GenerateImpulse(1);
                _explosion = true;
            }
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, false);
        }
    }
}