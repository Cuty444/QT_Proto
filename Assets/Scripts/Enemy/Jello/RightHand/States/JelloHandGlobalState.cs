using System;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)JelloHand.States.Global, false)]
    public class JelloHandGlobalState : FSMState<JelloHand>
    {
        public JelloHandGlobalState(IFSMEntity owner) : base(owner)
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
            if (_ownerEntity.CurrentStateIndex >= (int) JelloHand.States.Dead)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);
            _ownerEntity.HpImage.fillAmount = Util.Math.Remap(_ownerEntity.HP, _ownerEntity.HP.BaseValue, 0f);
            _ownerEntity.HpCanvas.gameObject.SetActive(_ownerEntity.HP > 0);
            
            _ownerEntity.MaterialChanger.SetHitMaterial();
            
            if (_ownerEntity.CurrentStateIndex >= (int) JelloHand.States.Projectile)
            {
                return;
            }

            if (_ownerEntity.HP <= 0)
            {
                if (attackType != AttackType.PowerSwing)
                {
                    var state = _ownerEntity.ChangeState(JelloHand.States.Rigid);
                    ((JelloHandRigidState) state)?.InitializeState(dir);
                }
            }
        }
        
        private void OnProjectileHit((Vector2 dir, float power) vector, LayerMask bounceMask, ProjectileProperties properties, Transform target)
        {
            if (_ownerEntity.CurrentStateIndex >= (int) JelloHand.States.Dead)
            {
                return;
            }
            
            var state = _ownerEntity.ChangeState(JelloHand.States.Projectile);
            ((JelloHandProjectileState) state)?.InitializeState(vector.dir, vector.power, bounceMask, properties, target);
        }
    }
}
