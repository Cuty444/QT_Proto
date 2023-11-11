using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.HandSkill)]
    public class JelloHandSkillState : FSMState<Jello>
    {
        private float _timer;
        
        public JelloHandSkillState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            var rightHand = _ownerEntity.RightHand;
            if (rightHand.CurrentStateIndex < (int)JelloRightHand.States.Projectile)
            {
                rightHand.ChangeState(JelloRightHand.States.Rush);
            }

            var leftHand = _ownerEntity.LeftHand;
            if (leftHand.CurrentStateIndex < (int)JelloLeftHand.States.Projectile)
            {
                leftHand.Shooter.PlayEnemyAtkSequence(leftHand.JelloData.ShootAtkId, leftHand.Owner);
            }

            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (_timer > _ownerEntity.JelloData.HandSkillLengthTime)
            {
                _ownerEntity.RevertToPreviousState();
            }
        }
    }
}