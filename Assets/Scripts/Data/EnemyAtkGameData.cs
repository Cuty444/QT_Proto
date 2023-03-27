using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class EnemyAtkGameData : IGameData
    {
        public int Index { get; set; }
    }


    [GameDataBase(typeof(EnemyAtkGameData), "EnemyAtkGameData")]
    public class EnemyAtkGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, EnemyAtkGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.TryAdd(data.Index, (EnemyAtkGameData)data);
        }

        public EnemyAtkGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}