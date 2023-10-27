using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace QT.InGame
{
    public class SaddyData : ScriptableObject
    {
        // 던지기
        [field: Space]
        [field: Space]
        [field: Space]
        [field: SerializeField] public float ThrowDistance { get; private set; }
        [field: Space]
        [field: SerializeField] public int ThrowAtkId { get; private set; }
    }
}
