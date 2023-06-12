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
        [SerializeField] private ParticleSystem[] _swingSlashParticle;
        [SerializeField] private TrailRenderer[] _teleportEffectLines;
        [SerializeField] private ParticleSystem _itemPickUpParticle;
        [SerializeField] private SpriteRenderer _itemPickUpImage;
        [SerializeField] private Animator _itemPickUpAnimator;

        private void EffectSetup()
        {
            _chargingMaintainParticle.Stop();
            _playerHitParticle.Stop();
            for (int i = 0; i < _swingSlashParticle.Length; i++)
            {
                _swingSlashParticle[i].Stop();
            }
            for (int i = 0; i < _chargingLevelParticle.Length; i++)
            {
                _chargingLevelParticle[i].Stop();
            }

            for (int i = 0; i < _dashParticle.Length; i++)
            {
                _dashParticle[i].Stop();
            }
            _itemPickUpParticle.Stop();
            _itemPickUpImage.enabled = false;
            TeleportEffectEmitting(false);
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
            _chargingLevelParticle[0].Play();
        }

        public void ChargingEffectStop()
        {
            _chargingLevelParticle[0].Stop();
        }
        
        public void FullChargingEffectPlay()
        {
            _chargingMaintainParticle.Play();
        }

        public void swingSlashEffectPlay(bool flip)
        {
            _swingSlashParticle[flip ? 1 : 0].Play();
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
            _itemPickUpAnimator.Play("ItemGainAnimation",-1,0f);
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
