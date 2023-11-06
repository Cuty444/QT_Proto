using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) JelloRightHand.States.Rigid)]
    public class JelloRightHandRigidState : FSMState<JelloRightHand>
    {
        private static readonly int StunAnimHash = Animator.StringToHash("IsStun");

        private float _timer;
        
        private bool _isKnockBack;
        private float _knockBackTime;
        
        private float _clearTime;

        private SoundManager _soundManager;
        private GlobalData _globalData;
        
        public JelloRightHandRigidState(IFSMEntity owner) : base(owner)
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            _soundManager = SystemManager.Instance.SoundManager;
        }

        public void InitializeState(Vector2 dir)
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.AddForce(dir * _globalData.KnockBackSpd, ForceMode2D.Impulse);
            
            _timer = 0;
            _knockBackTime = _globalData.KnockBackTime;
            _isKnockBack = true;
            
            _clearTime = _globalData.DeadAfterStunTime;
            _ownerEntity.MaterialChanger.SetRigidMaterial();
                
            _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
            _ownerEntity.Animator.SetBool(StunAnimHash, true);
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(StunAnimHash, false);
            _ownerEntity.MaterialChanger.ClearMaterial();
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (!DungeonManager.Instance.IsBattle)
            {
                _clearTime = _knockBackTime;
            }
            
            if (_isKnockBack && _timer > _knockBackTime)
            {
                _ownerEntity.Rigidbody.velocity = Vector2.zero;
                _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                _isKnockBack = false;
            }
            
            if (_timer > _clearTime)
            {
                _ownerEntity.ChangeState(JelloRightHand.States.Dead);
            }
        }
        
    }
}