using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QT
{
    public class MoveDust : MonoBehaviour
    {
        private const string MoveDustEffectPath = "Effect/Prefabs/FX_P_Move_Dust.prefab";
        private const string MoveSoundPath = "Assets/Sound/QT/Assets/Player_Walk_Stone_";
        [SerializeField] private Transform playerTransform;

        private ResourceManager _resourceManager;
        private SoundManager _soundManager;

        private void Start()
        {
            _resourceManager = SystemManager.Instance.ResourceManager;
            _soundManager = SystemManager.Instance.SoundManager;
        }

        public void LeftDust()
        {
            _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
            MoveSoundOn();
        }

        public void RightDust()
        {
            _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
            MoveSoundOn();
        }

        private void MoveSoundOn()
        {
            _soundManager.RandomSoundOneShot(MoveSoundPath,8);
        }
    }
}
