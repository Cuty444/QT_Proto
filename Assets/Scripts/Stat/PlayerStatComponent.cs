using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public class PlayerStatComponent : StatComponent
    {
        public PlayerStatComponent(CharacterGameData data, CharacterAtkGameData atkData)
        {
            _stats.Clear();

            // Status

            _stats.Add(PlayerStats.HP, new Status(data.MaxHP));
            _stats.Add(PlayerStats.BallStack, new Status(data.BallStackMax));

            _stats.Add(PlayerStats.DodgeCooldown, new Status(data.DodgeCooldown));
            _stats.Add(PlayerStats.ThrowCooldown, new Status(atkData.ThrowCooldown));
            _stats.Add(PlayerStats.SwingCooldown, new Status(atkData.SwingCooldown));

            _stats.Add(PlayerStats.MercyInvincibleTime, new Status(data.MercyInvincibleTime));
            _stats.Add(PlayerStats.DodgeInvincibleTime, new Status(data.DodgeInvincibleTime));

            // Stat

            _stats.Add(PlayerStats.PCHitboxRad, new(data.PCHitboxRad));
            _stats.Add(PlayerStats.MovementSpd, new(data.MovementSpd));
            _stats.Add(PlayerStats.ChargeMovementSpd, new(data.ChargeMovementSpd));
            _stats.Add(PlayerStats.DodgeDurationTime, new(data.DodgeDurationTime));
            _stats.Add(PlayerStats.DodgeAddForce, new(data.DodgeAddForce));
            _stats.Add(PlayerStats.ItemSlotMax, new(data.ItemSlotMax));
            _stats.Add(PlayerStats.GoldGain, new(data.GoldGain));

            _stats.Add(PlayerStats.ThrowAfterDelay, new(atkData.ThrowAfterDelay));
            _stats.Add(PlayerStats.ThrowSpd, new(atkData.ThrowSpd));
            _stats.Add(PlayerStats.ThrowBounceCount, new(atkData.ThrowBounceCount));

            _stats.Add(PlayerStats.SwingAfterDelay, new(atkData.SwingAfterDelay));
            _stats.Add(PlayerStats.SwingRad, new(atkData.SwingRad));
            _stats.Add(PlayerStats.SwingCentralAngle, new(atkData.SwingCentralAngle));


            _stats.Add(PlayerStats.ChargeTime, new(atkData.ChargeTime));

            _stats.Add(PlayerStats.ChargeAtkPierce, new(atkData.ChargeAtkPierce));

            _stats.Add(PlayerStats.ChargeShootSpd1, new(atkData.ChargeShootSpd1));
            _stats.Add(PlayerStats.ChargeShootSpd2, new(atkData.ChargeShootSpd2));

            _stats.Add(PlayerStats.ChargeBounceCount1, new(atkData.ChargeBounceCount1));
            _stats.Add(PlayerStats.ChargeBounceCount2, new(atkData.ChargeBounceCount2));

            _stats.Add(PlayerStats.ChargeRigidDmg1, new(atkData.ChargeRigidDmg1));
            _stats.Add(PlayerStats.ChargeRigidDmg2, new(atkData.ChargeRigidDmg2));

            _stats.Add(PlayerStats.ChargeProjectileDmg1, new(atkData.ChargeProjectileDmg1));
            _stats.Add(PlayerStats.ChargeProjectileDmg2, new(atkData.ChargeProjectileDmg2));

            _stats.Add(PlayerStats.EnemyProjectileDmg1, new(atkData.EnemyProjectileDmg1));
            _stats.Add(PlayerStats.EnemyProjectileDmg2, new(atkData.EnemyProjectileDmg2));

            _stats.Add(PlayerStats.ReflectCorrection, new(atkData.ReflectCorrection));
            _stats.Add(PlayerStats.AtkDmgPer, new(atkData.AtkDmgPer));
        }
    }
}
