using System;
using QT.Core;
using UnityEngine;

namespace QT.Enemy
{
    [FSMState((int)Enemy.States.Global)]
    public class EnemyGlobalState : FSMState<Enemy>
    {
        public EnemyGlobalState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
        }
        
        private void OnDamage(float damage, Vector2 hitPoint)
        {
            if (_ownerEntity.CurrentState >= (int)Enemy.States.Rigid)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-damage);
            _ownerEntity.ChangeState(Enemy.States.Rigid);
        }
    }
}
