using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QT.Core;
using UnityEngine;

namespace QT
{    
    public enum ProjectileOwner
    {
        Player,
        Enemy,
        EnemyElite,
        Boss,
        PlayerTeleport,
        PlayerAbsorb,
    }

    [Flags]
    public enum ProjectileProperties
    {
        None = 0,
        
        Pierce = 1 << 1,
        Guided = 1 << 2,
        Explosion = 1 << 3,
    }

    public struct ProjectileHitData
    {
        public Vector2 Dir;
        public float Power;
        public LayerMask BounceMask;
        public ProjectileOwner Owner;
        public ProjectileProperties Properties;
        public Transform Target;

        public ProjectileHitData(Vector2 dir, float power, LayerMask bounceMask, ProjectileOwner owner, ProjectileProperties properties, Transform target = null)
        {
            Dir = dir;
            Power = power;
            BounceMask = bounceMask;
            Owner = owner;
            Properties = properties;

            Target = target;
        }
    }
    
    public interface IProjectile
    {
        public int InstanceId { get; }
        
        public Vector2 Position { get; }
        public float ColliderRad { get; }
        public LayerMask BounceMask { get; }

        public void ProjectileHit(ProjectileHitData data);

        public void ResetBounceCount(int maxBounce);
        public void ResetProjectileDamage(int damage);
    }
    
    public class ProjectileManager : Singleton<ProjectileManager>
    {
        private Dictionary<int, IProjectile> _projectiles = new();

        public void Register(IProjectile projectile)
        {
            _projectiles.Add(projectile.InstanceId, projectile);
        }
        
        public void UnRegister(IProjectile projectile)
        {
            if(_projectiles.ContainsKey(projectile.InstanceId))
                _projectiles.Remove(projectile.InstanceId);
        }

        public void Clear()
        {
            _projectiles.Clear();
        }
        
        public void GetInRange(Vector2 origin, float range, float angle, Vector2 dir, ref List<IProjectile> outList, int layerMask)
        {
            foreach (var projectile in _projectiles.Values)
            {
                if ((projectile.BounceMask & layerMask) != 0)
                {
                    var checkRange = range + projectile.ColliderRad;
                    var targetDir = projectile.Position - origin;
                    
                    if (targetDir.sqrMagnitude < checkRange * checkRange)
                    {
                        var dot = Vector2.Dot((targetDir).normalized, dir);
                        var degrees = Mathf.Acos(dot) * Mathf.Rad2Deg;

                        if (degrees < angle)
                        {
                            outList.Add(projectile);
                        }
                    }
                }
            }
        }
        
        public void GetInRange(Vector2 origin, float range, ref List<IProjectile> outList, LayerMask layerMask, bool ignoreHitAble = false)
        {
            foreach (var projectile in _projectiles.Values)
            {
                if ((projectile.BounceMask & layerMask) != 0)
                {
                    if(ignoreHitAble && projectile is IHitAble)
                        continue;
                    
                    var checkRange = range + projectile.ColliderRad;
                    var targetDir = projectile.Position - origin;
                    
                    if (targetDir.sqrMagnitude < checkRange * checkRange)
                    {
                        outList.Add(projectile);
                    }
                }
            }
        }
        
        public List<IProjectile> GetAllProjectile()
        {
            return _projectiles.Values.ToList();
        }

    }

}
