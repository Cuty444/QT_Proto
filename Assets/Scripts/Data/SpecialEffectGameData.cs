using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT
{
    
    
    public class SpecialEffectGameData : IGameData
    {
        public int Index { get; set; }
        
        public ItemEffectTypes EffectType { get; set; }
        
        public float Param1 { get; set; }
        public float Param2 { get; set; }
        public float Param3 { get; set; }
        public float Param4 { get; set; }
    }


    [GameDataBase(typeof(SpecialEffectGameData), "SpecialEffectGameData")]
    public class SpecialEffectGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, SpecialEffectGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (SpecialEffectGameData)data);
        }

        public SpecialEffectGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}