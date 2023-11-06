using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class JelloHandData : ScriptableObject
    {
        [field: SerializeField] public float AttackCoolTime { get; private set; }
        
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("발사 패턴")]
        [field: SerializeField] public float ShootDistance { get; private set; }
        
        [field: Space]
        [field: Space]
        [field: Space]
        [field:Header("돌진 패턴")]
        [field: SerializeField] public float RushDistance { get; private set; }
        
    }
}
