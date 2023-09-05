using QT.Core;
using QT.Core.Data;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Enemy.States.Rigid)]
    public class EnemyRigidState : FSMState<Enemy>
    {
        private static readonly int RigidAnimHash = Animator.StringToHash("IsRigid");
        private static readonly int StunAnimHash = Animator.StringToHash("IsStun");

        private float _timer;
        
        private bool _isKnockBack;
        private float _knockBackTime;
        
        private float _clearTime;

        private SoundManager _soundManager;
        private PlayerManager _playerManager;
        private GlobalData _globalData;
        
        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            _soundManager = SystemManager.Instance.SoundManager;
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                _clearTime = _knockBackTime;
            });
        }

        public void InitializeState(Vector2 dir)
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.AddForce(dir * _globalData.KnockBackSpd, ForceMode2D.Impulse);
            
            _ownerEntity.Animator.SetBool(RigidAnimHash, true);
            
            _timer = 0;
            _knockBackTime = _globalData.KnockBackTime;
            _isKnockBack = true;
            
            if (_ownerEntity.HP <= 0)
            {
                _clearTime = _globalData.DeadAfterStunTime;
                _ownerEntity.MaterialChanger.SetRigidMaterial();
                
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
                _ownerEntity.Animator.SetBool(StunAnimHash, true);
                
                _ownerEntity.HpCanvas.gameObject.SetActive(false);
            }
            else
            {
                _clearTime = _globalData.RigidTime;
                _ownerEntity.MaterialChanger.SetHitMaterial();
            }
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

            if (_isKnockBack && _timer > _knockBackTime)
            {
                _ownerEntity.Rigidbody.velocity = Vector2.zero;
                _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                _isKnockBack = false;
            }
            
            if (_timer > _clearTime)
            {
                if (_ownerEntity.HP <= 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Dead);
                }
                else
                {
                    _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                    _ownerEntity.RevertToPreviousState();
                }
            }
        }
        
    }
}