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
        public async virtual void PlayerShoot(int shootDataId, AimTypes aimType, Vector2 direction, int bounce,List<Projectile> projectiles)
        {
            var projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>()
                .GetData(shootDataId);
            if (projectileData == null)
            {
                return;
            }

            var projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(PlayerProjectilePrefabPath);

            projectile.transform.position = _shootPoint.position;

            projectile.Init(50f,0f,0.5f, direction, bounce,10,true);
            if (projectiles.Contains(projectile))
                return;
            projectiles.Add(projectile);
        }
    }
}