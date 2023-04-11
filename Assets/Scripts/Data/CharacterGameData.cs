using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.Player
{
    public class CharacterGameData : IGameData
    {
        public int Index { get; set; }
        public int HPMax { get; set; }
        public float PCHitboxRad { get; set; }
        public int MovementSpd { get; set; }
        public int ChargeMovementSpd { get; set; }
        public float MercyInvincibleTime { get; set; }
        public float DodgeCooldown { get; set; }
        public float DodgeInvincibleTime { get; set; }
        public int ItemSlotMax { get; set; }
        public int BallStackMax { get; set; }
        public int GoldGain { get; set; }
        public int DefaultBallDataId { get; set; }
    }

    [GameDataBase(typeof(CharacterGameData),"CharacterGameData")]
    public class CharacterGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, CharacterGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (CharacterGameData)data);
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
