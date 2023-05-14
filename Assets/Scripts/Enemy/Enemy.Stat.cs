

namespace QT.InGame
{
    public partial class Enemy
    {
        public Status HP { get; private set; }
        public Stat MoveSpd { get; private set; }
        
        private  void SetUpStats()
        {
            HP = new Status(Data.MaxHp);
            MoveSpd = new Stat(Data.MovementSpd);
        }
    }
}