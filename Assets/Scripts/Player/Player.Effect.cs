using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QT.InGame
{
    public partial class Player
    {
        [SerializeField] private ParticleSystem[] _dashParticle;
        [SerializeField] private ParticleSystem _playerHitParticle;
        [SerializeField] private ParticleSystem _chargingMaintainParticle;
        [SerializeField] private ParticleSystem _chargingLevelParticle;
        [SerializeField] private TrailRenderer[] _teleportEffectLines;
        [SerializeField] private ParticleSystem _warpEffectParticle;

        [SerializeField] private ParticleSystem _itemPickUpParticle;
        [SerializeField] private Image _itemPickUpImage;
        [SerializeField] private TweenAnimator _itemPickUpAnimator;

        private void EffectSetup()
        {
            _chargingMaintainParticle.Stop();
            _playerHitParticle.Stop();
            _chargingLevelParticle.Stop();

            for (int i = 0; i < _dashParticle.Length; i++)
            {
                _dashParticle[i].Stop();
            }
            _itemPickUpParticle.Stop();
            _warpEffectParticle.Stop();
            _itemPickUpImage.enabled = false;
            TeleportEffectEmitting(false);
            
            _playerManager.PlayerMapTeleportPosition.AddListener((arg) =>
            {
                _warpEffectParticle.Play();
            });
        }
        
        public void DodgeEffectPlay(Vector2 dir)
        {
            bool isSide = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);
            if (isSide)
            {
                _dashParticle[dir.x < 0 ? 2 : 3].Play();
            }
            else
            {
                _dashParticle[dir.y > 0 ? 0 : 1].Play();
            }
            //_dashLRTransform.localScale = new Vector3((dir.x > 0 ? -1 : 1), 1, 1);
        }
        
        public void PlayerHitEffectPlay()
        {
            _playerHitParticle.Play();
        }

        public void ChargingEffectPlay()
        {
            _chargingLevelParticle.Play();
        }

        public void ChargingEffectStop()
        {
            _chargingLevelParticle.Stop();
        }
        
        public void FullChargingEffectPlay()
        {
            _chargingMaintainParticle.Play();
        }

        public void TeleportEffect(bool isActive)
        {
            TeleportEffectEmitting(isActive);
        }

        public void GainItem(Sprite sprite)
        {
            _itemPickUpParticle.Play();
            _itemPickUpImage.enabled = true;
            _itemPickUpImage.sprite = sprite;
            _itemPickUpAnimator.ReStart();
        }

        private void TeleportEffectEmitting(bool isActive)
        {
            for (int i = 0; i < _teleportEffectLines.Length; i++)
            {
                _teleportEffectLines[i].emitting = isActive;
            }
        }
    }
}
