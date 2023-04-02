using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int) Enemy.States.Rigid)]
    public class EnemyRigidState : FSMState<Enemy>
    {
        private float _rigidTime = 0f;

        private float _deadAfterStunTime;
        
        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _rigidTime = Time.time;
            _deadAfterStunTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
        }

        public override void UpdateState()
        {
            if (_rigidTime + _deadAfterStunTime < Time.time)
            {
                _ownerEntity.ChangeState(Enemy.States.Dead);
            }
        }

        public override void ClearState()
        {
            _ownerEntity.OnDamageEvent.RemoveListener(OnDamage);
        }
        
        private void OnDamage(float damage, Vector2 hitPoint)
        {
            _ownerEntity.ChangeState(Enemy.States.Projectile);
        }
    }
}