using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class SaddyData : ScriptableObject
    {
        [field:Header("공격 그룹")]
        
        [field: Space]
        [field: SerializeField] public float SwingCoolTime { get; private set; }
        [field: SerializeField] public float SwingDelayTime { get; private set; }
        [field: SerializeField] public float SwingPlayTime { get; private set; }
        [field: SerializeField] public int SwingAtkId { get; private set; }
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float AttackCoolTime { get; private set; }
        
        [field: Space]
        [field: SerializeField] public float RollingAttackDistance { get; private set; }
        [field: Space]
        [field: Space]
        [field: SerializeField] public float RollingAttackStartSpeed { get; private set; }
        [field: SerializeField] public float RollingAttackEndSpeed { get; private set; }
        [field: SerializeField] public float RollingAttackSteerAngle { get; private set; }
        [field: Space]
        
        [field: SerializeField] public float RollingAttackDelay { get; private set; }
        [field: SerializeField] public float RollingAttackLengthTime { get; private set; }
        [field: SerializeField] public float RollingAttackAtkDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public int RollingAttackAtkId { get; private set; }
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float SideStepDistance { get; private set; }
        [field: Space]
        [field: Space]
        [field: SerializeField] public float SideStepSpeed { get; private set; }
        [field: Space]
        [field: SerializeField] public float SideStepTime { get; private set; }
        [field: SerializeField] public float SideStepEndTime { get; private set; }
        [field: Space]
        [field: SerializeField] public float SideStepAtkDelay { get; private set; }
        [field: Space]
        [field: SerializeField] public int SideStepAtkId { get; private set; }
        [field: SerializeField] public int SideStepEndAtkId { get; private set; }

        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("핑퐁 그룹")]
        [field: SerializeField] public float ThrowDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public int ThrowAtkId { get; private set; }
    }
}
