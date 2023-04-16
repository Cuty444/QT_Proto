using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Input;

namespace QT.Player
{
    [FSMState((int)Player.States.Swing)]
    public class PlayerSwingState : PlayerMoveState
    {
        private const string HitLinePath = "Prefabs/HitLine.prefab";
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new ();
        private List<LineRenderer> _lines = new ();

        private float _chargingStartTime;
        private int _chargeLevel;
        
        private Vector2 _mousePos;
        private float _mouseDistance;
        
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.SwingAreaMeshFilter.mesh = CreateSwingAreaMesh(_ownerEntity.SwingRadius, _ownerEntity.SwingCentralAngle);
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            
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

        public override void UpdateState()
        {
            base.UpdateState();

            SetDirection();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            
            GetChargeLevel();
            GetProjectiles();
            
            SetLines();
        }
        
        public override void ClearState()
        {
            _projectiles.Clear();
            foreach (var line in _lines)
            {
                SystemManager.Instance.ResourceManager.ReleaseObject(line);
            }
            _lines.Clear();
            
            SystemManager.Instance.GetSystem<InputSystem>().OnKeyUpAttackEvent.RemoveListener(OnAtkEnd);
        }

        protected override void MoveDirection(Vector2 direction)
        {
            _moveDirection = direction.normalized;
        }
        
        private void OnAtkEnd()
        {
            var mask = _ownerEntity.ProjectileShooter.BounceMask;
            var power = _ownerEntity.ChargeShootSpd[_chargeLevel];
            var bounce = (int)_ownerEntity.ChargeBounceCount[_chargeLevel];
            foreach (var projectile in _projectiles)
            {
                projectile.ResetBounceCount(bounce);
                projectile.Hit(GetNewProjectileDir(projectile), power, mask);
            }
            
            _ownerEntity.PlayBatAnimation();
            
            _ownerEntity.ChangeState(Player.States.Idle);
        }

        private async void SetLineObjects()
        {
            for (int i = 0; i < MaxLineCount; i++)
            {
                var line = await SystemManager.Instance.ResourceManager.GetFromPool<LineRenderer>(HitLinePath);
                _lines.Add(line);
            }
        }
        
        private void SetDirection()
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mouseDistance = Vector2.Distance(_mousePos, _ownerEntity.transform.position);
            
            var angle = Util.Math.GetDegree(_ownerEntity.transform.position, _mousePos);
            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        private void GetChargeLevel()
        {
            for (int level = 0; level < _ownerEntity.ChargeTimes.Length; level++)
            {
                if (_ownerEntity.ChargeTimes[level] > Time.time - _chargingStartTime)
                {
                    _chargeLevel = level;
                    break;
                }
            }
        }
        
        private void GetProjectiles()
        {
            Vector2 playerPos = _ownerEntity.EyeTransform.position;
            Vector2 playerDir = _ownerEntity.EyeTransform.right;
            
            _projectiles.Clear();
            SystemManager.Instance.ProjectileManager.GetInRange(playerPos, _ownerEntity.SwingRadius, ref _projectiles);

            for (int i = 0; i < _projectiles.Count; i++)
            {
                var dot = Vector2.Dot((_projectiles[i].Position - playerPos).normalized, playerDir);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle > _ownerEntity.SwingCentralAngle * 0.5f)
                {
                    _projectiles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void SetLines()
        {
            var bounceCount = (int)_ownerEntity.ChargeBounceCount[_chargeLevel];
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
            var dir = (_mousePos - projectile.Position);
            
            if(dir.magnitude < _mouseDistance)
            {
                return (_mousePos - (Vector2)_ownerEntity.transform.position).normalized;
            }
            return dir.normalized;
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