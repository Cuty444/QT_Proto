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
        private readonly Dictionary<int, BuffCalculator> _datas = new();

        public void RegisterData(IGameData data)
        {
            var effectData = data as BuffEffectGameData;
            var buffEffect = new BuffCalculator(effectData);

            if (buffEffect.IsAvailable)
            {
                _datas.Add(data.Index, buffEffect);
            }
        }

        public BuffCalculator GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}