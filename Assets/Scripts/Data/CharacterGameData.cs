using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public class CharacterGameData : IGameData
    {
        public int Index { get; set; }
        
        public int MaxHP { get; set; }
        public float PCHitboxRad { get; set; }
        
        public float MovementSpd { get; set; }
        public float ChargeMovementSpd { get; set; }
        
        public float MercyInvincibleTime { get; set; }
        
        public float DodgeCooldown { get; set; }
        public float DodgeInvincibleTime { get; set; }
        public float DodgeDurationTime { get; set; }
        public float DodgeAddForce { get; set; }
        
        public int GoldGain { get; set; }
    }

    [GameDataBase(typeof(CharacterGameData),"CharacterGameData")]
    public class CharacterGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, CharacterGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (CharacterGameData)data);
        }

        public void OnInitialize(GameDataManager manager)
        {
            
        }

        public CharacterGameData GetData(int id)
        {
            if (_datas.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
