using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
{
    [FSMState((int) Player.States.Swing)]
    public class PlayerSwingState : PlayerMoveState
    {
        private const string HitLinePath = "Prefabs/HitLine.prefab";
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new();
        private List<Enemy.Enemy> _enemyInRange = new List<Enemy.Enemy>();
        private List<LineRenderer> _lines = new();

        private float _chargingStartTime;
        private int _chargeLevel;

        private LayerMask _projectileLayerMask;


        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.SwingAreaMeshFilter.mesh =
                CreateSwingAreaMesh(_ownerEntity.SwingRadius, _ownerEntity.SwingCentralAngle);
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            _projectileLayerMask = LayerMask.GetMask("Enemy", "ProjectileDelayed");
            SystemManager.Instance.ResourceManager.CacheAsset(HitLinePath);
        }


        public override void InitializeState()
        {
            base.InitializeState();

            _chargingStartTime = Time.time;
            _chargeLevel = 0;

            SystemManager.Instance.GetSystem<InputSystem>().OnKeyUpAttackEvent.AddListener(OnAtkEnd);

            SetLineObjects();
        }

        public override void ClearState()
        {
            _projectiles.Clear();
            foreach (var line in _lines)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(HitLinePath, line);
            }

            _lines.Clear();

            SystemManager.Instance.GetSystem<InputSystem>().OnKeyUpAttackEvent.RemoveListener(OnAtkEnd);
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            GetChargeLevel();
            GetProjectiles();

            SetLines();
        }

        private void OnAtkEnd()
        {
            var mask = _ownerEntity.ProjectileShooter.BounceMask;
            var power = _ownerEntity.ChargeShootSpd[_chargeLevel];
            var bounce = (int) _ownerEntity.ChargeBounceCount[_chargeLevel];
            var damage = _ownerEntity.ChargeProjectileDmg[_chargeLevel];
            foreach (var projectile in _projectiles)
            {
                projectile.ResetBounceCount(bounce);
                projectile.ProjectileHit(GetNewProjectileDir(projectile), power,damage, mask);
            }

            foreach (var hitEnemy in _enemyInRange)
            {
                hitEnemy.Hit((hitEnemy.Position - (Vector2)_ownerEntity.transform.position).normalized,_ownerEntity.ChargeRigidDmg[_chargeLevel]);
            }

            _ownerEntity.PlayBatAnimation();
            _ownerEntity.ChangeState(Player.States.Idle);
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
        }

        private void GetProjectiles()
        {
            Transform eye = _ownerEntity.EyeTransform;

            _projectiles.Clear();
            _enemyInRange.Clear();
            SystemManager.Instance.ProjectileManager.GetInRange(eye.position, _ownerEntity.SwingRadius,
                _ownerEntity.SwingCentralAngle * 0.5f, eye.right, ref _projectiles, _projectileLayerMask);
            GetInEnemyRange(eye.position, _ownerEntity.SwingRadius, _ownerEntity.SwingCentralAngle * 0.5f, eye.right,
                ref _enemyInRange);
        }

        private void SetLines()
        {
            var bounceCount = (int) _ownerEntity.ChargeBounceCount[_chargeLevel];
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
            lineRenderer.positionCount = bounceCount;

            lineRenderer.SetPosition(0, projectile.Position);
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
            }
        }


        private Vector2 GetNewProjectileDir(IProjectile projectile)
        {
            if (Vector2.Distance(projectile.Position, _ownerEntity.transform.position) >
                Vector2.Distance(_ownerEntity.MousePos, _ownerEntity.transform.position))
            {
                return (_ownerEntity.MousePos - (Vector2) _ownerEntity.transform.position).normalized;
            }

            return (_ownerEntity.MousePos - projectile.Position).normalized;
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
            ref List<Enemy.Enemy> outList)
        {
            foreach (var enemy in _ownerEntity._enemyList)
            {
                if (enemy.CurrentStateIndex > (int) Enemy.Enemy.States.Rigid)
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