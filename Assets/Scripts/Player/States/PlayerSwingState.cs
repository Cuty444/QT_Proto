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
        //private const string SwingNormalProjectileHitPath = "Effect/Prefabs/FX_Ball_Normal_Attack.prefab";
        private const string SwingBatHitPath = "Effect/Prefabs/FX_M_Bat_Hit_Boom.prefab";
        
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new();
        private List<IHitAble> _hitAbles = new();
        private List<LineRenderer> _lines = new();


        private LayerMask _projectileLayerMask;

        private bool _isCharging = false;
        private bool _isCharged = false;
        private float _chargingTime;
        private float _currentSwingRad, _currentSwingCentralAngle;

        private SoundManager _soundManager;
        private GlobalData _globalData;
        
        
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
            SetLineObjects();

            _isCharged = false;
            _isCharging = false;
            
            if (_ownerEntity.PreviousStateIndex != (int) Player.States.Dodge)
            {
                _chargingTime = 0;
                _moveSpeed = _ownerEntity.StatComponent.GetStat(PlayerStats.MovementSpd);

                _soundManager.PlaySFX(_soundManager.SoundData.ChargeSFX);
            }
        }

        public override void ClearState()
        {
            base.ClearState();
            _soundManager.StopSFX(_soundManager.SoundData.ChargeSFX);
            _projectiles.Clear();
            _ownerEntity.ChargingEffectStop();
            foreach (var line in _lines)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(HitLinePath, line);
            }

            _lines.Clear();
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
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

            SetLines();
        }
        

        protected override void OnSwing(bool isOn)
        {
            if (isOn)
            {
                return;
            }

            var mask = _ownerEntity.ProjectileShooter.BounceMask;
            var rigidDmg = _ownerEntity.StatComponent.GetDmg(_isCharged ? PlayerStats.ChargeRigidDmg2 : PlayerStats.ChargeRigidDmg1);
            var shootSpd = _ownerEntity.StatComponent.GetStat(_isCharged ? PlayerStats.ChargeShootSpd2 : PlayerStats.ChargeShootSpd1).Value;
            
            var bounce = (int) _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeBounceCount).Value;
            
            var projectileDamage = (int)_ownerEntity.StatComponent.GetDmg(_isCharged ? PlayerStats.ChargeProjectileDmg2 : PlayerStats.ChargeProjectileDmg1);
            var enemyProjectileDamage = (int) _ownerEntity.StatComponent.GetDmg(_isCharged ? PlayerStats.EnemyProjectileDmg2 : PlayerStats.EnemyProjectileDmg1);
            
            var powerBounce = (int) _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeBounceCount).Value;
            var powerEnemyProjectileDamage = (int) _ownerEntity.StatComponent.GetDmg(PlayerStats.EnemyProjectileDmg2);
            var powerShootSpd = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeShootSpd2).Value;
            
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
                    SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitAble.Position);
                    enemyHitCount++;

                    if (!_isCharged && hitAble.IsDead && hitAble is Enemy enemy)
                    {
                        enemy.ResetBounceCount(powerBounce);
                        enemy.ResetProjectileDamage(powerEnemyProjectileDamage);
                        enemy.ProjectileHit(GetNewProjectileDir(enemy), powerShootSpd, mask, ProjectileOwner.Player,
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

                    //if (_isCharged) // TODO : 차지에 따른 이펙트 변경시 코드
                    //{
                        SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath,
                            projectile.Position);
                    //}
                    //else
                    //{
                    //    SystemManager.Instance.ResourceManager.EmitParticle(SwingNormalProjectileHitPath,
                    //        projectile.Position);
                    //}

                    hitCount++;
                    ballHitCount++;
                }
            }


            _ownerEntity.PlayBatAnimation();
            _ownerEntity.ChangeState(Player.States.Move);

            _ownerEntity.StatComponent.GetStatus(PlayerStats.SwingCooldown).SetStatus(0);

            if (hitCount > 0)
            {
                var aimDir = ((Vector2) _ownerEntity.transform.position - _ownerEntity.AimPosition).normalized;
                _ownerEntity.AttackImpulseSource.GenerateImpulse(aimDir * _ownerEntity.AttackImpulseForce);
                
                SystemManager.Instance.PlayerManager.OnSwingHit?.Invoke();
                _soundManager.PlayOneShot(_soundManager.SoundData.PlayerSwingHitSFX);
            }

            if (ballHitCount > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.BallAttackSFX);
                
                SystemManager.Instance.PlayerManager.OnParry?.Invoke();
            }

            if(ballHitCount == 0 && enemyHitCount == 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.SwingSFX);
            }

            if (stunEnemyCount > 0)
            {
                SystemManager.Instance.PlayerManager.OnAttackStunEnemy?.Invoke();
            }
            
            SystemManager.Instance.PlayerManager.OnSwing?.Invoke();
        }

        private async void SetLineObjects()
        {
            for (int i = 0; i < MaxLineCount; i++)
            {
                var line = await SystemManager.Instance.ResourceManager.GetFromPool<LineRenderer>(HitLinePath);
                line.positionCount = 0;
                _lines.Add(line);
            }
        }


        private void GetChargeLevel()
        {
            if (!_isCharging && _globalData.ChargeAtkDelay < _chargingTime)
            {
                _isCharging = true;
                _moveSpeed = _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeMovementSpd);
                _ownerEntity.ChargingEffectPlay();
                
                CheckSwingAreaMesh();
                _ownerEntity.SwingAreaMeshRenderer.enabled = true;
            }
            
            if (!_isCharged && _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeTime).Value < _chargingTime)
            {
                _isCharged = true;
                _soundManager.StopSFX(_soundManager.SoundData.ChargeSFX);
                _ownerEntity.FullChargingEffectPlay();
                _ownerEntity.ChargingEffectStop();
                _soundManager.PlayOneShot(_soundManager.SoundData.ChargeEndSFX);
            }
        }

        private void GetProjectiles()
        {
            Transform eye = _ownerEntity.EyeTransform;

            _projectiles.Clear();
            _hitAbles.Clear();
            
            float swingRad = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingRad);
            float swingCentralAngle = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingCentralAngle);
            
            ProjectileManager.Instance.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _projectiles, _projectileLayerMask);
            HitAbleManager.Instance.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _hitAbles);
        }

        private void SetLines()
        {
            var bounceCount = (int) _ownerEntity.StatComponent.GetStat(PlayerStats.ChargeBounceCount).Value;;

            for (int i = 0; i < _lines.Count; i++)
            {
                if (_projectiles.Count > i && _projectiles[i] is not Enemy)
                {  
                    SetLine(_projectiles[i], _lines[i], bounceCount);
                }
                else
                {
                    _lines[i].positionCount = 0;
                }
            }
        }

        private void SetLine(IProjectile projectile, LineRenderer lineRenderer, int bounceCount)
        {
            var dir = GetNewProjectileDir(projectile);
            var mask = _ownerEntity.ProjectileShooter.BounceMask;
            float reflectCorrection = _ownerEntity.StatComponent.GetStat(PlayerStats.ReflectCorrection);

            lineRenderer.enabled = true;
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = bounceCount + 2;

            lineRenderer.SetPosition(0, projectile.Position);
            var hit2 = Physics2D.CircleCast(lineRenderer.GetPosition((0)), 0.5f, dir, Mathf.Infinity, mask);

            if (hit2.collider)
            {
                lineRenderer.SetPosition(1, hit2.point + (hit2.normal * 0.5f));
            }
            for (int i = 1; i < bounceCount; i++)
            {
                var hit = Physics2D.CircleCast(lineRenderer.GetPosition((i - 1)), 0.5f, dir, Mathf.Infinity, mask);

                if (hit.collider == null)
                {
                    lineRenderer.positionCount = i + 1;
                    break;
                }

                lineRenderer.SetPosition(i, hit.point + (hit.normal * 0.5f));
                dir = Vector2.Reflect(dir, hit.normal);

                if (reflectCorrection != 0)
                {
                    var targetDir = ((Vector2) _ownerEntity.transform.position - hit.point).normalized;
                    dir = Vector3.RotateTowards(dir, targetDir, reflectCorrection * Mathf.Deg2Rad, 0);
                }
            }
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