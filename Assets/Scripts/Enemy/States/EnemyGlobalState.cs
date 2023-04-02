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
        
        private  bool _isRiged = false;

        private void OnDamage(float damage, Vector2 hitPoint)
        {
            if (_ownerEntity.CurrentState == (int)Enemy.States.Dead)
            {
                return;
            }

            if (_ownerEntity.CurrentState != (int) Enemy.States.Rigid)
            {
                _ownerEntity.HP.AddStatus(-damage);
                if (_ownerEntity.HP > 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Rigid);
                }
                else
                {
                    _ownerEntity.ChangeState(Enemy.States.Projectile);
                }
            }
        }
    }
}
