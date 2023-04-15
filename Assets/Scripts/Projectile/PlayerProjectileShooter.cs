using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QT.Core;

namespace QT
{
    public class PlayerProjectileShooter : ProjectileShooter
    {
        private const string PlayerProjectilePrefabPath = "Prefabs/PlayerProjectile.prefab";
        public virtual async void PlayerShoot(int projectileDataId, AimTypes aimType, Vector2 direction, int bounce, List<Projectile> projectiles)
        {
            var projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>().GetData(projectileDataId);
            if (projectileData == null)
            {
                return;
            }

            var projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(PlayerProjectilePrefabPath);

            projectile.transform.position = _shootPoint.position;

            projectile.Init(projectileData, direction, 50f, bounce);
            if (projectiles.Contains(projectile))
                return;
            projectiles.Add(projectile);
        }
    }
}