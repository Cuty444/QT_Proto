using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.InGame
{
    public class Steering : MonoBehaviour
    {
        private static LayerMask FallLayerMask => LayerMask.GetMask("Fall");
        
        [Header("장애물 감지")]
        [SerializeField] private float _detectionRadius = 2;
        [SerializeField] private LayerMask _obstacleLayerMask;

        [SerializeField] private float _enemySize;

        public void DetectObstacle(ref DirectionWeights danger)
        {
            var obstacles = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _obstacleLayerMask);

            foreach (var obstacleCollider in obstacles)
            {
                if (obstacleCollider.gameObject == gameObject)
                {
                    continue;
                }

                var obstacleDir = obstacleCollider.ClosestPoint(transform.position) - (Vector2) transform.position;

                var distance = obstacleDir.magnitude;
                var weight = distance <= _enemySize ? 1 : (_detectionRadius - distance) / _detectionRadius;

                danger.AddWeight(obstacleDir, weight);
            }
        }
        
        public Vector2 CalculateContexts(DirectionWeights danger, DirectionWeights interest)
        {
            var dir = Vector2.zero;
            for (var i = 0; i < DirectionWeights.DirCount; i++)
            {
                interest.Weights[i] = Mathf.Clamp01(interest.Weights[i] - danger.Weights[i]);
                dir += DirectionWeights.Directions[i] * interest.Weights[i];
            }
            
            //Debug
#if UNITY_EDITOR
            interest.ShowDebugRays(transform.position, Color.green);
            danger.ShowDebugRays(transform.position, Color.red);
            Debug.DrawRay(transform.position, dir, Color.yellow);
#endif
            
            return dir;
        }

        public bool IsStuck()
        {
            var collider = Physics2D.OverlapPoint(transform.position, FallLayerMask);
            return collider != null;
        }
    }
    
    
    public class DirectionWeights
    {
        public const int DirCount = 8;
        
        public static readonly Vector2[] Directions =
        {
            new(0, 1),
            new Vector2(1, 1).normalized,
            new(1, 0),
            new Vector2(1, -1).normalized,
            new(0, -1),
            new Vector2(-1, -1).normalized,
            new(-1, 0),
            new Vector2(-1, 1).normalized
        };

        public readonly float[] Weights = new float[DirCount];

        public int AddedWeightCount { get; private set; } = 0;

        public void AddWeight(Vector2 dir, float weight)
        {
            dir.Normalize();

            for (var i = 0; i < DirCount; i++)
            {
                var result = Vector2.Dot(dir, Directions[i]) * weight;

                if (result > Weights[i])
                {
                    Weights[i] = result;
                }
            }

            AddedWeightCount++;
        }

        public void ShowDebugRays(Vector2 start, Color color)
        {
#if UNITY_EDITOR
            for (var i = 0; i < DirCount; i++)
            {
                Debug.DrawRay(start, Directions[i] * Weights[i], color);
            }
#endif
        }
    }
}
