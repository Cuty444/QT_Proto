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
            _ownerEntity.HP.AddStatus(-damage);
            
            if (_ownerEntity.HP <= 0 && !_isRiged)
            {
                _ownerEntity.ChangeState(Enemy.States.Rigid);
                _isRiged = true;
            }
        }
    }
}
