using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;
using QT.Sound;

namespace QT.InGame
{
    [FSMState((int) Player.States.Swing)]
    public class PlayerSwingState : PlayerMoveState
    {
        private const string HitLinePath = "Prefabs/HitLine.prefab";
        private const string SwingProjectileHitPath = "Effect/Prefabs/FX_M_Ball_Hit_Boom.prefab";
        
        private const string SwingBatHitPath = "Effect/Prefabs/FX_M_Small_Hit.prefab";
        private const string SwingChargedBatHitPath = "Effect/Prefabs/FX_M_Bat_Hit_Boom.prefab";
        
        private const string SwingChargedHitPath = "Effect/Prefabs/FullScreen/FX_FullScreen.prefab";

        private static readonly int SwingLevelShaderHash = Shader.PropertyToID("_Progress");
        
        private readonly int SwingAnimHash = Animator.StringToHash("Swing");
        private readonly int SwingLevelAnimHash = Animator.StringToHash("SwingLevel");
        private readonly int ChargeLevelAnimHash = Animator.StringToHash("ChargeLevel");
        
        private const int Segments = 16;
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
                
                SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnSwingStart, null);
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
            
            var properties = ProjectileProperties.None;

            Transform projectileTarget = null;
            if (_ownerEntity.StatComponent.GetStat(PlayerStats.ProjectilePierce).Value >= 1)
            {
                properties |= ProjectileProperties.Pierce;
            }
            
            if (_ownerEntity.StatComponent.GetStat(PlayerStats.ProjectileGuide).Value >= 1)
            {
                properties |= ProjectileProperties.Guided;
                projectileTarget = GetProjectileTarget();
            }
            
            if (_ownerEntity.StatComponent.GetStat(PlayerStats.ProjectileExplosion).Value >= 1)
            {
                properties |= ProjectileProperties.Explosion;
            }
            
            

            int hitCount = 0;
            int ballHitCount = 0;
            int enemyHitCount = 0;
            int stunEnemyCount = 0;

            var hitData = new ProjectileHitData(Vector2.zero, shootSpd, mask, ProjectileOwner.Player, properties,
                projectileTarget);
            
            foreach (var hitAble in _hitAbles)
            {
                if (hitAble == _ownerEntity)
                {
                    continue;
                }
                if (hitAble.IsClearTarget)
                {
                    SystemManager.Instance.ResourceManager.EmitParticle(_isCharged ? SwingChargedBatHitPath : SwingBatHitPath, hitAble.Position);

                    enemyHitCount++;

                    if (!_isCharged && hitAble.IsDead && hitAble is IProjectile projectile)
                    {
                        projectile.ResetBounceCount(bounce);
                        projectile.ResetProjectileDamage(enemyProjectileDamage);

                        hitData.Dir = GetNewProjectileDir(projectile);
                        projectile.ProjectileHit(hitData);
                    
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
                    projectile.ResetProjectileDamage(projectile is IEnemy ? enemyProjectileDamage : projectileDamage);
                    
                    hitData.Dir = GetNewProjectileDir(projectile);
                    projectile.ProjectileHit(hitData);

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

                SystemManager.Instance.EventManager.InvokeEvent(_isCharged ? TriggerTypes.OnChargedSwingHit : TriggerTypes.OnSwingHit, null);
                
                _soundManager.PlayOneShot(_soundManager.SoundData.PlayerSwingHitSFX);

                if (_isCharged)
                {
                    SystemManager.Instance.ResourceManager.EmitParticle(SwingChargedHitPath, _ownerEntity.Position);
                }
            }

            if (ballHitCount > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.BallAttackSFX);
                
                SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnParry, null);
            }

            if(ballHitCount == 0 && enemyHitCount == 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.SwingSFX);
            }

            if (stunEnemyCount > 0)
            {
                SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnAttackStunEnemy, null);
            }
            
            SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnSwing, null);

            SetSwingAnimation();

            _ownerEntity.ChangeState(Player.States.Move);
        }

        private Transform GetProjectileTarget()
        {
            var origin = _ownerEntity.AimPosition;
            var allHitAble = HitAbleManager.Instance.GetAllHitAble();
            var minDist = float.MaxValue;
            IHitAble minHitable = null;
            
            foreach (var hitable in allHitAble)
            {
                if (hitable.IsClearTarget && !hitable.IsDead)
                {
                    var dist = (hitable.Position - origin).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minHitable = hitable;
                    }
                }
            }

            if (minHitable == null)
            {
                return null;
            }

            return ((MonoBehaviour) minHitable).transform;
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
            chargeLevel *= chargeLevel * chargeLevel * chargeLevel * chargeLevel;
            chargeLevel = Mathf.Clamp01(chargeLevel);
            
            _ownerEntity.Animator.SetFloat(ChargeLevelAnimHash, chargeLevel);
            _ownerEntity.SwingAreaMeshRenderer.material.SetFloat(SwingLevelShaderHash, chargeLevel);
            
            
            if (!_isCharged && chargeTime < _chargingTime)
            {
                _isCharged = true;
                _soundManager.StopSFX(_soundManager.SoundData.ChargeSFX);
                _ownerEntity.FullChargingEffectPlay();
                _ownerEntity.ChargingEffectStop();
                _soundManager.PlayOneShot(_soundManager.SoundData.ChargeEndSFX);
                
                SystemManager.Instance.EventManager.InvokeEvent(TriggerTypes.OnCharged, null);
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
            var uvs = new Vector2[Segments + 2];
            var indices = new int[Segments * 3];

            float angleRad = angle * Mathf.Deg2Rad;
            float angleStep = angleRad / Segments;
            float currentAngle = -angleRad / 2f;

            uvs[0] = new Vector2(0.5f, 0.5f);
            vertices[0] = Vector3.zero;

            for (int i = 0; i <= Segments; i++)
            {
                var pos = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
                vertices[i + 1] = pos * radius;
                uvs[i + 1] = new Vector2(0.5f + pos.x * 0.5f, 0.5f + pos.y * 0.5f);

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
            mesh.uv = uvs;
            mesh.RecalculateBounds();

            return mesh;
        }
        
    }
}