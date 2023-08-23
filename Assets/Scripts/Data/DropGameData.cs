using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

using GradeTypes = QT.ItemGameData.GradeTypes;


namespace QT
{
    public enum DropGameType
    {
         Start = 9001,
         Shop = 9002,
         Select = 9003
     }
    
    public class DropGameData : IGameData
    {
        public int Index { get; private set;}
        public float Normal { get; private set; }
        public float Rare { get; private set;}
        public float Cursed { get; private set;}
        public float Active { get; private set;}
        public float Hp { get; private set;}
        public float Gold { get; private set;}
    }
    
    
    public class DropPercentage
    {
        public readonly float[] Percentages = new float[(int)GradeTypes.Max];
        private readonly float _max;

        public DropPercentage(DropGameData data)
        {
            _max += Percentages[(int)GradeTypes.Normal] = data.Normal;
            _max += Percentages[(int)GradeTypes.Rare] = data.Rare;
            _max += Percentages[(int)GradeTypes.Cursed] = data.Cursed;
            _max += Percentages[(int)GradeTypes.Active] = data.Active;
            _max += Percentages[(int)GradeTypes.Hp] = data.Hp;
            _max += Percentages[(int)GradeTypes.Gold] = data.Gold;
        }

        public GradeTypes RandomGradeType()
        {
            float key = Random.Range(0, _max);
            float target = 0;

            for (int i = 0; i < (int) GradeTypes.Max; i++)
            {
                target += Percentages[i];
                if (key < target)
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
