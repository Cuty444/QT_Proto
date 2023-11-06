using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Core.Data;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)JelloLeftHand.States.Dead)]
    public class JelloLeftHandDeadState : FSMState<JelloLeftHand>
    {
        private const float ReleaseTime = 2.0f;

        private static readonly int DeadAnimHash = Animator.StringToHash("IsDead");
        
        private float _releaseTime;
        private bool _isReleased;
        
        public JelloLeftHandDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(DeadAnimHash, true);
            _ownerEntity.SetPhysics(false);
            _ownerEntity.BallObject.localPosition = Vector3.up * _ownerEntity.BallHeightMin;

            _ownerEntity.ShadowSprite.DOFade(0, 1).SetEase(Ease.InQuad);
            
            if (_ownerEntity.Steering.IsStuck())
            {
                _ownerEntity.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InQuad);
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Monster_WaterDrop);
            }
            
            HitAbleManager.Instance.UnRegister(_ownerEntity);
            ProjectileManager.Instance.UnRegister(_ownerEntity);

            _releaseTime = Time.timeSinceLevelLoad;
            _isReleased = false;
        }

        public override void UpdateState()
        {
            if (!_isReleased && (Time.timeSinceLevelLoad - _releaseTime) >= ReleaseTime)
            {
                _isReleased = true;
                _ownerEntity.gameObject.SetActive(false);
            }
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(DeadAnimHash, false);
            _ownerEntity.SetPhysics(true);
            _ownerEntity.ShadowSprite.DOFade(0.5f, 1).SetEase(Ease.InQuad);
        }
        
    }
}
