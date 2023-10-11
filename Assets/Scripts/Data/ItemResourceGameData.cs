using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class ItemResourceGameData : IGameData
    {
        public enum ResourceTypes    
        {
            Bat,
            MoveSound,
            ProjectileTrail,
            SwingSound
        }
        
        public int Index { get; set; }
        public ResourceTypes ResourceType { get; set; }
        public string ResourcePath { get; set; }
    }


    [GameDataBase(typeof(ItemResourceGameData), "ItemResourceGameData")]
    public class ItemResourceGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<ItemResourceGameData>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<ItemResourceGameData>());
            }
            
            list.Add((ItemResourceGameData)data);
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public List<ItemResourceGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}