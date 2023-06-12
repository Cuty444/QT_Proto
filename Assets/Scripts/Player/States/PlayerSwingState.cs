using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Sound;

namespace QT.InGame
{
    [FSMState((int) Player.States.Swing)]
    public class PlayerSwingState : PlayerMoveState
    {
        private const string HitLinePath = "Prefabs/HitLine.prefab";
        private const string SwingProjectileHitPath = "Effect/Prefabs/FX_Ball_Attack.prefab";
        private const string SwingBatHitPath = "Effect/Prefabs/FX_Bat_Hit.prefab";
        
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new();
        private List<Enemy> _enemyInRange = new ();
        private List<IHitable> _hitableRange = new();
        private List<LineRenderer> _lines = new();


        private LayerMask _projectileLayerMask;

        private bool _isCharged = false;
        private float _chargingTime;
        private float _currentSwingRad, _currentSwingCentralAngle;

        private SoundManager _soundManager;

        
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            CheckSwingAreaMesh();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            _soundManager = SystemManager.Instance.SoundManager;

            _projectileLayerMask = LayerMask.GetMask("Player");
            
            SystemManager.Instance.ResourceManager.CacheAsset(HitLinePath);
        }


        public override void InitializeState()
        {
            base.InitializeState();

            CheckSwingAreaMesh();
            SetLineObjects();

            _ownerEntity.SwingAreaMeshRenderer.enabled = true;

            if (_ownerEntity.PreviousStateIndex != (int) Player.States.Dodge)
            {
                _isCharged = false;
                _chargingTime = 0;
                _soundManager.PlaySFX(_soundManager.SoundData.ChargeSFX);
            }
            if(!_isCharged)
                _ownerEntity.ChargingEffectPlay();
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
            var power = _ownerEntity.GetStat(_isCharged ? PlayerStats.ChargeShootSpd2 : PlayerStats.ChargeShootSpd1).Value;
            var bounce = (int) _ownerEntity.GetStat(_isCharged ? PlayerStats.ChargeBounceCount2 : PlayerStats.ChargeBounceCount1).Value;
            var projectileDamage = (int)_ownerEntity.GetDmg(_isCharged ? PlayerStats.ChargeProjectileDmg2 : PlayerStats.ChargeProjectileDmg1);
            var pierce = (int) _ownerEntity.GetStat(PlayerStats.ChargeAtkPierce).Value;
            bool isPierce = _isCharged && pierce == 1;
            int hitCount = 0;
            int ballHitCount = 0;
            int enemyHitCount = 0;

            foreach (var projectile in _projectiles)
            {
                projectile.ResetBounceCount(bounce);
                projectile.ResetProjectileDamage(projectileDamage);
                projectile.ProjectileHit(GetNewProjectileDir(projectile), power, mask, ProjectileOwner.Player,
                    _ownerEntity.GetStat(PlayerStats.ReflectCorrection),isPierce);
                SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, projectile.Position);
                hitCount++;
                ballHitCount++;
            }

            
            var damage = _ownerEntity.GetDmg(_isCharged ? PlayerStats.ChargeRigidDmg2 : PlayerStats.ChargeRigidDmg1);
            projectileDamage =
                (int) _ownerEntity.GetDmg(_isCharged
                    ? PlayerStats.EnemyProjectileDmg2
                    : PlayerStats.EnemyProjectileDmg1);
            foreach (var hitEnemy in _enemyInRange)
            {
                hitEnemy.Hit(((Vector2) _ownerEntity.transform.position - hitEnemy.Position).normalized, damage,AttackType.Swing);
                hitEnemy.ResetProjectileDamage(projectileDamage);
                hitEnemy.ProjectileHit(GetNewProjectileDir(hitEnemy), power, mask, ProjectileOwner.Player,_ownerEntity.GetStat(PlayerStats.ReflectCorrection),false);
                SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitEnemy.Position);
                hitCount++;
                enemyHitCount++;
            }

            foreach (var hit in _hitableRange)
            {
                hit.Hit(((Vector2) _ownerEntity.transform.position - hit.GetPosition()).normalized,damage,AttackType.Swing);
                hitCount++;
                enemyHitCount++;
            }
            
            _ownerEntity.PlayBatAnimation();
            _ownerEntity.ChangeState(Player.States.Move);

            _ownerEntity.GetStatus(PlayerStats.SwingCooldown).SetStatus(0);

            if (hitCount > 0)
            {
                var aimDir = ((Vector2) _ownerEntity.transform.position - _ownerEntity.AimPosition).normalized;
                _ownerEntity.AttackImpulseSource.GenerateImpulse(aimDir * _ownerEntity.AttackImpulseForce);
            }

            if (ballHitCount > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.BallAttackSFX);
            }

            if (enemyHitCount > 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.PlayerSwingHitSFX);
            }
            if(ballHitCount == 0 && enemyHitCount == 0)
            {
                _soundManager.PlayOneShot(_soundManager.SoundData.SwingSFX);
            }
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
            if (_isCharged)
            {
                return;
            }
            
            if (_ownerEntity.GetStat(PlayerStats.ChargeTime).Value < _chargingTime)
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
            _enemyInRange.Clear();
            _hitableRange.Clear();
            
            float swingRad = _ownerEntity.GetStat(PlayerStats.SwingRad);
            float swingCentralAngle = _ownerEntity.GetStat(PlayerStats.SwingCentralAngle);
            
            SystemManager.Instance.ProjectileManager.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _projectiles, _projectileLayerMask);
            GetInEnemyRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _enemyInRange);
            GetInHitalbeCheck(eye.position,swingRad,swingCentralAngle * 0.5f,eye.right);
        }

        private void SetLines()
        {
            var bounceCount = (int) _ownerEntity.GetStat(_isCharged ? PlayerStats.ChargeBounceCount2 : PlayerStats.ChargeBounceCount1).Value;;
            
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_projectiles.Count > i)
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

            lineRenderer.enabled = true;
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = bounceCount + 2;

            lineRenderer.SetPosition(0, projectile.Position);
            var hit2 = Physics2D.CircleCast(lineRenderer.GetPosition((0)), 0.5f, dir, Mathf.Infinity,
                _ownerEntity.ProjectileShooter.BounceMask);

            if (hit2.collider)
            {
                lineRenderer.SetPosition(1, hit2.point + (hit2.normal * 0.5f));
            }
            for (int i = 1; i < bounceCount; i++)
            {
                var hit = Physics2D.CircleCast(lineRenderer.GetPosition((i - 1)), 0.5f, dir, Mathf.Infinity,
                    _ownerEntity.ProjectileShooter.BounceMask);

                if (hit.collider == null)
                {
                    lineRenderer.positionCount = i + 1;
                    break;
                }

                lineRenderer.SetPosition(i, hit.point + (hit.normal * 0.5f));
                dir = Vector2.Reflect(dir, hit.normal);

                float reflectCorrection = _ownerEntity.GetStat(PlayerStats.ReflectCorrection);
                
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
            float swingRad = _ownerEntity.GetStat(PlayerStats.SwingRad);
            float swingAngle = _ownerEntity.GetStat(PlayerStats.SwingCentralAngle);

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

        private void GetInEnemyRange(Vector2 origin, float range, float angle, Vector2 dir,
            ref List<Enemy> outList)
        { 
            foreach (var hitable in _ownerEntity._hitableList)
            {
                
                if (hitable is not Enemy)
                {
                    continue;
                }
                
                var enemy = (Enemy) hitable;
                
                if (enemy.CurrentStateIndex > (int) Enemy.States.Rigid)
                    continue;
                var checkRange = range + enemy.ColliderRad;
                var targetDir = enemy.Position - origin;

                if (targetDir.sqrMagnitude < checkRange * checkRange)
                {
                    var dot = Vector2.Dot((targetDir).normalized, dir);
                    var degrees = Mathf.Acos(dot) * Mathf.Rad2Deg;

                    if (degrees < angle)
                    {
                        outList.Add(enemy);
                    }
                }
            }
        }

        private void GetInHitalbeCheck(Vector2 origin,float range,float angle,Vector2 dir)
        {
            foreach (var hitable in _ownerEntity._floorAllHit)
            {
                if (hitable is not Enemy)
                {
                    var hitCheckRange = range + 0.5f;
                    var hitTargetDir = hitable.GetPosition() - origin;

                    if (hitTargetDir.sqrMagnitude < hitCheckRange * hitCheckRange)
                    {
                        var dot = Vector2.Dot((hitTargetDir).normalized, dir);
                        var degrees = Mathf.Acos(dot) * Mathf.Rad2Deg;

                        if (degrees < angle)
                        {
                            _hitableRange.Add(hitable);
                        }
                    }
                }
            }
        }
    }
}