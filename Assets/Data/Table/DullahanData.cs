using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class DullahanData : ScriptableObject
    {
        [field: SerializeField] public float AttackRangeMin { get; private set; }
        [field: SerializeField] public float AttackRangeMax { get; private set; }
        
        [field: Space]
        [field: SerializeField] public int AttackAtkId { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float RushRangeMin { get; private set; }
        [field: SerializeField] public float RushRangeMax { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float RushReadyTime { get; private set; }
        [field: SerializeField] public float RushLengthTime { get; private set; }
        [field: Space]
        [field: SerializeField] public float RushSpeed { get; private set; }
        [field: SerializeField] public float RushHitDamage { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float ThrowRangeMin { get; private set; }
        [field: SerializeField] public float ThrowRangeMax { get; private set; }
        
        [field: Space]
        [field: SerializeField] public int ThrowAtkId { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float JumpRangeMin { get; private set; }
        [field: SerializeField] public float JumpRangeMax { get; private set; }
        
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
    }
}
