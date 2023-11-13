using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.Dead)]
    public class SaddyDeadState : FSMState<Saddy>
    {
        private static readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");
        private static readonly int IsSpawnAnimHash = Animator.StringToHash("IsSpawn");
        private static readonly int IsStunAnimHash = Animator.StringToHash("IsStun");
        
        private SoundManager _soundManager;
        private SaddyData _data;

        private float _timer;
        
        public SaddyDeadState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.Animator.SetBool(IsStunAnimHash, false);
            _ownerEntity.Animator.SetBool(IsSpawnAnimHash, false);
            
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, true);
        }

        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, false);
        }

    }
}
