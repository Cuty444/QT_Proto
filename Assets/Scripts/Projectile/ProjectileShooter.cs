using System.Collections;
using System.Collections.Generic;
using QT;
using QT.Core;
using UnityEngine;

public enum AimTypes
{
    World,
    Target,
    MoveDirection,
}

public class ProjectileShooter : MonoBehaviour
{
    protected virtual LayerMask _bounceMask => LayerMask.GetMask("Wall");
    
    [SerializeField] protected Transform _shootPoint;

    private Transform _targetTransform;

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }
    
    public virtual void Shoot(int shootDataId, AimTypes aimType)
    {
        var shootData = SystemManager.Instance.DataManager.GetDataBase<ShootGameDataBase>().GetData(shootDataId);

        if(shootData == null)
        {
            return;
        }

        foreach (var shoot in shootData)
        {
            var dir = GetDirection(shoot.ShootAngle, aimType);
            ShootProjectile(shoot.ProjectileDataId, dir, shoot.InitalSpd, shoot.MaxBounceCount);
        }
    }

    public virtual async void ShootProjectile(int projectileDataId, Vector2 dir, float speed, int bounceCount)
    {
        var projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>().GetData(projectileDataId);
        if (projectileData == null)
        {
            return;
        }
            
        var projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(projectileData.PrefabPath);
        projectile.transform.position = _shootPoint.position;
            
        projectile.Init(projectileData, dir, speed, bounceCount, _bounceMask);
    }

    protected Vector2 GetDirection(float angle, AimTypes aimType)
    {
        switch (aimType)
        {
            case AimTypes.Target:
                var dir = _targetTransform.position - transform.position;
                angle += Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                break;
            
            case AimTypes.MoveDirection:
                angle += transform.rotation.eulerAngles.z;
                break;
        }

        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    
}
