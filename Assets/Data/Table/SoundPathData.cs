using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.Sound
{
    public class SoundPathData : ScriptableObject
    {
        [field: SerializeField] public EventReference WalkSFX{ get; private set; }
        [field: SerializeField] public EventReference SwingSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerSwingHitSFX{ get; private set; }
        [field: SerializeField] public EventReference BallAttackSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerHitSFX{ get; private set; }
        [field: SerializeField] public EventReference ChargeSFX{ get; private set; }
        [field: SerializeField] public EventReference ChargeEndSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerThrowHitSFX{ get; private set; }
        [field: SerializeField] public EventReference PlayerDashSFX{ get; private set; }
        
        [field: SerializeField] public EventReference BallBounceSFX{ get; private set; }
        
        [field: SerializeField] public EventReference GameOverSFX{ get; private set; }
        
        [field: SerializeField] public EventReference MonsterStun{ get; private set; }
        [field: SerializeField] public EventReference MonsterFly{ get; private set; }
        
        [field: SerializeField] public EventReference Stage1BGM{ get; private set; }
        [field: SerializeField] public EventReference LoadingBGM{ get; private set; }
        
        [field: SerializeField] public string[] Bank{ get; private set; }
    }
}
