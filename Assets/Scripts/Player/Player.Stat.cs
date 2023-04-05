
namespace QT.Player
{
    public partial class Player
    {
        public float AtkRadius { get; private set; }
        public float AtkCentralAngle { get; private set; }
        
        public float[] AtkShootSpeed { get; private set; }
        public float AtkCoolTime { get; private set; }
        public float[] ChargingMaxTimes { get; private set; }
        public int[] SwingRigidDmg { get; private set; }
        public int BallStackMax { get; private set; }
        public int[] ChargeBounceValues { get; private set; }

        public float MercyInvincibleTime { get; private set; }
        public float DodgeInvincibleTime { get; private set; }
        
        public int HPMax { get; private set; }
        public int HP { get; private set; }
    }

}