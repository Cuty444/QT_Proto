using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QT.InGame
{
    public enum PlayerStats
    {
        // Status
        
        HP,
        
        DodgeCooldown,
        SwingCooldown,
        
        DodgeInvincibleTime,
        MercyInvincibleTime,
        
        // Stat
        
        PCHitboxRad,
        MovementSpd,
        ChargeMovementSpd,
        DodgeDurationTime,
        DodgeAddForce,
        GoldGain,
            
        SwingRad,
        SwingCentralAngle,
            
        ChargeTime,
            
        ChargeAtkPierce,
        
        ChargeShootSpd,
        
        ChargeBounceCount,
            
        ReflectCorrection,
        
        ChargeRigidDmg1,
        ChargeRigidDmg2,
        
        ChargeProjectileDmg,
        EnemyProjectileDmg,
        
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
        
        protected void AddStat(PlayerStats stat, Stat value)
        {
            _stats[stat] = value;
        }
    }
}
