using System.Runtime.CompilerServices;

namespace QT.Player
{
    public partial class Player
    {
        public Status ThrowCooldown { get; private set; }
        public Status ThrowAfterDelay { get; private set; }
        public Stat ThrowSpd { get; private set; }
        public Stat ThrowBounceCount { get; private set; }
        public Status SwingCooldown { get; private set; }
        public Status SwingAfterDelay { get; private set; }
        public Stat[] ChargeTimes { get; private set; }
        public int ChargeAtkPierce { get; private set; }
        public Stat[] ChargeShootSpd { get; private set; }
        public Stat[] ChargeBounceCount { get; private set; }
        public Stat[] ChargeRigidDmg { get; private set; }
        public Stat[] ChargeProjectileDmg { get; private set; }
        public Stat AtkDmgPer { get; private set; }
        public Stat SwingRadius { get; private set; }
        public Stat SwingCentralAngle { get; private set; }
        

        #region CharacterGameData
        public Status HP { get; private set; }
        
        public Stat PCHitboxRad { get; private set; }
        public Stat MovementSpd { get; private set; }
        public Stat ChargeMovementSpd { get; private set; }
        public Status MercyInvincibleTime { get; private set; }
        public Status DodgeCooldown { get; private set; }
        public Status DodgeInvincibleTime { get; private set; }
        public Stat ItemSlotMax { get; private set; }
        public Stat BallStackMax { get; private set; }
        public Stat GoldGain { get; private set; }
        public int DefaultBallDataId { get; private set; }
        
        #endregion
        
        private void SetUpStats()
        {
            SwingRadius = new Stat(AtkData.SwingRad);
            SwingCentralAngle = new Stat(AtkData.SwingCentralAngle);

            ThrowCooldown = new Status(AtkData.ThrowCooldown);
            ThrowAfterDelay = new Status(AtkData.ThrowAfterDelay);
            ThrowSpd = new Stat(AtkData.ThrowSpd);
            ThrowBounceCount = new Stat(AtkData.ThrowBounceCount);
            SwingCooldown = new Status(AtkData.SwingCooldown);
            SwingAfterDelay = new Status(AtkData.SwingAfterDelay);
            AtkDmgPer = new Stat(AtkData.AtkDmgPer);
            ChargeAtkPierce = AtkData.ChargeAtkPierce;
            ChargeTimes = new Stat[]
            {
                new Stat(AtkData.ChargeTimes1),
                new Stat(AtkData.ChargeTimes2), 
                new Stat(AtkData.ChargeTimes3)
            };
            ChargeShootSpd = new Stat[]
            {
                new Stat(AtkData.ChargeShootSpd1),
                new Stat(AtkData.ChargeShootSpd2),
                new Stat(AtkData.ChargeShootSpd3),
                new Stat(AtkData.ChargeShootSpd4)
            };
            ChargeBounceCount = new Stat[]
            {
                new Stat(AtkData.ChargeBounceCount1),
                new Stat(AtkData.ChargeBounceCount2),
                new Stat(AtkData.ChargeBounceCount3),
                new Stat(AtkData.ChargeBounceCount4)
            };
            ChargeRigidDmg = new Stat[]
            {
                new Stat(AtkData.ChargeRigidDmg1),
                new Stat(AtkData.ChargeRigidDmg2),
                new Stat(AtkData.ChargeRigidDmg3),
                new Stat(AtkData.ChargeRigidDmg4)
            };
            ChargeProjectileDmg = new Stat[]
            {
                new Stat(AtkData.ChargeProjectileDmg1),
                new Stat(AtkData.ChargeProjectileDmg2),
                new Stat(AtkData.ChargeProjectileDmg3),
                new Stat(AtkData.ChargeProjectileDmg4)
            };

            HP = new Status(Data.HPMax);
            PCHitboxRad = new Stat(Data.PCHitboxRad);
            MovementSpd = new Stat(Data.MovementSpd);
            ChargeMovementSpd = new Stat(Data.ChargeMovementSpd);
            MercyInvincibleTime = new Status(Data.MercyInvincibleTime);
            DodgeCooldown = new Status(Data.DodgeInvincibleTime);
            DodgeInvincibleTime = new Status(Data.DodgeInvincibleTime);
            ItemSlotMax = new Stat(Data.ItemSlotMax);
            BallStackMax = new Stat(Data.BallStackMax);
            GoldGain = new Stat(Data.GoldGain);
            DefaultBallDataId = Data.DefaultBallDataId;
        }
        
        //public float[] AtkShootSpeed { get; private set; }
        //public float AtkCoolTime { get; private set; }
        //public float[] ChargingMaxTimes { get; private set; }
        //public int[] SwingRigidDmg { get; private set; }
        //public int BallStackMax { get; private set; }
        //public int[] ChargeBounceValues { get; private set; }
        //public float MercyInvincibleTime { get; private set; }
        //public float DodgeInvincibleTime { get; private set; }
    }

}