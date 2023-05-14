using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Enemy.States.Rigid)]
    public class EnemyRigidState : FSMState<Enemy>
    {
        private static readonly int RigidAnimHash = Animator.StringToHash("IsRigid");
        
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
                _ownerEntity.OnHitEvent.AddListener(OnDamage);
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
                
                _ownerEntity.MaterialChanger.ChangeMaterial();
                
                SystemManager.Instance.ProjectileManager.Register(_ownerEntity);
            }
            else
            {
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
            }
        }

        public override void ClearState()
        {
            _ownerEntity.OnHitEvent.RemoveListener(OnDamage);
            _ownerEntity.MaterialChanger.ClearMaterial();
        }

        public override void UpdateState()
        {
            if (_rigidStartTime + _rigidTime < Time.time)
            {
                if (_ownerEntity.HP <= 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Dead);
                    SystemManager.Instance.ProjectileManager.UnRegister(_ownerEntity);
                }
                else
                {
                    _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                    _ownerEntity.RevertToPreviousState();
                }
            }
        }

        private void OnDamage(Vector2 dir, float power, LayerMask bounceMask)
        {
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state)?.InitializeState(dir, power, bounceMask);
        }
    }
}