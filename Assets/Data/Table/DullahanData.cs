using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class DullahanData : ScriptableObject
    {
        // 돌진
        [field: SerializeField] public float RushDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public float RushReadyTime { get; private set; }
        [field: SerializeField] public float RushLengthTime { get; private set; }
        [field: SerializeField] public float RushAirTime { get; private set; }
        [field: Space]
        [field: SerializeField] public float RushSpeed { get; private set; }
        [field: SerializeField] public float RushAirSpeed { get; private set; }
        [field: SerializeField] public float RushHitDamage { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float StunTime { get; private set; }
        
        
        
        // 내려찍기
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float SmashDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public int SmashAtkId { get; private set; }
        
        
        
        // 던지기
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float ThrowDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public float ThrowReadyTime { get; private set; }
        [field: SerializeField] public int ThrowAtkId { get; private set; }
        
        
        // 소환
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float SummonDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public float SummonReadyTime { get; private set; }
        [field: SerializeField] public float SummonTime { get; private set; }

        
        
        // 점프
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float JumpDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public float JumpReadyTime { get; private set; }
        [field: SerializeField] public float JumpingTime { get; private set; }
        [field: SerializeField] public float JumpLengthTime { get; private set; }
        [field: SerializeField] public float JumpEndTime { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float JumpMoveSpeed { get; private set; }
        [field: SerializeField] public float JumpMinMoveSpeed { get; private set; }
        
        [field: Space]
        [field: SerializeField] public int LandingAtkId { get; private set; }
        [field: SerializeField] public float LandingHitDamage { get; private set; }
        [field: SerializeField] public float LandingHitRange { get; private set; }
        
        
        // 공격
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float AttackDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public float AttackBeforeDelay { get; private set; }
        [field: SerializeField] public float AttackAfterDelay { get; private set; }
        [field: SerializeField] public float AttackDamage { get; private set; }
        
    }
}
