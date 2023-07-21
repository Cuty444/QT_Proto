using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public enum PlayerStats
    {
        // Status
        
        HP,
        BallStack,
        
        DodgeCooldown,
        ThrowCooldown,
        SwingCooldown,
        
        DodgeInvincibleTime,
        MercyInvincibleTime,
        
        // Stat
        
        PCHitboxRad,
        MovementSpd,
        ChargeMovementSpd,
        DodgeDurationTime,
        DodgeAddForce,
        ItemSlotMax,
        GoldGain,
            
        ThrowAfterDelay,
        ThrowSpd,
        ThrowBounceCount,
        SwingAfterDelay,
        SwingRad,
        SwingCentralAngle,
            
        ChargeTime,
            
        ChargeAtkPierce,
            
        ChargeShootSpd1,
        ChargeShootSpd2,
            
        ChargeBounceCount1,
        ChargeBounceCount2,
            
        ReflectCorrection,
            
        ChargeRigidDmg1,
        ChargeRigidDmg2,
            
        ChargeProjectileDmg1,
        ChargeProjectileDmg2,
        
        EnemyProjectileDmg1,
        EnemyProjectileDmg2,
        
        TeleportAllowableDistance,
            
        AtkDmgPer,
    }
    
    public abstract class StatComponent
    {
        protected readonly Dictionary<PlayerStats, Stat> _stats = new();

        public Stat GetStat(PlayerStats stat)
        {
            return _stats[stat];
        }

        public float GetDmg(PlayerStats stat)
        {
            return _stats[stat].Value * _stats[PlayerStats.AtkDmgPer].Value;
        }
        
        public Status GetStatus(PlayerStats stat)
        {
            return _stats[stat] as Status;
        }
    }
}
