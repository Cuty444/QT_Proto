using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;
using QT.Sound;
using EventType = QT.Core.EventType;

namespace QT.InGame
{
    [FSMState((int) Player.States.Swing)]
    public class PlayerSwingState : PlayerMoveState
    {
        private const string HitLinePath = "Prefabs/HitLine.prefab";
        private const string SwingProjectileHitPath = "Effect/Prefabs/FX_M_Ball_Hit_Boom.prefab";
        
        private const string SwingBatHitPath = "Effect/Prefabs/FX_M_Small_Hit.prefab";
        private const string SwingChargedBatHitPath = "Effect/Prefabs/FX_M_Bat_Hit_Boom.prefab";
        
        private readonly int SwingAnimHash = Animator.StringToHash("Swing");
        private readonly int SwingLevelAnimHash = Animator.StringToHash("SwingLevel");
        private readonly int ChargeLevelAnimHash = Animator.StringToHash("ChargeLevel");
        
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new();
        private List<IHitAble> _hitAbles = new();
        //private List<LineRenderer> _lines = new();


        private LayerMask _projectileLayerMask;

        private bool _isCharging = false;
        private bool _isCharged = false;
        private float _chargingTime;
        private float _currentSwingRad, _currentSwingCentralAngle;

        private SoundManager _soundManager;
        private GlobalData _globalData;
        
        private Coroutine _swingAfterCoroutine;
        
        
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            CheckSwingAreaMesh();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            _soundManager = SystemManager.Instance.SoundManager;

            _projectileLayerMask = int.MaxValue;//LayerMask.GetMask("Player");
            
            SystemManager.Instance.ResourceManager.CacheAsset(HitLinePath);
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
        }


        public override void InitializeState()
        {
            base.InitializeState();

            _isCharged = false;
            _isCharging = false;
            
            if (_ownerEntity.PreviousStateIndex != (int) Player.States.Dodge)
            {
                _chargingTime = 0;
                _moveSpeed = _ownerEntity.StatComponent.GetStat(PlayerStats.MovementSpd);
            }
        }

