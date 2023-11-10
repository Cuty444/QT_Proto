using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) JelloRightHand.States.Spawn)]
    public class JelloRightHandSpawnState : FSMState<JelloRightHand>
    {
        private static readonly int StunAnimHash = Animator.StringToHash("IsStun");

        private float _clearTime;
        private float _timer;
        
        private SoundManager _soundManager;
        
        public JelloRightHandSpawnState(IFSMEntity owner) : base(owner)
        {
            _soundManager = SystemManager.Instance.SoundManager;
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.AddForce(Vector2.left * _ownerEntity.JelloData.SpawnSpeed, ForceMode2D.Impulse);
            
            //_ownerEntity.Animator.SetBool(StunAnimHash, true);
            
            _clearTime = _ownerEntity.JelloData.SpawnTime;
            _timer = 0;
        }

        public override void ClearState()
        {
            //_ownerEntity.Animator.SetBool(StunAnimHash, false);
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (_timer > _clearTime)
            {
                _ownerEntity.ChangeState(JelloRightHand.States.Normal);
            }
        }
        
    }
}