using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core.Data
{

    public class GlobalData : ScriptableObject
    {
        /// <summary>
        /// 차징 이동속도 및 이펙트 적용까지 유예 시간
        /// </summary>
        [field: Header("차징 대기")]
        [field: SerializeField]
        [Tooltip("차징 이동속도 및 이펙트 적용까지 유예 시간")]
        public float ChargeAtkDelay { get; private set; } = 0.1f;
        
        
        /// <summary>
        /// 배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도
        /// </summary>
        [field: Header("적 경직")]
        [field: SerializeField]
        [Tooltip("경직(rigid)상태에 진입했을 때 적 넉백 속도")]
        public float KnockBackSpd { get; private set; } = 4;
        
        /// <summary>
        /// 배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도
        /// </summary>
        [field: SerializeField]
        [Tooltip("배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도")]
        public float KnockBackTime { get; private set; } = 0.2f;


        /// <summary>
        /// 배트로 때려서 경직(rigid)상태가 풀릴때까지 걸리는 시간
        /// </summary>
        [field: SerializeField]
        [Tooltip("배트로 때려서 경직(rigid)상태가 풀릴때까지 걸리는 시간")]
        public float RigidTime { get; private set; } = 0.1f;
        
        /// <summary>
        /// [기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간
        /// </summary>
        [field: SerializeField]
        [Tooltip("[기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간")]
        public float DeadAfterStunTime { get; private set; } = 5;
        
        
        
        /// <summary>
        /// 일정 속도에 도달시 삭제되는 속도값
        /// </summary>
        [field: Header("투사체")]
        [field: SerializeField]
        [Tooltip("일정 속도에 도달시 삭제되는 속도값")]
        public float BallMinSpdDestroyed { get; private set; } = 0.1f;
        
        /// <summary>
        /// 공이 날아갈때 ms당 속도가 감소되는 값
        /// </summary>
        [field: SerializeField]
        [Tooltip("공이 날아갈때 ms당 속도가 감소되는 값")]
        public float SpdDecay { get; private set; } = 1;
        
        
        
        /// <summary>
        /// 적 피격 시 깜박임 효과 지속시간
        /// </summary>
        [field: Header("피격")]
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
        /// 플레이어 공격 범위 색깔
        /// </summary>
        [field: SerializeField]
        [Tooltip("플레이어 공격 범위 색깔")]
        public Color SwingAreaColor { get; private set; }
        
        /// <summary>
        /// 플레이어 공속 게이지 커브
        /// </summary>
        [field: SerializeField]
        [Tooltip("공속 게이지 그래프")]
        public Gradient AttackSpeedColorCurve { get; private set; }
    }
}
