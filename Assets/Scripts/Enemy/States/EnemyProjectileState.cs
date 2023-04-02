using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Projectile)]
    public class EnemyProjectileState : FSMState<Enemy>
    {
        private float _deadTime = 0f;

        private float _deadAfterStunTime;
        
        public EnemyProjectileState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _deadTime = Time.time;
            _deadAfterStunTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
        }

        public override void UpdateState()
        {
            if (_deadTime + _deadAfterStunTime < Time.time)
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
