using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class DropGameData : IGameData
    {
        public int Index { get; set; }
        public float Normal { get; set; }
        public float Rare { get; set; }
        public float Cursed { get; set; }
        public float Weapon { get; set; }
        public float Hp { get; set; }
        public float Gold { get; set; }
    }
    [GameDataBase(typeof(DropGameData),"DropGameData")]
    public class DropGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, DropGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (DropGameData)data);
        }
        
        public DropGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
