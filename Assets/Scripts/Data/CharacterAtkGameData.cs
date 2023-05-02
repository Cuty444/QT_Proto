using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class CharacterAtkGameData : IGameData
    {
        public int Index { get; set; }
        public float ThrowCooldown { get; set; }
        public float ThrowAfterDelay { get; set; }
        public float ThrowSpd { get; set; }
        public float ThrowBounceCount { get; set; }
        public float SwingCooldown { get; set; }
        public float SwingAfterDelay { get; set; }
        public float SwingRad { get; set; }
        public int SwingCentralAngle { get; set; }
        public float ChargeTime1 { get; set; }
        public float ChargeTime2 { get; set; }
        public float ChargeTime3 { get; set; }
        public int ChargeAtkPierce { get; set; }
        public int ChargeShootSpd1 { get; set; }
        public int ChargeShootSpd2 { get; set; }
        public int ChargeShootSpd3 { get; set; }
        public int ChargeShootSpd4 { get; set; }
        public int ChargeBounceCount1 { get; set; }
        public int ChargeBounceCount2 { get; set; }
        public int ChargeBounceCount3 { get; set; }
        public int ChargeBounceCount4 { get; set; }
        public int ChargeRigidDmg1 { get; set; }
        public int ChargeRigidDmg2 { get; set; }
        public int ChargeRigidDmg3 { get; set; }
        public int ChargeRigidDmg4 { get; set; }
        public int ChargeProjectileDmg1 { get; set; }
        public int ChargeProjectileDmg2 { get; set; }
        public int ChargeProjectileDmg3 { get; set; }
        public int ChargeProjectileDmg4 { get; set; }        
        public float ReflectCorrection { get; set; }
        public int AtkDmgPer { get; set; }
    }

    [GameDataBase((typeof(CharacterAtkGameData)),"CharacterAtkGameData")]
    public class CharacterAtkGameDataBase : IGameDataBase
    {
        private readonly Dictionary<int, CharacterAtkGameData> _datas = new();

        public void RegisterData(IGameData data)
        {
            _datas.Add(data.Index, (CharacterAtkGameData)data);
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
