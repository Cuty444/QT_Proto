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
            AfterIdleSec,
            Sight,
        }

        public int Index { get; set; }
        public int MaxHp { get; set; }
    
        public float AgroRange { get; set; }
        public MoveTypes MoveType { get; set; }
        public float MovementSpd { get; set; }
        public float BodyContactDmg { get; set; }
        public float SpacingRad { get; set; }
        public float MoveTargetUpdatePeroid { get; set; }

        public float AtkCheckDelay { get; set; }
        public AtkStartTypes AtkStartType { get; set; }
        public float AtkStartParam { get; set; }
    
        public int AtkDataId { get; set; }
        public int DeadAtkDataId { get; set; }
        
        public int ProjectileDataId { get; set; }
        
        public int GoldDropMin { get; set; }
        public int GoldDropMax { get; set; }
        
        public string PrefabPath { get; set; }
    }


    [GameDataBase(typeof(EnemyGameData), "EnemyGameData")]
    public class EnemyGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, EnemyGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (EnemyGameData)data);
        }

        public EnemyGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}