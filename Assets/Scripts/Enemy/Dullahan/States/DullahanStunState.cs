using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Stun)]
    public class DullahanStunState : FSMState<Dullahan>
    {
        private readonly int IsStunAnimHash = Animator.StringToHash("IsStun");
        
        private float _time;
        
        public DullahanStunState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(IsStunAnimHash, true);
            _time = 0;
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;
            if (_time > _ownerEntity.DullahanData.StunTime)
            {
                _ownerEntity.ChangeState(Dullahan.States.Normal);
            }
        }

        public override void FixedUpdateState()
        {
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsStunAnimHash, false);
        }
    }
}