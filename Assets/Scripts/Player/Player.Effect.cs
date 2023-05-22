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
        
        public void DodgeEffectPlay(Vector2 dir)
        {
            bool isSide = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);
            
            _dashParticle[isSide ? 1 : 0].Play();
            _dashLRTransform.localScale = new Vector3((dir.x > 0 ? -1 : 1), 1, 1);
        }
        
        public void PlayerHitEffectPlay()
        {
            _playerHitParticle.Play();
        }

        public void FullChargingEffectPlay()
        {
            //if (_chargingMaintainParticle.isPlaying)
            //    return;
            _chargingMaintainParticle.Play();
        }

        public void FullChargingEffectStop()
        {
            _chargingLevelParticle[0].Stop();
            //_chargingMaintainParticle.Stop();
        }

        public void ChargingEffectPlay(int index)
        {
            if (index > 0)
                return;
            _chargingLevelParticle[index].Play();
        }

        public void swingSlashEffectPlay()
        {
            _swingSlashParticle.Play();
        }
    }
}
