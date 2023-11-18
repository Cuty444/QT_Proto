
namespace QT.InGame
{
    public partial class Jello
    {
        public Status HP { get; private set; }
        public Stat MoveSpd { get; private set; }
        
        private  void SetUpStats(float hpPer)
        {
            HP = new Status(Data.MaxHp * hpPer);
            MoveSpd = new Stat(Data.MovementSpd);
        }
    }
}
