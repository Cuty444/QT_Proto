using System.Collections;
using System.Collections.Generic;
using QT;
using QT.Core;
using UnityEngine;

public enum AimTypes
{
    World,
    Angle,
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
        }
    }

    private Vector2 GetDirection(float angle ,AimTypes aimType)
    {
        switch (aimType)
        {
            case AimTypes.Angle:
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
