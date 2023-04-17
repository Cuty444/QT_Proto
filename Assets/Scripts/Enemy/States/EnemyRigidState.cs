using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int) Enemy.States.Rigid)]
    public class EnemyRigidState : FSMState<Enemy>
    {
        private readonly int RigidAnimHash = Animator.StringToHash("IsRigid");
        
        private float _rigidStartTime;
        private float _rigidTime;

        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(RigidAnimHash, true);
            _rigidStartTime = Time.time;

            if (_ownerEntity.HP <= 0)
            {
                _ownerEntity.OnDamageEvent.AddListener(OnDamage);
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
            }
            else
            {
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
            }
        }

        public override void ClearState()
        {
            _ownerEntity.OnDamageEvent.RemoveListener(OnDamage);
        }

        public override void UpdateState()
        {
            if (_rigidStartTime + _rigidTime < Time.time)
            {
                if (_ownerEntity.HP <= 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Dead);
                }
                else
                {
                    _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                    _ownerEntity.RevertToPreviousState();
                }
            }
        }

        private void OnDamage(Vector2 dir, float power)
        {
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state).InitializeState(dir, power);
        }
    }
}