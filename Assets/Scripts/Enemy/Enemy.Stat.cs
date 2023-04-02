

namespace QT.Enemy
{
    public partial class Enemy
    {
        public Status HP { get; private set; }
        public Stat MoveSpd { get; private set; }
        
        private  void SetUpStats()
        {
            HP = new Status(Data.MaxHP);
            MoveSpd = new Stat(Data.MoveSpd);
        }
    }
}