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
        private const string SwingBatBallHitSoundPath = "Assets/Sound/QT/Assets/Player_Ball_Attack_2.wav";
        private const string SwingBatEnemyHitSoundPath = "Assets/Sound/QT/Assets/Player_Swing_Hit.wav";
        private const string SwingMissSoundPath = "Assets/Sound/QT/Assets/Player_Swing_Swing_";
        private const string ChargeSoundPath = "Assets/Sound/QT/Assets/Player_Charge.wav";
        private const string ChargeEndSoundPath = "Assets/Sound/QT/Assets/GameCharge_End_";
        private const int Segments = 32;
        private const int MaxLineCount = 10;

        private List<IProjectile> _projectiles = new();
        private List<Enemy> _enemyInRange = new ();
        private List<LineRenderer> _lines = new();


        private LayerMask _projectileLayerMask;

        private bool _isCharged = false;
        private float _chargingTime;
        private float _currentSwingRad, _currentSwingCentralAngle;


        private GlobalDataSystem _globalDataSystem;
        private SoundManager _soundManager;

        
        public PlayerSwingState(IFSMEntity owner) : base(owner)
        {
            CheckSwingAreaMesh();
            
            _ownerEntity.SwingAreaMeshRenderer.enabled = false;
            
            _globalDataSystem = SystemManager.Instance.GetSystem<GlobalDataSystem>();

            _soundManager = SystemManager.Instance.SoundManager;
            
            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                _projectileLayerMask = LayerMask.GetMask("Wall");
                
            }
            else
            {
                _projectileLayerMask = LayerMask.GetMask("Enemy");
            }
            
            SystemManager.Instance.ResourceManager.CacheAsset(HitLinePath);
        }


        public override void InitializeState()
        {
            base.InitializeState();

            CheckSwingAreaMesh();
            SetLineObjects();

            _ownerEntity.SwingAreaMeshRenderer.enabled = true;

            Debug.LogError((Player.States)_ownerEntity.PreviousStateIndex);
            if (_ownerEntity.PreviousStateIndex != (int) Player.States.Dodge)
            {
                _isCharged = false;
                _chargingTime = 0;
            }
        }

        public override void ClearState()
        {
            base.ClearState();
            _soundManager.ControlAudioStop(ChargeSoundPath);
            _projectiles.Clear();
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
            int hitCount = 0;
            int ballHitCount = 0;
            int enemyHitCount = 0;

            if (_globalDataSystem.GlobalData.IsPlayerParrying)
            {
                bounce = 0;
            }

            foreach (var projectile in _projectiles)
            {
                projectile.ResetBounceCount(bounce);
                projectile.ProjectileHit(GetNewProjectileDir(projectile), power, mask, ProjectileOwner.Player,
                    _ownerEntity.GetStat(PlayerStats.ReflectCorrection));
                SystemManager.Instance.ResourceManager.EmitParticle(SwingProjectileHitPath, projectile.Position);
                hitCount++;
                ballHitCount++;
            }

            
            var damage = _ownerEntity.GetStat(_isCharged ? PlayerStats.ChargeRigidDmg2 : PlayerStats.ChargeRigidDmg1).Value;
            foreach (var hitEnemy in _enemyInRange)
            {
                hitEnemy.Hit(((Vector2) _ownerEntity.transform.position - hitEnemy.Position).normalized, damage);
                SystemManager.Instance.ResourceManager.EmitParticle(SwingBatHitPath, hitEnemy.Position);
                hitCount++;
                enemyHitCount++;
            }

            _ownerEntity.swingSlashEffectPlay();
            _ownerEntity.PlayBatAnimation();
            _ownerEntity.ChangeState(Player.States.Move);

            _ownerEntity.GetStatus(PlayerStats.SwingCooldown).SetStatus(0);

            if (hitCount > 0)
            {
                _ownerEntity.AttackImpulseSource.GenerateImpulse(_ownerEntity.LookDir * 0.5f);
            }

            if (ballHitCount > 0)
            {
                _soundManager.PlayOneShot(SwingBatBallHitSoundPath);
            }

            if (enemyHitCount > 0)
            {
                _soundManager.PlayOneShot(SwingBatEnemyHitSoundPath);
            }
            if(ballHitCount == 0 && enemyHitCount == 0)
            {
                _soundManager.RandomSoundOneShot(SwingMissSoundPath,4);
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
                
                _ownerEntity.FullChargingEffectPlay();
                _soundManager.RandomSoundOneShot(ChargeEndSoundPath,3);
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
            var bounceCount = (int) _ownerEntity.GetStat(_isCharged ? PlayerStats.ChargeBounceCount2 : PlayerStats.ChargeBounceCount1).Value;;
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