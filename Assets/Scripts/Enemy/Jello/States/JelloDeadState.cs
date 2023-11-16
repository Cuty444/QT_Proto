using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Sound;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Dead)]
    public class JelloDeadState : FSMState<Jello>
    {
        private static readonly int DeadAnimHash = Animator.StringToHash("IsDead");
        
        private SoundManager _soundManager;
        private JelloData _data;

        private float _timer;
        
        public JelloDeadState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _ownerEntity.SetPhysics(false);
            
            _ownerEntity.Animator.SetBool(DeadAnimHash, true);
            
            _ownerEntity.LeftHand.Hit(Vector2.zero, 99999, AttackType.Ball);
            _ownerEntity.RightHand.Hit(Vector2.zero, 99999, AttackType.Ball);
            
            SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnKillEnemy, null);
        }

        public override void ClearState()
        {
            _ownerEntity.SetPhysics(true);
            _ownerEntity.Animator.SetBool(DeadAnimHash, false);
        }

    }
}
