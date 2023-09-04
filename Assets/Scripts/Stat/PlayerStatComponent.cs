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

            AddStat(PlayerStats.HP, new Status(data.MaxHP));
            AddStat(PlayerStats.BallStack, new Status(data.BallStackMax));

            AddStat(PlayerStats.DodgeCooldown, new Status(data.DodgeCooldown));
            AddStat(PlayerStats.ThrowCooldown, new Status(atkData.ThrowCooldown));
            AddStat(PlayerStats.SwingCooldown, new Status(atkData.SwingCooldown));

            AddStat(PlayerStats.MercyInvincibleTime, new Status(data.MercyInvincibleTime));
            AddStat(PlayerStats.DodgeInvincibleTime, new Status(data.DodgeInvincibleTime));

            // Stat

            AddStat(PlayerStats.PCHitboxRad, new(data.PCHitboxRad));
            AddStat(PlayerStats.MovementSpd, new(data.MovementSpd));
            AddStat(PlayerStats.ChargeMovementSpd, new(data.ChargeMovementSpd));
            AddStat(PlayerStats.DodgeDurationTime, new(data.DodgeDurationTime));
            AddStat(PlayerStats.DodgeAddForce, new(data.DodgeAddForce));
            AddStat(PlayerStats.ItemSlotMax, new(data.ItemSlotMax));
            AddStat(PlayerStats.GoldGain, new(data.GoldGain));

            AddStat(PlayerStats.ThrowAfterDelay, new(atkData.ThrowAfterDelay));
            AddStat(PlayerStats.ThrowSpd, new(atkData.ThrowSpd));
            AddStat(PlayerStats.ThrowBounceCount, new(atkData.ThrowBounceCount));

            AddStat(PlayerStats.SwingRad, new(atkData.SwingRad));
            AddStat(PlayerStats.SwingCentralAngle, new(atkData.SwingCentralAngle));


            AddStat(PlayerStats.ChargeTime, new(atkData.ChargeTime));

            AddStat(PlayerStats.ChargeAtkPierce, new(atkData.ChargeAtkPierce));

            AddStat(PlayerStats.ChargeShootSpd1, new(atkData.ChargeShootSpd1));
            AddStat(PlayerStats.ChargeShootSpd2, new(atkData.ChargeShootSpd2));

            AddStat(PlayerStats.ChargeBounceCount, new(atkData.ChargeBounceCount));

            AddStat(PlayerStats.ChargeRigidDmg1, new(atkData.ChargeRigidDmg1));
            AddStat(PlayerStats.ChargeRigidDmg2, new(atkData.ChargeRigidDmg2));

            AddStat(PlayerStats.ChargeProjectileDmg1, new(atkData.ChargeProjectileDmg1));
            AddStat(PlayerStats.ChargeProjectileDmg2, new(atkData.ChargeProjectileDmg2));

            AddStat(PlayerStats.EnemyProjectileDmg1, new(atkData.EnemyProjectileDmg1));
            AddStat(PlayerStats.EnemyProjectileDmg2, new(atkData.EnemyProjectileDmg2));

            AddStat(PlayerStats.ReflectCorrection, new(atkData.ReflectCorrection));
            AddStat(PlayerStats.AtkDmgPer, new(atkData.AtkDmgPer));
        }
    }
}
