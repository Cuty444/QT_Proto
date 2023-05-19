using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QT.InGame
{
    public enum PlayerStats
    {
        HP,
        
        PCHitboxRad,
        MovementSpd,
        ChargeMovementSpd,
        MercyInvincibleTime,
        DodgeCooldown,
        DodgeInvincibleTime,
        DodgeDurationTime,
        DodgeAddForce,
        ItemSlotMax,
        BallStackMax,
        GoldGain,
            
        ThrowCooldown,
        ThrowAfterDelay,
        ThrowSpd,
        ThrowBounceCount,
        SwingCooldown,
        SwingAfterDelay,
        SwingRad,
        SwingCentralAngle,
            
        ChargeTime1,
        ChargeTime2,
        ChargeTime3,
            
        ChargeAtkPierce,
            
        ChargeShootSpd1,
        ChargeShootSpd2,
        ChargeShootSpd3,
        ChargeShootSpd4,
            
        ChargeBounceCount1,
        ChargeBounceCount2,
        ChargeBounceCount3,
        ChargeBounceCount4,
            
        ReflectCorrection,
            
        ChargeRigidDmg1,
        ChargeRigidDmg2,
        ChargeRigidDmg3,
        ChargeRigidDmg4,
            
        ChargeProjectileDmg1,
        ChargeProjectileDmg2,
        ChargeProjectileDmg3,
        ChargeProjectileDmg4,
            
        AtkDmgPer,
    }

    public partial class Player
    {
        public const int MaxChargeLevel = 4;
        
        private readonly Dictionary<PlayerStats, Stat> _stats = new();

        private readonly Stat[] _chargeTimes = new Stat[MaxChargeLevel - 1];
        private readonly Stat[] _chargeShootSpds = new Stat[MaxChargeLevel];
        private readonly Stat[] _chargeBounceCounts = new Stat[MaxChargeLevel];
        private readonly Stat[] _chargeRigidDmgs = new Stat[MaxChargeLevel];
        private readonly Stat[] _chargeProjectileDmgs = new Stat[MaxChargeLevel];
        
        public Stat[] ChargeTimes => _chargeTimes;
        public Stat[] ChargeShootSpds => _chargeShootSpds;
        public Stat[] ChargeBounceCounts => _chargeBounceCounts;
        public Stat[] ChargeRigidDmgs => _chargeRigidDmgs;
        public Stat[] ChargeProjectileDmgs => _chargeProjectileDmgs;

        private void InitStats()
        {
            _stats.Clear();
            _stats.Add(PlayerStats.HP, new Status(Data.MaxHP));
            
            _stats.Add(PlayerStats.PCHitboxRad, new(Data.PCHitboxRad));
            _stats.Add(PlayerStats.MovementSpd, new(Data.MovementSpd));
            _stats.Add(PlayerStats.ChargeMovementSpd, new(Data.ChargeMovementSpd));
            _stats.Add(PlayerStats.MercyInvincibleTime, new(Data.MercyInvincibleTime));
            _stats.Add(PlayerStats.DodgeCooldown, new(Data.DodgeCooldown));
            _stats.Add(PlayerStats.DodgeInvincibleTime, new(Data.DodgeInvincibleTime));
            _stats.Add(PlayerStats.DodgeDurationTime, new(Data.DodgeDurationTime));
            _stats.Add(PlayerStats.DodgeAddForce, new(Data.DodgeAddForce));
            _stats.Add(PlayerStats.ItemSlotMax, new(Data.ItemSlotMax));
            _stats.Add(PlayerStats.BallStackMax, new(Data.BallStackMax));
            _stats.Add(PlayerStats.GoldGain, new(Data.GoldGain));

            _stats.Add(PlayerStats.ThrowCooldown, new(AtkData.ThrowCooldown));
            _stats.Add(PlayerStats.ThrowAfterDelay, new(AtkData.ThrowAfterDelay));
            _stats.Add(PlayerStats.ThrowSpd, new(AtkData.ThrowSpd));
            _stats.Add(PlayerStats.ThrowBounceCount, new(AtkData.ThrowBounceCount));
            _stats.Add(PlayerStats.SwingCooldown, new(AtkData.SwingCooldown));
            _stats.Add(PlayerStats.SwingAfterDelay, new(AtkData.SwingAfterDelay));
            _stats.Add(PlayerStats.SwingRad, new(AtkData.SwingRad));
            _stats.Add(PlayerStats.SwingCentralAngle, new(AtkData.SwingCentralAngle));

            _stats.Add(PlayerStats.ChargeAtkPierce, new(AtkData.ChargeAtkPierce));
            _stats.Add(PlayerStats.ReflectCorrection, new(AtkData.ReflectCorrection));
            _stats.Add(PlayerStats.AtkDmgPer, new(AtkData.AtkDmgPer));
            
            // Charge
            
            _chargeTimes[0] = new(AtkData.ChargeTime1);
            _chargeTimes[1] = new(AtkData.ChargeTime2);
            _chargeTimes[2] = new(AtkData.ChargeTime3);
            
            _chargeShootSpds[0] = new(AtkData.ChargeShootSpd1);
            _chargeShootSpds[1] = new(AtkData.ChargeShootSpd2);
            _chargeShootSpds[2] = new(AtkData.ChargeShootSpd3);
            _chargeShootSpds[3] = new(AtkData.ChargeShootSpd4);
            
            _chargeBounceCounts[0] = new(AtkData.ChargeBounceCount1);
            _chargeBounceCounts[1] = new(AtkData.ChargeBounceCount2);
            _chargeBounceCounts[2] = new(AtkData.ChargeBounceCount3);
            _chargeBounceCounts[3] = new(AtkData.ChargeBounceCount4);
            
            _chargeRigidDmgs[0] = new(AtkData.ChargeRigidDmg1);
            _chargeRigidDmgs[1] = new(AtkData.ChargeRigidDmg2);
            _chargeRigidDmgs[2] = new(AtkData.ChargeRigidDmg3);
            _chargeRigidDmgs[3] = new(AtkData.ChargeRigidDmg4);
            
            _chargeProjectileDmgs[0] = new(AtkData.ChargeProjectileDmg1);
            _chargeProjectileDmgs[1] = new(AtkData.ChargeProjectileDmg2);
            _chargeProjectileDmgs[2] = new(AtkData.ChargeProjectileDmg3);
            _chargeProjectileDmgs[3] = new(AtkData.ChargeProjectileDmg4);
            
            _stats.Add(PlayerStats.ChargeTime1, _chargeTimes[0]);
            _stats.Add(PlayerStats.ChargeTime2, _chargeTimes[1]);
            _stats.Add(PlayerStats.ChargeTime3, _chargeTimes[2]);
            
            _stats.Add(PlayerStats.ChargeShootSpd1, _chargeShootSpds[0]);
            _stats.Add(PlayerStats.ChargeShootSpd2, _chargeShootSpds[1]);
            _stats.Add(PlayerStats.ChargeShootSpd3, _chargeShootSpds[2]);
            _stats.Add(PlayerStats.ChargeShootSpd4, _chargeShootSpds[3]);
            
            _stats.Add(PlayerStats.ChargeBounceCount1, _chargeBounceCounts[0]);
            _stats.Add(PlayerStats.ChargeBounceCount2, _chargeBounceCounts[1]);
            _stats.Add(PlayerStats.ChargeBounceCount3, _chargeBounceCounts[2]);
            _stats.Add(PlayerStats.ChargeBounceCount4, _chargeBounceCounts[3]);
            
            _stats.Add(PlayerStats.ChargeRigidDmg1, _chargeRigidDmgs[0]);
            _stats.Add(PlayerStats.ChargeRigidDmg2, _chargeRigidDmgs[1]);
            _stats.Add(PlayerStats.ChargeRigidDmg3, _chargeRigidDmgs[2]);
            _stats.Add(PlayerStats.ChargeRigidDmg4, _chargeRigidDmgs[3]);
            
            _stats.Add(PlayerStats.ChargeProjectileDmg1, _chargeProjectileDmgs[0]);
            _stats.Add(PlayerStats.ChargeProjectileDmg2, _chargeProjectileDmgs[1]);
            _stats.Add(PlayerStats.ChargeProjectileDmg3, _chargeProjectileDmgs[2]);
            _stats.Add(PlayerStats.ChargeProjectileDmg4, _chargeProjectileDmgs[3]);
        }

        public Stat GetStat(PlayerStats stat)
        {
            return _stats[stat];
        }
    }
}