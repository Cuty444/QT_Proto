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

            AddStat(PlayerStats.DodgeCooldown, new Status(data.DodgeCooldown));
            AddStat(PlayerStats.SwingCooldown, new Status(atkData.SwingCooldown));

            AddStat(PlayerStats.MercyInvincibleTime, new Status(data.MercyInvincibleTime));
            AddStat(PlayerStats.DodgeInvincibleTime, new Status(data.DodgeInvincibleTime));

            // Stat

            AddStat(PlayerStats.PCHitboxRad, new(data.PCHitboxRad));
            AddStat(PlayerStats.MovementSpd, new(data.MovementSpd));
            AddStat(PlayerStats.ChargeMovementSpd, new(data.ChargeMovementSpd));
            AddStat(PlayerStats.DodgeDurationTime, new(data.DodgeDurationTime));
            AddStat(PlayerStats.DodgeAddForce, new(data.DodgeAddForce));
            AddStat(PlayerStats.GoldGain, new(data.GoldGain));

            AddStat(PlayerStats.SwingRad, new(atkData.SwingRad));
            AddStat(PlayerStats.SwingCentralAngle, new(atkData.SwingCentralAngle));


            AddStat(PlayerStats.ChargeTime, new(atkData.ChargeTime));

            AddStat(PlayerStats.ProjectileGuide, new(atkData.ProjectileGuide));
            AddStat(PlayerStats.ProjectileExplosion, new(atkData.ProjectileExplosion));
            AddStat(PlayerStats.ProjectilePierce, new(atkData.ProjectilePierce));

            AddStat(PlayerStats.ChargeShootSpd, new(atkData.ChargeShootSpd));

            AddStat(PlayerStats.ChargeBounceCount, new(atkData.ChargeBounceCount));

            AddStat(PlayerStats.ChargeRigidDmg1, new(atkData.ChargeRigidDmg1));
            AddStat(PlayerStats.ChargeRigidDmg2, new(atkData.ChargeRigidDmg2));

            AddStat(PlayerStats.ChargeProjectileDmg, new(atkData.ChargeProjectileDmg));

            AddStat(PlayerStats.EnemyProjectileDmg, new(atkData.EnemyProjectileDmg));

            AddStat(PlayerStats.AtkDmgPer, new(atkData.AtkDmgPer));
        }
    }
}
