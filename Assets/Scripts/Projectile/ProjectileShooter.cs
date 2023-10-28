using System.Collections;
using System.Collections.Generic;
using QT;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public enum AimTypes
    {
        World,
        Target,
        MoveDirection,
    }

    public class ProjectileShooter : MonoBehaviour
    {
        public virtual LayerMask BounceMask => LayerMask.GetMask("Wall", "HardCollider", "ProjectileCollider");
        public virtual ProjectileOwner Owner => ProjectileOwner.Player;

        public Transform ShootPoint;
        private Transform _targetTransform;

        private StatComponent _statComponent;

        public void Init(StatComponent statComponent)
        {
            _statComponent = statComponent;
        }
        
        public void SetTarget(Transform target)
        {
            _targetTransform = target;
        }

        public virtual void Shoot(int shootDataId, AimTypes aimType, ProjectileOwner owner)
        {
            var shootData = SystemManager.Instance.DataManager.GetDataBase<ShootGameDataBase>().GetData(shootDataId);

            if (shootData == null)
            {
                return;
            }

            int bounceStat = 0;
            var properties = ProjectileProperties.None;
            if (_statComponent != null)
            {
                bounceStat = (int)_statComponent.GetStat(PlayerStats.ChargeBounceCount).Value;
                if (_statComponent.GetStat(PlayerStats.ProjectilePierce).Value >= 1)
                {
                    properties |= ProjectileProperties.Pierce;
                }
            }
            
            foreach (var shoot in shootData)
            {
                var dir = GetDirection(shoot.ShootAngle, aimType);

                ShootProjectile(shoot.ProjectileDataId, dir, shoot.InitalSpd, shoot.MaxBounceCount + bounceStat, owner, properties);
            }
        }

        public virtual async void ShootProjectile(int projectileDataId, Vector2 dir, float speed, int bounceCount, ProjectileOwner owner, ProjectileProperties properties, float releaseDelay = 0)
        {
            var projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                .GetData(projectileDataId);
            if (projectileData == null)
            {
                return;
            }

            var projectile =
                await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(projectileData.PrefabPath);
            projectile.transform.position = ShootPoint.position;

            projectile.Init(projectileData, dir, speed, bounceCount, BounceMask, owner, properties, _targetTransform, releaseDelay, projectileData.PrefabPath);
        }

        protected virtual Vector2 GetDirection(float angle, AimTypes aimType)
        {
            switch (aimType)
            {
                case AimTypes.Target:
                    var dir = _targetTransform.position - transform.position;
                    angle += Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    break;

                case AimTypes.MoveDirection:
                    angle += ShootPoint.rotation.eulerAngles.z;
                    break;
            }

            angle *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}