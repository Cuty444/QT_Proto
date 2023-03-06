using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GlobalData : ScriptableObject
{
    [SerializeField]
    private string _index = "0";
    public string Index { get => _index; }
    [SerializeField]
    [Tooltip("에어본 상태의 적이 충돌했을 때 최소로 들어갈 수 있는 피해량 제한.")]
    private int _bounceMinDmg = 100;
    public int BounceMinDmg { get => _bounceMinDmg; }
    [SerializeField]
    [Tooltip("공이 날아갈때 ms당 속도가 감소되는 값")]
    private float _ballSpdDecelerationValue = 1f;
    public float BallSpdDecelerationValue { get => _ballSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("에어본 상태 진입한 적이 ms당 속도가 감소되는 값")]
    private float _airborneSpdDecelerationValue = 1f;
    public float AirborneSpdDecelerationValue { get => _airborneSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("경직 상태 진입한 적이 ms당 속도가 감소되는 값")]
    private float _rigidSpdDecelerationValue = 1f;
    public float RigidSpdDecelerationValue { get => _rigidSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("튕김발생 시 속도 줄어드는 비율")]
    private float _bounceSpdReductionRate = 1f;
    public float BounceSpdReductionRate { get => _bounceSpdReductionRate; }
    [SerializeField]
    [Tooltip("배트로 때려서 경직(rigid)상태에 진입했을 때 적 속도")]
    private float _rigidSpd = 1f;
    public float RigidSpd { get => _rigidSpd; }
}