        public override void ClearState()
        {
            base.ClearState();
            _soundManager.StopSFX(_soundManager.SoundData.ChargeSFX);
            _projectiles.Clear();
            _ownerEntity.ChargingEffectStop();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash,0);
        }

        public override void UpdateState()
        {
            _chargingTime += Time.deltaTime;
            GetChargeLevel();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            
            GetProjectiles();
        }
        

        protected override void OnSwing(bool isOn)
        {
            if (isOn)
            {
                return;
            }

            var mask = _ownerEntity.ProjectileShooter.BounceMask;
            var rigidDmg = _ownerEntity.StatComponent.GetDmg(_isCharged ? PlayerStats.ChargeRigidDmg2 : PlayerStats.ChargeRigidDmg1);
            
            var shootSpd = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeShootSpd).Value;
            var bounce = (int) _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeBounceCount).Value;
            
            var projectileDamage = (int)_ownerEntity.StatComponent.GetDmg(PlayerStats.ChargeProjectileDmg);
            var enemyProjectileDamage = (int) _ownerEntity.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg);

            var pierce = (int) _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeAtkPierce).Value;
            bool isPierce = _isCharged && pierce >= 1;
            
            int hitCount = 0;
            int ballHitCount = 0;
            int enemyHitCount = 0;
            int stunEnemyCount = 0;

            foreach (var hitAble in _hitAbles)
            {
                if (hitAble.IsClearTarget)
                {
                    if (!_isCharged)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitAble.Position);
                    }

                    enemyHitCount++;

                    if (!_isCharged && hitAble.IsDead && hitAble is Enemy enemy)
                    {
                        enemy.ResetBounceCount(bounce);
                        enemy.ResetProjectileDamage(enemyProjectileDamage);
                        enemy.ProjectileHit(GetNewProjectileDir(enemy), shootSpd, mask, ProjectileOwner.Player,
                            _ownerEntity.StatComponent.GetStat(PlayerStats.ReflectCorrection), isPierce);
                    
                        stunEnemyCount++;
                    }
                }
                
                var hitDir = (hitAble.Position - (Vector2) _ownerEntity.transform.position).normalized;
                
                hitAble.Hit(hitDir, rigidDmg, _isCharged ? AttackType.PowerSwing : AttackType.Swing);
                hitCount++;
            }

            if (_isCharged)
            {
                foreach (var projectile in _projectiles)
                {
                    projectile.ResetBounceCount(bounce);
                    projectile.ResetProjectileDamage(projectile is Enemy ? enemyProjectileDamage : projectileDamage);
                    projectile.ProjectileHit(GetNewProjectileDir(projectile), shootSpd, mask, ProjectileOwner.Player,
                        _ownerEntity.StatComponent.GetStat(PlayerStats.ReflectCorrection), isPierce);

                    if (projectile is not IHitAble)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, projectile.Position);
                    }

                    hitCount++;
                    ballHitCount++;
                }
            }

            _ownerEntity.StatComponent.GetStatus(PlayerStats.SwingCooldown).SetStatus(0);

            if (hitCount > 0)
            {
                var aimDir = ((Vector2) _ownerEntity.transform.position - _ownerEntity.AimPosition).normalized;
                _ownerEntity.AttackImpulseSource.GenerateImpulse(aimDir * _ownerEntity.AttackImpulseForce);

                SystemManager.Instance.EventManager.InvokeEvent(EventType.OnSwingHit, null);
                _soundManager.PlayOneShot(_soundManager.SoundData.PlayerSwingHitSFX);

                if (_isCharged)
                {
                    SystemManager.Instance.ResourceManager.EmitParticle(SwingChargedBatHitPath, _ownerEntity.Position);
                }
            }

            if (ballHitCount > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.BallAttackSFX);
                
                SystemManager.Instance.EventManager.InvokeEvent(EventType.OnParry, null);
            }

            if(ballHitCount == 0 && enemyHitCount == 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.SwingSFX);
            }

            if (stunEnemyCount > 0)
            {
                SystemManager.Instance.EventManager.InvokeEvent(EventType.OnAttackStunEnemy, null);
            }
            
            SystemManager.Instance.EventManager.InvokeEvent(EventType.OnSwing, null);

            SetSwingAnimation();

            _ownerEntity.ChangeState(Player.States.Move);
        }

        private void SetSwingAnimation()
        {
            _ownerEntity.Animator.SetInteger(SwingLevelAnimHash, _isCharged ? 1 : 0);
            _ownerEntity.Animator.SetTrigger(SwingAnimHash);
            
            _ownerEntity.LockAim = true; 
            if(_swingAfterCoroutine != null)
                _ownerEntity.StopCoroutine(_swingAfterCoroutine);
            _swingAfterCoroutine = _ownerEntity.StartCoroutine(Util.UnityUtil.WaitForFunc(() => { _ownerEntity.LockAim = false; }, _isCharged ? 0.7f : 0.3f));
        }

        
        private void GetChargeLevel()
        {
            if (!_isCharging && _globalData.ChargeAtkDelay < _chargingTime)
            {
                _isCharging = true;
                _moveSpeed = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeMovementSpd);
                
                _ownerEntity.ChargingEffectPlay();
                _soundManager.PlaySFX(_soundManager.SoundData.ChargeSFX);
                
                CheckSwingAreaMesh();
                _ownerEntity.SwingAreaMeshRenderer.enabled = true;
                
                return;
            }
            
            var chargeTime = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeTime).Value;
            var chargeLevel = _chargingTime / chargeTime;
            chargeLevel *= chargeLevel * chargeLevel;
            chargeLevel = Mathf.Clamp01(chargeLevel);
            
            _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash, chargeLevel);
            _ownerEntity.SwingAreaMeshRenderer.material.color = Color.Lerp(Color.clear, _globalData.SwingAreaColor, chargeLevel);
            
            
            if (!_isCharged && chargeTime < _chargingTime)
            {
                _isCharged = true;
                _soundManager.StopSFX(_soundManager.SoundData.ChargeSFX);
                _ownerEntity.FullChargingEffectPlay();
                _ownerEntity.ChargingEffectStop();
                _soundManager.PlayOneShot(_soundManager.SoundData.ChargeEndSFX);
                
                SystemManager.Instance.EventManager.InvokeEvent(EventType.OnCharged, null);
            }
        }

        private void GetProjectiles()
        {
            Transform eye = _ownerEntity.SwingAreaMeshRenderer.transform;

            _projectiles.Clear();
            _hitAbles.Clear();
            
            float swingRad = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingRad);
            float swingCentralAngle = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingCentralAngle);
            
            ProjectileManager.Instance.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _projectiles, _projectileLayerMask);
            HitAbleManager.Instance.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _hitAbles);
        }


        private Vector2 GetNewProjectileDir(IProjectile projectile)
        {
            Vector2 ownerPos = _ownerEntity.transform.position;
            
            if ((projectile.Position - ownerPos).sqrMagnitude > (_ownerEntity.AimPosition - ownerPos).sqrMagnitude)
            {
                return (_ownerEntity.AimPosition -ownerPos).normalized;
            }
            
            return (_ownerEntity.AimPosition - projectile.Position).normalized;
        }

        private void CheckSwingAreaMesh()
        {
            float swingRad = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingRad);
            float swingAngle = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingCentralAngle);

            if (_currentSwingRad != swingRad || _currentSwingCentralAngle != swingAngle)
            {
                _ownerEntity.SwingAreaMeshFilter.mesh =
                    CreateSwingAreaMesh(swingRad, swingAngle);

                _currentSwingRad = swingRad;
                _currentSwingCentralAngle = swingAngle;
            }
        }


        private Mesh CreateSwingAreaMesh(float radius, float angle)
        {
            Mesh mesh = new Mesh();

            var vertices = new Vector3[Segments + 2];
            var indices = new int[Segments * 3];

            float angleRad = angle * Mathf.Deg2Rad;
            float angleStep = angleRad / Segments;
            float currentAngle = -angleRad / 2f;

            vertices[0] = Vector3.zero;
            for (int i = 0; i <= Segments; i++)
            {
                vertices[i + 1] = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * radius;
                currentAngle += angleStep;
            }

            for (int i = 0; i < Segments; i++)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();

            return mesh;
        }
        
    }
}