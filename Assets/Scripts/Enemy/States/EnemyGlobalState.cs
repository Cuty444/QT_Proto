using System;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)Enemy.States.Global, false)]
    public class EnemyGlobalState : FSMState<Enemy>
    {
        public EnemyGlobalState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
        }
        
        private void OnDamage(Vector2 dir, float power)
        {
            if (_ownerEntity.CurrentStateIndex >= (int)Enemy.States.Rigid)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero; 
            _ownerEntity.Rigidbody.AddForce(-dir, ForceMode2D.Impulse);
            _ownerEntity.ChangeState(Enemy.States.Rigid);
        }
    }
}
