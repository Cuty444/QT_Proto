using System;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)JelloRightHand.States.Global, false)]
    public class JelloRightHandGlobalState : FSMState<JelloRightHand>
    {
        public JelloRightHandGlobalState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
            _ownerEntity.OnProjectileHitEvent.AddListener(OnProjectileHit);
        }

        public override void ClearState()
        {
            _ownerEntity.OnDamageEvent.RemoveListener(OnDamage);
            _ownerEntity.OnProjectileHitEvent.RemoveListener(OnProjectileHit);
        }

        private void OnDamage(Vector2 dir, float power, AttackType attackType)
        {
            if (_ownerEntity.CurrentStateIndex >= (int) JelloRightHand.States.Dead)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);
            _ownerEntity.HpIndicator.SetHP(_ownerEntity.HP);
            
            _ownerEntity.MaterialChanger.SetHitMaterial();
            
            if (_ownerEntity.CurrentStateIndex >= (int) JelloRightHand.States.Projectile)
            {
                return;
            }

            if (_ownerEntity.HP <= 0)
            {
                if (attackType != AttackType.PowerSwing)
                {
                    var state = _ownerEntity.ChangeState(JelloRightHand.States.Rigid);
                    ((JelloRightHandRigidState) state)?.InitializeState(dir);
                }
            }
        }
        
        private void OnProjectileHit((Vector2 dir, float power) vector, LayerMask bounceMask, ProjectileProperties properties, Transform target)
        {
            if (_ownerEntity.CurrentStateIndex >= (int) JelloRightHand.States.Dead)
            {
                return;
            }
            
            var state = _ownerEntity.ChangeState(JelloRightHand.States.Projectile);
            ((JelloRightHandProjectileState) state)?.InitializeState(vector.dir, vector.power, bounceMask, properties, target);
        }
    }
}
