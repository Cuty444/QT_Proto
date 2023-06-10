

using QT.Core;
using QT.Core.Map;

namespace QT.InGame
{
    public partial class Enemy
    {
        public Status HP { get; private set; }
        public Stat MoveSpd { get; private set; }
        
        private  void SetUpStats()
        {
            HP = new Status(Data.MaxHp * SystemManager.Instance.GetSystem<DungeonMapSystem>().GetEnemyHpIncreasePer());
            MoveSpd = new Stat(Data.MovementSpd);
        }
    }
}