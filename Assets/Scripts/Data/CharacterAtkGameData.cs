using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class CharacterAtkGameData : IGameData
    {
        public int Index { get; set; }

        public float SwingCooldown { get; set; }
        public float SwingRad { get; set; }
        public int SwingCentralAngle { get; set; }
        
        public float ChargeTime { get; set; }
        
        public int ProjectileGuide { get; set; }
        public int ProjectileExplosion { get; set; }
        public int ProjectilePierce { get; set; }
        
        public int ChargeShootSpd { get; set; }
        
        public int ChargeBounceCount { get; set; }
        
        public int ChargeRigidDmg1 { get; set; }
        public int ChargeRigidDmg2 { get; set; }
        
        public int ChargeProjectileDmg { get; set; }   
        
        public int EnemyProjectileDmg { get; set; }
        
        public float AtkDmgPer { get; set; }
    }

    [GameDataBase((typeof(CharacterAtkGameData)),"CharacterAtkGameData")]
    public class CharacterAtkGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, CharacterAtkGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (CharacterAtkGameData)data);
        }
        
        public void OnInitialize(GameDataManager manager)
        {
            
        }
        
        public CharacterAtkGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }
    
}
