using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GlobalData : ScriptableObject
{
    [SerializeField]
    private string _index = "0";
    public string Index => _index;

    [SerializeField]
    [Tooltip("튕김이 최소로 들어갈 수 있는 피해량 제한.")]
    private int _bounceMinDmg = 100;
    /// <summary>
    /// 튕김이 최소로 들어갈 수 있는 피해량 제한.
    /// </summary>
    public int BounceMinDmg => _bounceMinDmg;

    [SerializeField]
    [Tooltip("공이 날아갈때 ms당 속도가 감소되는 값")]
    private float _ballSpdDecelerationValue = 1f;
    /// <summary>
    /// 공이 날아갈때 ms당 속도가 감소되는 값
    /// </summary>
    public float BallSpdDecelerationValue => _ballSpdDecelerationValue;

    [SerializeField]
    [Tooltip("튕김발생 시 속도 줄어드는 비율 (0은 튕기지않음, 1은 무한히 튕김 에너지 손실율 0%)")]
    [Range(0.0f,1.0f)]
    private float _bounceSpdReductionRate = 1f;
    /// <summary>
    /// 튕김발생 시 속도 줄어드는 비율 (0은 튕기지않음, 1은 무한히 튕김 에너지 손실율 0%)
    /// </summary>
    public float BounceSpdReductionRate => _bounceSpdReductionRate;

    [SerializeField]
    [Tooltip("에어본 상태 진입한 적이 ms당 속도가 감소되는 값 - 미적용")]
    private float _airborneSpdDecelerationValue = 1f;
    /// <summary>
    /// 에어본 상태 진입한 적이 ms당 속도가 감소되는 값
    /// </summary>
    public float AirborneSpdDecelerationValue => _airborneSpdDecelerationValue;

    [SerializeField]
    [Tooltip("경직 상태 진입한 적이 ms당 속도가 감소되는 값 - 미적용")]
    private float _rigidSpdDecelerationValue = 1f;
    /// <summary>
    /// 경직 상태 진입한 적이 ms당 속도가 감소되는 값
    /// </summary>
    public float RigidSpdDecelerationValue => _rigidSpdDecelerationValue;

    [SerializeField]
    [Tooltip("배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도 - 미적용")]
    private float _rigidSpd = 1f;
    /// <summary>
    /// 배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도
    /// </summary>
    public float RigidSpd => _rigidSpd;

    [SerializeField]
    [Tooltip("일정 속도에 도달시 삭제되는 속도값")]
    private float _ballMinSpdDestroyed = 0.1f;
    /// <summary>
    /// 일정 속도에 도달시 삭제되는 속도값
    /// </summary>
    public float BallMinSpdDestroyed => _ballMinSpdDestroyed;

    [SerializeField]
    [Tooltip("[기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간")]
    /// <summary>
    /// [기절] 상태 진입 후 사망처리 되는데 까지 걸리는 시간
    /// </summary>
    private float _deadAfterStunTime = 5f;
    public float DeadAfterStunTime => _deadAfterStunTime;
}
