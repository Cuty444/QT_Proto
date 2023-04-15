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
    [SerializeField] protected Transform _shootPoint;

    private Transform _targetTransform;

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }
    
    public virtual async void Shoot(int shootDataId, AimTypes aimType)
    {
        var shootData = SystemManager.Instance.DataManager.GetDataBase<ShootGameDataBase>().GetData(shootDataId);

        if(shootData == null)
        {
            return;
        }

        foreach (var shoot in shootData)
        {
            var projectileData = SystemManager.Instance.DataManager.GetDataBase<ProjectileGameDataBase>().GetData(shoot.ProjectileDataId);
            if (projectileData == null)
            {
                continue;
            }
            
            var projectile = await SystemManager.Instance.ResourceManager.GetFromPool<Projectile>(projectileData.PrefabPath);
            projectile.transform.position = _shootPoint.position;
            
            var dir = GetDirection(shoot.ShootAngle, aimType);
            projectile.Init(projectileData, dir, shoot.InitalSpd, shoot.MaxBounceCount);
        }
    }

    private Vector2 GetDirection(float angle ,AimTypes aimType)
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
