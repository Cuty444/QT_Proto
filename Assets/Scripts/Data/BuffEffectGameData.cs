using System.Collections.Generic;
using QT.Core;
using QT.InGame;

namespace QT
{
    public class BuffEffectGameData : IGameData
    {
        public int Index { get; set; }
        
        public string ApplyStat { get; set; }
        public string ApplyValue { get; set; }
        
        public StatModifier.ModifierType ValueOperatorType { get; set; }
        
        public float Duration { get; set; }
    }


    [GameDataBase(typeof(BuffEffectGameData), "BuffEffectGameData")]
    public class BuffEffectGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<BuffCalculator>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<BuffCalculator>());
            }
            
            var buffCalculator = new BuffCalculator(data as BuffEffectGameData);
            if (buffCalculator.IsAvailable)
            {
                list.Add(buffCalculator);
            }
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public List<BuffCalculator> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}