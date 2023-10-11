using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT
{
    public class EnemyAtkGameData : IGameData
    {
        public int Index { get; set; }
        
        public int ShootDataId { get; set; }

        public AimTypes AimType { get; set; }
        
        public float BeforeDelay { get; set; }
        public float AfterDelay { get; set; }
    }


    [GameDataBase(typeof(EnemyAtkGameData), "EnemyAtkGameData")]
    public class EnemyAtkGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<EnemyAtkGameData>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<EnemyAtkGameData>());
            }
            
            list.Add((EnemyAtkGameData)data);
        }
        
        public void OnInitialize(GameDataManager manager)
        {
            
        }

        public List<EnemyAtkGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}