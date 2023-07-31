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

    public interface IHitable
    {
        public int InstanceId { get; }

        public Vector2 Position { get; }
        public float ColliderRad { get; }

        public void Hit(Vector2 dir, float power, AttackType attackType = AttackType.Ball);

        public float GetHp();
    }

    public class HitableManager
    {
        private readonly Dictionary<int, IHitable> _hitables = new();

        public void Register(IHitable hitable)
        {
            _hitables.Add(hitable.InstanceId, hitable);
        }

        public void UnRegister(IHitable hitable)
        {
            _hitables.Remove(hitable.InstanceId);
        }

        public void Clear()
        {
            _hitables.Clear();
        }

        public void GetInRange(Vector2 origin, float range, float angle, Vector2 dir, ref List<IHitable> outList)
        {
            foreach (var hitable in _hitables.Values)
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
        
        public void GetInRange(Vector2 origin, float range, ref List<IHitable> outList)
        {
            foreach (var hitable in _hitables.Values)
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