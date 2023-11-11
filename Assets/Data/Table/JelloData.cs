using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class JelloData : ScriptableObject
    {
        [field: SerializeField] public float AttackCoolTime { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("발사 패턴")]
        [field: SerializeField] public float ShootDistance { get; private set; }
        [field: SerializeField] public float ShootPositionCorrectionSpeedMultiplier { get; private set; }
        [field: Space]
        [field: Space]
        [field: SerializeField] public float ShootReadyDelay { get; private set; }
        [field: Space]
        [field: SerializeField] public float ShootRightHandDelay { get; private set; }
        [field: SerializeField] public int ShootRightHandShootId { get; private set; }
        [field: Space]
        [field: SerializeField] public float ShootLeftHandDelay { get; private set; }
        [field: SerializeField] public int ShootLeftHandShootId { get; private set; }
        [field: Space]
        [field: SerializeField] public float ShootFinalDelay { get; private set; }
        [field: SerializeField] public int ShootFinalShootId { get; private set; }
        [field: SerializeField] public int ShootFinalShootId2 { get; private set; }
        
        
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("돌진 패턴")]
        [field: SerializeField] public float RushDistance { get; private set; }
        [field: Space]
        [field: Space]
        [field: SerializeField] public float RushReadyTime { get; private set; }
        [field: Space]
        [field: SerializeField] public float RushHitCheckDelay { get; private set; }
        [field: SerializeField] public float RushLengthTime { get; private set; }
        [field: SerializeField] public float RushAirTime { get; private set; }
        [field: SerializeField] public float RushEndDelay { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float RushSpeed { get; private set; }
        [field: SerializeField] public float RushAirSpeed { get; private set; }
        [field: Space]
        [field: SerializeField] public float RushHitDamage { get; private set; }
        [field: SerializeField] public int RushAtkId { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("분열")]
        
        [field: SerializeField] public float SplitCondition { get; private set; }
        
        [field: SerializeField] public float SplitDelay { get; private set; }
        [field: SerializeField] public float SplitAfterDelay { get; private set; }
        
        
        [field: SerializeField] public float SplitShootSpeed { get; private set; }
        [field: Space]
        [field: SerializeField] public int RightHandEnemyId { get; private set; }
        [field: SerializeField] public int LeftHandEnemyId { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("분열 움직임")]
        
        [field: SerializeField] public float SplitMoveDistance { get; private set; }
        [field: SerializeField] public float SplitAttackCoolTime { get; private set; }
        

        [field: Space]
        [field: Space]
        [field:Header("내려찍기")]
        [field: SerializeField] public float StompReadyTime { get; private set; }
        [field: SerializeField] public float StompLengthTime { get; private set; }
        [field: SerializeField] public float StompEndDelay { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float StompAirSpeed { get; private set; }
        [field: SerializeField] public int StompShootId { get; private set; }
        
        [field: Space]
        [field: SerializeField] public int StompRepeatCount { get; private set; }
        
    }
}
