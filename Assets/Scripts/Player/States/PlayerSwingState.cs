using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;

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
        private List<LineRenderer> _lines = new();

        private float _chargingStartTime;
        private int _chargeLevel;

        private LayerMask _projectileLayerMask;

        private bool[] _chargingEffectCheck;

        private GlobalDataSystem _globalDataSystem;

        
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.SwingAreaMeshFilter.mesh =
                CreateSwingAreaMesh(_ownerEntity.GetStat(PlayerStats.SwingRad), _ownerEntity.GetStat(PlayerStats.SwingCentralAngle));
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();
            
            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                _projectileLayerMask = LayerMask.GetMask("Wall");
                
            }
            else
            {
                _projectileLayerMask = LayerMask.GetMask("Enemy");
            }
            
            SystemManager.Instance.ResourceManager.CacheAsset(HitLinePath);
            _chargingEffectCheck = new bool[3];
        }


        public override void InitializeState()
        {
            base.InitializeState();

            _chargingStartTime = Time.time;
            _chargeLevel = 0;
            
            SetLineObjects();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = true;
        }

        public override void ClearState()
        {
            base.ClearState();
            
            _projectiles.Clear();
            foreach (var line in _lines)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(HitLinePath, line);
            }

            _lines.Clear();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            GetChargeLevel();
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
            var power = _ownerEntity.ChargeShootSpds[_chargeLevel];
            var bounce = (int) _ownerEntity.ChargeBounceCounts[_chargeLevel];
            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                bounce = 0;
            }
            var damage = _ownerEntity.ChargeProjectileDmgs[_chargeLevel];
            foreach (var projectile in _projectiles)
            {
                projectile.ResetBounceCount(bounce);
                projectile.ProjectileHit(GetNewProjectileDir(projectile), power, mask, _ownerEntity.GetStat(PlayerStats.ReflectCorrection));
                SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, projectile.Position); 

            }

            foreach (var hitEnemy in _enemyInRange)
            {
                hitEnemy.Hit(((Vector2)_ownerEntity.transform.position - hitEnemy.Position).normalized,_ownerEntity.ChargeRigidDmgs[_chargeLevel]);
                SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitEnemy.Position); 
            }

            for (int i = 0; i < _chargingEffectCheck.Length; i++)
            {
                _chargingEffectCheck[i] = false;
            }
            _ownerEntity.swingSlashEffectPlay();
            _ownerEntity.FullChargingEffectStop();
            _ownerEntity.PlayBatAnimation();
            _ownerEntity.ChangeState(Player.States.Move);
            
            _ownerEntity.GetStatus(PlayerStats.SwingCooldown).SetStatus(0);
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
            for (int level = _ownerEntity.ChargeTimes.Length - 1; level >= 0; level--)
            {
                if (_ownerEntity.ChargeTimes[level] < Time.time - _chargingStartTime)
                {
                    _chargeLevel = level + 1;
                    break;
                }

                if (level == 0)
                {
                    _chargeLevel = 0;
                }
            }

            for (int i = 0; i < _chargingEffectCheck.Length; i++)
            {
                if (_chargeLevel > i && !_chargingEffectCheck[i])
                {
                    _ownerEntity.ChargingEffectPlay(i);
                    if (_chargeLevel > 2)
                    {
                        _ownerEntity.FullChargingEffectPlay();
                        _ownerEntity.FullChargingEffectStop();
                    }
                    _chargingEffectCheck[i] = true;
                }
            }
            
        }

        private void GetProjectiles()
        {
            Transform eye = _ownerEntity.EyeTransform;

            _projectiles.Clear();
            _enemyInRange.Clear();
            
            float swingRad = _ownerEntity.GetStat(PlayerStats.SwingRad);
            float swingCentralAngle = _ownerEntity.GetStat(PlayerStats.SwingCentralAngle);
            
            SystemManager.Instance.ProjectileManager.GetInRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _projectiles, _projectileLayerMask);
            GetInEnemyRange(eye.position, swingRad, swingCentralAngle * 0.5f, eye.right, ref _enemyInRange);
        }

        private void SetLines()
        {
            var bounceCount = (int) _ownerEntity.ChargeBounceCounts[_chargeLevel];
            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                bounceCount = 0;
            }
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
            return -_ownerEntity.LookDir;
            // if (Vector2.Distance(projectile.Position, _ownerEntity.transform.position) >
            //     Vector2.Distance(_ownerEntity.MousePos, _ownerEntity.transform.position))
            // {
            //     return (_ownerEntity.MousePos - (Vector2) _ownerEntity.transform.position).normalized;
            // }
            //
            // return (_ownerEntity.MousePos - projectile.Position).normalized;
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
            foreach (var enemy in _ownerEntity._enemyList)
            {
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
    }
}