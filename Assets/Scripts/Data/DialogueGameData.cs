using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class DialogueGameData : IGameData
    {
        public int Index { get; set; }
        public string LocaleId { get; set; }
        public float Duration { get; set; }
    }


    [GameDataBase(typeof(DialogueGameData), "DialogueGameData")]
    public class DialogueGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, List<DialogueGameData>> _datas = new();

        public void RegisterData(IGameData data)
        {
            if(!_datas.TryGetValue(data.Index, out var list))
            {
                _datas.Add(data.Index, list = new List<DialogueGameData>());
            }
            
            list.Add((DialogueGameData)data);
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public List<DialogueGameData> GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}