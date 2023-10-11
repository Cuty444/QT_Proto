using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class ShootGameData : IGameData
    {
        public int Index { get; set; }
        
        public float ShootAngle { get; set; }
        public int MaxBounceCount { get; set; }
        public int ProjectileDataId { get; set; }
        public float InitalSpd { get; set; }
    }


    [GameDataBase(typeof(ShootGameData), "ShootGameData")]
    public class ShootGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<ShootGameData>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<ShootGameData>());
            }
            
            list.Add((ShootGameData)data);
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public List<ShootGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}