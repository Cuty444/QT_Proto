using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Stun)]
    public class SaddyStunState : FSMState<Saddy>
    {
        private static readonly int IsMoveAnimHash = Animator.StringToHash("IsMove");
        private static readonly int StunAnimHash = Animator.StringToHash("IsStun");
        
        private SoundManager _soundManager;
        
        private SaddyData _data;

        private bool _isStun;
        
        private float _timer;
        
        public SaddyStunState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.Animator.SetBool(StunAnimHash, true);
            _ownerEntity.Animator.SetBool(IsMoveAnimHash, false);

            _isStun = true;
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (_isStun)
            {
                if (_timer > _data.StunTime)
                {
                    _isStun = false;
                    _timer = 0;
                }
            }
            else
            {
                if (_timer > _data.StunAfterDelay)
                {
                    _ownerEntity.ChangeState(_ownerEntity.GetNextGroupStartState());
                }
            }
            
        }

        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.Animator.SetBool(StunAnimHash, false);
        }

    }
}
