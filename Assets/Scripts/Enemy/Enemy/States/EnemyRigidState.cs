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
        private const string TeleportLinePath = "Prefabs/TeleportLine.prefab";

        private float _rigidStartTime;
        private float _rigidTime;

        private SoundManager _soundManager;
        private GlobalDataSystem _globalDataSystem;
        private PlayerManager _playerManager;
        public EnemyRigidState(IFSMEntity owner) : base(owner)
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _playerManager = SystemManager.Instance.PlayerManager;
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            _playerManager.PlayerMapClearPosition.AddListener((arg) =>
            {
                _rigidTime = 0f;
            });
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(RigidAnimHash, true);
            _rigidStartTime = Time.time;

            if (_ownerEntity.HP <= 0)
            {
                _ownerEntity.OnHitEvent.AddListener(OnDamage);
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.DeadAfterStunTime;
                _soundManager.PlayOneShot(_soundManager.SoundData.MonsterStun);
                _ownerEntity.MaterialChanger.SetRigidMaterial();
                SetTeleportLineObjects();
                _playerManager.PlayerMapClearPosition.AddListener((arg) => LineRendererClear());
                SystemManager.Instance.ProjectileManager.Register(_ownerEntity);
            }
            else
            {
                if (_ownerEntity.HitAttackType == AttackType.Swing)
                {
                    _ownerEntity.OnHitEvent.AddListener(OnDamage);
                    _ownerEntity.Rigidbody.velocity = Vector2.zero;
                    SystemManager.Instance.ProjectileManager.Register(_ownerEntity);
                }

                _ownerEntity.MaterialChanger.SetHitMaterial();
                _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
            }
        }

        public override void ClearState()
        {
            LineRendererClear();

            _ownerEntity.OnHitEvent.RemoveListener(OnDamage);
            _ownerEntity.MaterialChanger.ClearMaterial();
        }

        public override void UpdateState()
        {
            if (_rigidStartTime + _rigidTime < Time.time)
            {
                if (_ownerEntity.HP <= 0)
                {
                    _ownerEntity.ChangeState(Enemy.States.Dead);
                    SystemManager.Instance.ProjectileManager.UnRegister(_ownerEntity);
                }
                else
                {
                    _ownerEntity.Animator.SetBool(RigidAnimHash, false);
                    _ownerEntity.RevertToPreviousState();
                }
            }
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            if (_ownerEntity.TeleportLineRenderer != null)
            {
                Vector2 targetPosition = _playerManager.Player.transform.position;
                SetTeleportLine(_ownerEntity.TeleportLineRenderer,targetPosition,
                    _playerManager.Player.StatComponent.GetStat(PlayerStats.TeleportAllowableDistance)
                    < Vector2.Distance(targetPosition, _ownerEntity.transform.position));
            }
        }

        private void OnDamage(Vector2 dir, float power, LayerMask bounceMask)
        {
            var state = _ownerEntity.ChangeState(Enemy.States.Projectile);
            ((EnemyProjectileState) state)?.InitializeState(dir, power, bounceMask);
        }

        private async void SetTeleportLineObjects()
        {
            var line = await SystemManager.Instance.ResourceManager.GetFromPool<LineRenderer>(TeleportLinePath);
            line.positionCount = 0;
            _ownerEntity.TeleportLineRenderer = line;
        }
        
        private void SetTeleportLine(LineRenderer lineRenderer,Vector2 position,bool isDistance)
        {
            if (!isDistance)
            {
                if (_playerManager.Player.CurrentStateIndex >= (int)Player.States.Fall)
                {
                    lineRenderer.positionCount = 0;
                    return;
                }
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, position);
                lineRenderer.SetPosition(1, _ownerEntity.transform.position);
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }

        private void LineRendererClear()
        {
            if (_ownerEntity.TeleportLineRenderer != null)
            {
                _ownerEntity.TeleportLineRenderer.positionCount = 0;
                SystemManager.Instance.ResourceManager.ReleaseObject(TeleportLinePath,
                    _ownerEntity.TeleportLineRenderer);
                _ownerEntity.TeleportLineRenderer = null;
            }
        }
    }
}