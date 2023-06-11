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
        
        private void OnDamage(Vector2 dir, float power,AttackType attackType)
        {
            if (_ownerEntity.CurrentStateIndex >= (int)Enemy.States.Projectile)
            {
                if (attackType != AttackType.Teleport)
                {
                    return;
                }
            }

            _ownerEntity.HP.AddStatus(-power);
            _ownerEntity.HpImage.fillAmount = Util.Math.Remap(_ownerEntity.HP,_ownerEntity.HP.BaseValue,0f);
            _ownerEntity.HpCanvas.gameObject.SetActive(true);
            _ownerEntity.Rigidbody.velocity = Vector2.zero; 
            _ownerEntity.Rigidbody.AddForce(dir, ForceMode2D.Impulse);
            _ownerEntity.HitAttackType = attackType;
            _ownerEntity.ChangeState(Enemy.States.Rigid);
        }
    }
}
