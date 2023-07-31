using System.Collections.Generic;
using UnityEngine;

namespace QT
{
    public enum AttackType
    {
        Ball,
        Swing,
        Teleport
    }

    public interface IHitAble
    {
        public int InstanceId { get; }

        public Vector2 Position { get; }
        public float ColliderRad { get; }

        public void Hit(Vector2 dir, float power, AttackType attackType = AttackType.Ball);

        public float GetHp();
    }

    public class HitAbleManager : Singleton<HitAbleManager>
    {
        private readonly Dictionary<int, IHitAble> _hitAbles = new();

        public void Register(IHitAble hitAble)
        {
            _hitAbles.Add(hitAble.InstanceId, hitAble);
        }

        public void UnRegister(IHitAble hitAble)
        {
            _hitAbles.Remove(hitAble.InstanceId);
        }

        public void Clear()
        {
            _hitAbles.Clear();
        }

        public void GetInRange(Vector2 origin, float range, float angle, Vector2 dir, ref List<IHitAble> outList)
        {
            foreach (var hitable in _hitAbles.Values)
            {
                var checkRange = range + hitable.ColliderRad;
                var targetDir = hitable.Position - origin;

                if (targetDir.sqrMagnitude < checkRange * checkRange)
                {
                    var dot = Vector2.Dot(targetDir.normalized, dir);
                    var degrees = Mathf.Acos(dot) * Mathf.Rad2Deg;

                    if (degrees < angle) outList.Add(hitable);
                }
            }
        }
        
        public void GetInRange(Vector2 origin, float range, ref List<IHitAble> outList)
        {
            foreach (var hitable in _hitAbles.Values)
            {
                var checkRange = range + hitable.ColliderRad;
                var targetDir = hitable.Position - origin;

                if (targetDir.sqrMagnitude < checkRange * checkRange)
                {
                    outList.Add(hitable);
                }
            }
        }
    }
}