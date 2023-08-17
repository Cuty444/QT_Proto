using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

using GradeTypes = QT.ItemGameData.GradeTypes;


namespace QT
{
    public class DropGameData : IGameData
    {
        public int Index { get; }
        public float Normal { get; }
        public float Rare { get; }
        public float Cursed { get; }
        public float Active { get; }
        public float Hp { get; }
        public float Gold { get; }
    }
    
    
    public class DropPercentage
    {
        private readonly float[] _keys = new float[(int)GradeTypes.Max];
        private readonly float _max;

        public DropPercentage(DropGameData data)
        {
            _keys[0] = data.Normal;
            _keys[1] = _keys[0] + data.Rare;
            _keys[2] = _keys[1] + data.Cursed;
            _keys[3] = _keys[2] + data.Active;
            _keys[4] = _keys[3] + data.Hp;
            _max = _keys[5] = _keys[4] +data.Gold;
        }

        public GradeTypes RandomGradeType()
        {
            float key = Random.Range(0, _max);

            for (int i = 0; i < (int) GradeTypes.Max; ++i)
            {
                if (key < _keys[i])
                {
                    return (GradeTypes)i;
                }
            }

            return GradeTypes.Normal;
        }
    }
    
    
    [GameDataBase(typeof(DropGameData),"DropGameData")]
    public class DropGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, DropPercentage> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, new DropPercentage((DropGameData)data));
        }
        
        public DropPercentage GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
