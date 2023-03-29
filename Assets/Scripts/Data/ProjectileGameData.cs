using System.Collections.Generic;
using QT.Core;

namespace QT
{
    public class ProjectileGameData : IGameData
    {
        public int Index { get; set; }
    }


    [GameDataBase(typeof(ProjectileGameData), "ProjectileGameData")]
    public class ProjectileGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, ProjectileGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (ProjectileGameData)data);
        }

        public ProjectileGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }  
}