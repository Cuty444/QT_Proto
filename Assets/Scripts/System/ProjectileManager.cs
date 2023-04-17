using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public interface IProjectile
    {
        public int ProjectileId { get; }
        public Vector2 Position { get; }
        
        public void Hit(Vector2 dir, float power);
        public void Hit(Vector2 dir, float power, LayerMask bounceMask);

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
        
        public void GetInRange(Vector2 origin, float range, ref List<IProjectile> outList, int layerMask)
        {
            foreach (var projectile in _projectiles.Values)
            {
                if ((projectile.GetLayerMask() & layerMask) != 0)
                {
                    if ((origin - projectile.Position).sqrMagnitude < range * range)
                    {
                        outList.Add(projectile);
                    }
                }
            }
        }
    }
}
