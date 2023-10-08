using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT
{
    public class MoveDust : MonoBehaviour
    {
        private const string MoveDustEffectPath = "Effect/Prefabs/FX_P_Move_Dust.prefab";
        private const string MoveGardenEffectPath = "Effect/Prefabs/FX_P_Move_Garden.prefab";
        [SerializeField] private Transform playerTransform;

        [SerializeField] private StudioEventEmitter _studioEventEmitter;
        [SerializeField] private StudioParameterTrigger _stone;
        [SerializeField] private StudioParameterTrigger _flower;
        private ResourceManager _resourceManager;
        private SoundManager _soundManager;
        private PlayerManager _playerManager;

        private void Start()
        {
            _resourceManager = SystemManager.Instance.ResourceManager;
            _soundManager = SystemManager.Instance.SoundManager;
            _playerManager = SystemManager.Instance.PlayerManager;
        }

        public void LeftDust()
        {
            _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position,
                Quaternion.Euler(0f,0f,_playerManager.Player.IsMoveFlip ? 70f: 0f));
            MoveSoundOn();
        }

        public void RightDust()
        {
            _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position,
                Quaternion.Euler(0f,0f,_playerManager.Player.IsMoveFlip ? 70f: 0f));
            MoveSoundOn();
        }

        private void MoveSoundOn()
        {
            _studioEventEmitter.Play();
            
            _stone.TriggerParameters();
            //Lookup(_soundManager.SoundData.WalkSFX,"Stone");
            _soundManager.PlayOneShot(_stone.Emitters[0].Target.EventReference);
        }
    }
}
