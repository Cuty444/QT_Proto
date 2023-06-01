using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        
        TeleportAllowableDistance,
            
        AtkDmgPer,
    }

    public partial class Player
    {
        private readonly Dictionary<PlayerStats, Stat> _stats = new();

        private void InitStats()
        {
            _stats.Clear();
            
            // Status
            
            _stats.Add(PlayerStats.HP, new Status(Data.MaxHP));
            _stats.Add(PlayerStats.BallStack, new Status(Data.BallStackMax));
            
            _stats.Add(PlayerStats.DodgeCooldown, new Status(Data.DodgeCooldown));
            _stats.Add(PlayerStats.ThrowCooldown, new Status(AtkData.ThrowCooldown));
            _stats.Add(PlayerStats.SwingCooldown, new Status(AtkData.SwingCooldown));
            
            _stats.Add(PlayerStats.MercyInvincibleTime, new Status(Data.MercyInvincibleTime));
            _stats.Add(PlayerStats.DodgeInvincibleTime, new Status(Data.DodgeInvincibleTime));
            
            // Stat
            
            _stats.Add(PlayerStats.PCHitboxRad, new(Data.PCHitboxRad));
            _stats.Add(PlayerStats.MovementSpd, new(Data.MovementSpd));
            _stats.Add(PlayerStats.ChargeMovementSpd, new(Data.ChargeMovementSpd));
            _stats.Add(PlayerStats.DodgeDurationTime, new(Data.DodgeDurationTime));
            _stats.Add(PlayerStats.DodgeAddForce, new(Data.DodgeAddForce));
            _stats.Add(PlayerStats.ItemSlotMax, new(Data.ItemSlotMax));
            _stats.Add(PlayerStats.GoldGain, new(Data.GoldGain));

            _stats.Add(PlayerStats.ThrowAfterDelay, new(AtkData.ThrowAfterDelay));
            _stats.Add(PlayerStats.ThrowSpd, new(AtkData.ThrowSpd));
            _stats.Add(PlayerStats.ThrowBounceCount, new(AtkData.ThrowBounceCount));
            
            _stats.Add(PlayerStats.SwingAfterDelay, new(AtkData.SwingAfterDelay));
            _stats.Add(PlayerStats.SwingRad, new(AtkData.SwingRad));
            _stats.Add(PlayerStats.SwingCentralAngle, new(AtkData.SwingCentralAngle));
            
            
            _stats.Add(PlayerStats.ChargeTime, new(AtkData.ChargeTime));
            
            _stats.Add(PlayerStats.ChargeAtkPierce, new(AtkData.ChargeAtkPierce));
            
            _stats.Add(PlayerStats.ChargeShootSpd1, new(AtkData.ChargeShootSpd1));
            _stats.Add(PlayerStats.ChargeShootSpd2, new(AtkData.ChargeShootSpd2));
            
            _stats.Add(PlayerStats.ChargeBounceCount1, new(AtkData.ChargeBounceCount1));
            _stats.Add(PlayerStats.ChargeBounceCount2, new(AtkData.ChargeBounceCount2));
            
            _stats.Add(PlayerStats.ChargeRigidDmg1, new(AtkData.ChargeRigidDmg1));
            _stats.Add(PlayerStats.ChargeRigidDmg2, new(AtkData.ChargeRigidDmg2));
            
            _stats.Add(PlayerStats.ChargeProjectileDmg1, new(AtkData.ChargeProjectileDmg1));
            _stats.Add(PlayerStats.ChargeProjectileDmg2, new(AtkData.ChargeProjectileDmg2));

            _stats.Add(PlayerStats.TeleportAllowableDistance, new (AtkData.TeleportAllowableDistance));
            
            _stats.Add(PlayerStats.ReflectCorrection, new(AtkData.ReflectCorrection));
            _stats.Add(PlayerStats.AtkDmgPer, new(AtkData.AtkDmgPer));
        }

        public Stat GetStat(PlayerStats stat)
        {
            return _stats[stat];
        }
        
        public Status GetStatus(PlayerStats stat)
        {
            return _stats[stat] as Status;
        }
    }
}