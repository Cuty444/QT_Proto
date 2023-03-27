using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class EnemyGameData : IGameData
    {
        public enum MoveTypes
        {
            None,
            Spacing,
            SpacingLeft
        }
    
        public enum AtkStartTypes
        {
            None,
            Sight,
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxHP { get; set; }
    
        public MoveTypes MoveType { get; set; }
        public float MoveSpd { get; set; }
        public float SpacingRad { get; set; }
        public float MoveTargetUpdatePeroid { get; set; }

        public float AtkCheakDelay { get; set; }
        public AtkStartTypes AtkStartType { get; set; }
        public float AtkStartParam { get; set; }
    
        public int DropGoldMin { get; set; }
        public int DropGoldMax { get; set; }
    
        public int AtkDataId { get; set; }
        public int DeadAtkDataId { get; set; }
    }


    [GameDataBaseAttribute(typeof(EnemyGameData), "EnemyGameData")]
    public class EnemyGameDataBase : IGameDataBase
    {
        public Dictionary<int, IGameData> datas { get; set; }

        public EnemyGameData GetData(int id)
        {
            if (datas.TryGetValue(id, out var value)) return (EnemyGameData) value;

            return null;
        }
    }  
}