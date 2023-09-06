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
            _ownerEntity.OnProjectileHitEvent.AddListener(OnProjectileHit);
        }

        private void OnDamage(Vector2 dir, float power, AttackType attackType)
        {
            if (_ownerEntity.CurrentStateIndex >= (int) Enemy.States.Dead)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);
            _ownerEntity.HpImage.fillAmount = Util.Math.Remap(_ownerEntity.HP, _ownerEntity.HP.BaseValue, 0f);
            _ownerEntity.HpCanvas.gameObject.SetActive(true);
            
            if (_ownerEntity.CurrentStateIndex >= (int) Enemy.States.Projectile)
            {
                return;
            }

            if (attackType != AttackType.PowerSwing)
            {
                var state = _ownerEntity.ChangeState(Enemy.States.Rigid);
                ((EnemyRigidState) state)?.InitializeState(dir);
            }
        }
        
        private void OnProjectileHit((Vector2 dir, float power) vector, LayerMask bounceMask, ProjectileOwner owner, bool isPierce)
        {
            if (_ownerEntity.CurrentStateIndex >= (int) Enemy.States.Dead)
            {
                return;
            }
            
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state)?.InitializeState(vector.dir, vector.power, bounceMask, isPierce, owner != ProjectileOwner.PlayerAbsorb);
        }
    }
}
