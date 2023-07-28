using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT
{
    
    
    public class ActiveEffectGameData : IGameData
    {
        public int Index { get; set; }
        
        public ItemEffectTypes ActiveEffectType { get; set; }
        
        public float Param1 { get; set; }
        public float Param2 { get; set; }
        public float Param3 { get; set; }
    }


    [GameDataBase(typeof(ActiveEffectGameData), "ActiveEffectGameData")]
    public class ActiveEffectGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, ActiveEffectGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (ActiveEffectGameData)data);
        }

        public ActiveEffectGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}