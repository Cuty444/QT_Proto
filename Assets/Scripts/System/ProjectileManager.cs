using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public interface IProjectile : IHitable
    {
        public int ProjectileId { get; }
        public Vector2 Position { get; }
        public float ColliderRad { get; }
        
        public void ProjectileHit(Vector2 dir, float power, LayerMask bounceMask);

        public void ResetBounceCount(int maxBounce);

        public LayerMask GetLayerMask();
    }
    
    public class ProjectileManager
    {
        private Dictionary<int, IProjectile> _projectiles = new();
        
        public void Register(IProjectile projectile)
        {
            _projectiles.Add(projectile.ProjectileId, projectile);
        }
        
        public void UnRegister(IProjectile projectile)
        {
            _projectiles.Remove(projectile.ProjectileId);
        }
        
        public void GetInRange(Vector2 origin, float range, float angle, Vector2 dir, ref List<IProjectile> outList, int layerMask)
        {
            foreach (var projectile in _projectiles.Values)
            {
                if ((projectile.GetLayerMask() & layerMask) != 0)
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
    }
}
