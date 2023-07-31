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

        private float _rigidTime;
        private float _rigidTimer;

        private SoundManager _soundManager;
        private PlayerManager _playerManager;
        
        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _playerManager = SystemManager.Instance.PlayerManager;
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                _rigidTimer = _rigidTime;
            });
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(RigidAnimHash, true);
            _rigidTimer = 0;

            _ownerEntity.OnProjectileHitEvent.AddListener(OnProjectileHit);
            
            if (_ownerEntity.HP <= 0)
            {
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
                _ownerEntity.MaterialChanger.SetRigidMaterial();
                
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
            }
            else
            {
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
                _ownerEntity.MaterialChanger.SetHitMaterial();
            }
        }

        public override void ClearState()
        {
            _ownerEntity.OnProjectileHitEvent.RemoveListener(OnProjectileHit);
            _ownerEntity.MaterialChanger.ClearMaterial();
        }

        public override void UpdateState()
        {
            _rigidTimer += Time.deltaTime;
            if (_rigidTimer > _rigidTime)
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

        private void OnProjectileHit(Vector2 dir, float power, LayerMask bounceMask)
        {
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state)?.InitializeState(dir, power, bounceMask);
        }
        
    }
}