using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Data
{

    public class GlobalData : ScriptableObject
    {
        /// <summary>
        /// 튕김이 최소로 들어갈 수 있는 피해량 제한.
        /// </summary>
        [field: SerializeField]
        [Tooltip("튕김이 최소로 들어갈 수 있는 피해량 제한.")]
        public int BounceMinDmg { get; private set; } = 100;

        /// <summary>
        /// 공이 날아갈때 ms당 속도가 감소되는 값
        /// </summary>
        [field: SerializeField]
        [Tooltip("공이 날아갈때 ms당 속도가 감소되는 값")]
        public float SpdDecay { get; private set; } = 1;


        /// <summary>
        /// 튕김발생 시 속도 줄어드는 비율 (0은 튕기지않음, 1은 무한히 튕김 에너지 손실율 0%)
        /// </summary>
        [Tooltip("튕김발생 시 속도 줄어드는 비율 (0은 튕기지않음, 1은 무한히 튕김 에너지 손실율 0%)")]
        [field: Range(0.0f, 1.0f)]
        public float BounceSpdReductionRate { get; private set; } = 0.5f;

        /// <summary>
        /// 에어본 상태 진입한 적이 ms당 속도가 감소되는 값
        /// </summary>
        [field: SerializeField]
        [Tooltip("에어본 상태 진입한 적이 ms당 속도가 감소되는 값 - 미적용")]
        public float AirborneSpdDecelerationValue { get; private set; } = 1;

        /// <summary>
        /// 경직 상태 진입한 적이 ms당 속도가 감소되는 값
        /// </summary>
        [field: SerializeField]
        [Tooltip("경직 상태 진입한 적이 ms당 속도가 감소되는 값 - 미적용")]
        public float RigidSpdDecelerationValue { get; private set; } = 1;

        /// <summary>
        /// 배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도
        /// </summary>
        [field: SerializeField]
        [Tooltip("배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도 - 미적용")]
        public float RigidSpd { get; private set; } = 0.1f;

        /// <summary>
        /// 배트로 때려서 경직(rigid)상태가 풀릴때까지 걸리는 시간
        /// </summary>
        [field: SerializeField]
        [Tooltip("배트로 때려서 경직(rigid)상태가 풀릴때까지 걸리는 시간")]
        public float RigidTime { get; private set; } = 0.1f;

        /// <summary>
        /// 일정 속도에 도달시 삭제되는 속도값
        /// </summary>
        [field: SerializeField]
        [Tooltip("일정 속도에 도달시 삭제되는 속도값")]
        public float BallMinSpdDestroyed { get; private set; } = 0.1f;


        /// <summary>
        /// [기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간
        /// </summary>
        [field: SerializeField]
        [Tooltip("[기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간")]
        public float DeadAfterStunTime { get; private set; } = 5;
        
        
        /// <summary>
        /// 적 피격 시 깜박임 효과 지속시간
        /// </summary>
        [field: SerializeField]
        [Tooltip("적 피격 시 깜박임 효과 지속시간")]
        public float EnemyHitEffectDuration { get; private set; } = 0.1f;
        
        
        /// <summary>
        /// 적 피격 시 깜박임 효과 그래프
        /// </summary>
        [field: SerializeField]
        [Tooltip("적 피격 시 깜박임 효과 그래프")]
        public AnimationCurve EnemyHitEffectCurve { get; private set; } = new ();
        
        /// <summary>
        /// 플레이어 피격 시 깜박임 효과 그래프
        /// </summary>
        [field: SerializeField]
        [Tooltip("플레이어 피격 시 깜박임 효과 그래프")]
        public AnimationCurve PlayerHitEffectCurve { get; private set; } = new ();
        
        /// <summary>
        /// 적 낙사 스케일링 그래프
        /// </summary>
        [field: SerializeField]
        [Tooltip("적 낙사 스케일링 그래프")]
        public AnimationCurve EnemyFallScaleCurve { get; private set; } = new ();
        
        /// <summary>
        /// 범위 안 텔레포트 컬러
        /// </summary>
        [field: SerializeField]
        [Tooltip("범위 안 텔레포트 컬러")]
        public Color CloseColor { get; private set; } = new ();
        
        /// <summary>
        /// 범위 안 텔레포트 컬러
        /// </summary>
        [field: SerializeField]
        [Tooltip("범위 밖 텔레포트 컬러")]
        public Color FarColor { get; private set; } = new ();
        
        /// <summary>
        /// 플레이어 공속 게이지 커브
        /// </summary>
        [field: SerializeField]
        [Tooltip("공속 게이지 그래프")]
        public Gradient AttackSpeedColorCurve { get; private set; }
    }
}
