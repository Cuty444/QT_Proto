using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) JelloLeftHand.States.Spawn)]
    public class JelloLeftHandSpawnState : FSMState<JelloLeftHand>
    {
        private static readonly int StunAnimHash = Animator.StringToHash("IsStun");

        private float _clearTime;
        private float _timer;
        
        private SoundManager _soundManager;
        
        public JelloLeftHandSpawnState(IFSMEntity owner) : base(owner)
        {
            _soundManager = SystemManager.Instance.SoundManager;
        }

        public override void InitializeState()
        {
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.AddForce(Vector2.right * _ownerEntity.JelloData.SpawnSpeed, ForceMode2D.Impulse);
            
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
                _ownerEntity.ChangeState(JelloLeftHand.States.Normal);
            }
        }
        
    }
}