using System.Collections;
using System.Timers;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Rush)]
    public class DullahanRushState : FSMState<Dullahan>
    {
        private readonly int IsRushingAnimHash = Animator.StringToHash("IsRushing");
        private readonly int RushReadyAnimHash = Animator.StringToHash("RushReady");
        
        
        private bool _isReady;
        private Vector2 _dir;

        private float _time;

        public DullahanRushState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            _ownerEntity.SetDir(_dir,2);

            _isReady = false;
            _time = 0;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;

            _dir = new Vector2(_dir.x > 0 ? 1 : -1, _dir.y > 0 ? 1 : -1);
            
            _ownerEntity.Animator.SetTrigger(RushReadyAnimHash);
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;
            if (!_isReady)
            {
                if (_time > _ownerEntity.DullahanData.RushReadyTime)
                {
                    _isReady = true;
                }
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, true);

                return;
            }

            _ownerEntity.Rigidbody.velocity = _dir * _ownerEntity.DullahanData.RushSpeed;
            
            if (_time > _ownerEntity.DullahanData.RushLengthTime)
            {
                _ownerEntity.ChangeState(Dullahan.States.Normal);
                _ownerEntity.Animator.SetBool(IsRushingAnimHash, false);
            }
        }

        public override void FixedUpdateState()
        {
        }

        public override void ClearState()
        {
        }
        
    }
}