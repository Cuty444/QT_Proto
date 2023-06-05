using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)Enemy.States.Dead)]
    public class EnemyDeadState : FSMState<Enemy>
    {
        private static readonly int DeadAnimHash = Animator.StringToHash("Dead");
        
        public EnemyDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetTrigger(DeadAnimHash);
            _ownerEntity.SetPhysics(false);
            _ownerEntity.BallObject.localPosition = Vector3.up * _ownerEntity.BallHeightMin;
            _ownerEntity.HpCanvas.gameObject.SetActive(false);
            PlayerManager _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.OnGoldValueChanged.Invoke(_playerManager.Player.GetGoldCost() + _ownerEntity.RandomGoldDrop());
            if (_ownerEntity.IsFall)
            {
                _ownerEntity.FallScale();
            }
        }
    }
}
