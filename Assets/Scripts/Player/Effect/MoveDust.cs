using System.Collections;
using System.Collections.Generic;
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
            if (_playerManager.Player.IsGarden)
            {
                _resourceManager.EmitParticle(MoveGardenEffectPath, playerTransform.position);
            }
            else
            {
                _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
            }
            MoveSoundOn();
        }

        public void RightDust()
        {
            if (_playerManager.Player.IsGarden)
            {
                _resourceManager.EmitParticle(MoveGardenEffectPath, playerTransform.position);
            }
            else
            {
                _resourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
            }
            MoveSoundOn();
        }

        private void MoveSoundOn()
        {
            if (_playerManager.Player.IsGarden)
            {
            }
            else
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.WalkSFX);
            }
        }
    }
}
