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

            if (_ownerEntity.HP <= 0)
            {
                _ownerEntity.OnHitEvent.AddListener(OnDamage);
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
                _ownerEntity.MaterialChanger.SetRigidMaterial();
                
                ProjectileManager.Instance.Register(_ownerEntity);
            }
            else
            {
                if (_ownerEntity.HitAttackType == AttackType.Swing)
                {
                    _ownerEntity.OnHitEvent.AddListener(OnDamage);
                    _ownerEntity.Rigidbody.velocity = Vector2.zero;
                    ProjectileManager.Instance.Register(_ownerEntity);
                }

                _ownerEntity.MaterialChanger.SetHitMaterial();
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
            }
        }

        public override void ClearState()
        {
            _ownerEntity.OnHitEvent.RemoveListener(OnDamage);
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
                    ProjectileManager.Instance.UnRegister(_ownerEntity);
                }
                else
                {
                    _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                    _ownerEntity.RevertToPreviousState();
                }
            }
        }

        private void OnDamage(Vector2 dir, float power, LayerMask bounceMask)
        {
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state)?.InitializeState(dir, power, bounceMask);
        }
        
    }
}