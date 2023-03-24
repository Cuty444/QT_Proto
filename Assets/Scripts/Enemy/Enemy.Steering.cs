using UnityEngine;

namespace QT.Enemy
{
    public partial class Enemy
    {
        [Header("장애물 감지")]
        [SerializeField] private float _detectionRadius = 2;
        [SerializeField] private LayerMask _obstacleLayerMask;

        [SerializeField] private float _enemySize;

        public void DetectObstacle(DirectionWeights danger)
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
    }


    public class DirectionWeights
    {
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

        public readonly float[] Weights = new float[8];

        public void AddWeight(Vector2 dir, float weight)
        {
            dir.Normalize();

            for (var i = 0; i < 8; i++)
            {
                var result = Vector2.Dot(dir, Directions[i]) * weight;

                if (result > Weights[i])
                {
                    Weights[i] = result;
                }
            }
        }

        public void ShowDebugRays(Vector2 start, Color color)
        {
#if UNITY_EDITOR
            for (var i = 0; i < 8; i++)
            {
                Debug.DrawRay(start, Directions[i] * Weights[i], color);
            }
#endif
        }
    }
}