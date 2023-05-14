using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        [SerializeField] private Transform _dashLRTransform;
        [SerializeField] private ParticleSystem[] _dashParticle;
        [SerializeField] private ParticleSystem _playerHitParticle;
        [SerializeField] private ParticleSystem _chargingMaintainParticle;
        [SerializeField] private ParticleSystem[] _chargingLevelParticle;
        [SerializeField] private ParticleSystem _swingSlashParticle;
        private int _dodgeDirection;

        private void EffectSetup()
        {
            FullChargingEffectStop();
            _playerHitParticle.Stop();
            _swingSlashParticle.Stop();
            for (int i = 0; i < _chargingLevelParticle.Length; i++)
            {
                _chargingLevelParticle[i].Stop();
            }
        }
        
        public void DodgeEffectPlay()
        {
            _dashParticle[_dodgeDirection].Play();
        }

        public void DodgeEffectRotation(float rotation) // TODO : Particle System 좌우 반전 미적용 추후 패치 필요
        {
            _dashLRTransform.rotation = Quaternion.Euler(0f, rotation,0f);
        }

        public void PlayerHitEffectPlay()
        {
            _playerHitParticle.Play();
        }

        public void FullChargingEffectPlay()
        {
            if (_chargingMaintainParticle.isPlaying)
                return;
            _chargingMaintainParticle.Play();
        }

        public void FullChargingEffectStop()
        {
            _chargingMaintainParticle.Stop();
        }

        public void ChargingEffectPlay(int index)
        {
            _chargingLevelParticle[index].Play();
        }

        public void swingSlashEffectPlay()
        {
            _swingSlashParticle.Play();
        }
    }
}
