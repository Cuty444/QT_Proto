using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterTableObject",menuName = "Data/CharacterTable", order = 2)]
public class CharacterTable : ScriptableObject
{
    [SerializeField]
    private int _index = 100;
    public int Index => _index;

    [SerializeField]
    [Tooltip("최대 체력")]
    private int _hpMax = 100;
    public int HPMax => _hpMax;
    
    [SerializeField]
    [Tooltip("가지고 있는 공 최대 갯수 제한")]
    private int _ballStackMax = 3;
    /// <summary>
    /// 가지고 있는 공 최대 갯수 제한
    /// </summary>
    public int BallStackMax => _ballStackMax;

    [SerializeField]
    [Tooltip("캐릭터 히트박스 원형 반지름값")]
    private float _pcHitBoxRad = 0.5f;
    /// <summary>
    /// 캐릭터 히트박스 원형 반지름값
    /// </summary>
    public float PCHitBoxRad => _pcHitBoxRad;

    [SerializeField]
    [Tooltip("이동 속도. unit")]
    private float _movementSpd = 1f;
    /// <summary>
    /// 이동 속도. unit
    /// </summary>
    public float MovementSpd => _movementSpd;

    [SerializeField]
    [Tooltip("피격 시 무적 시간(s)")]
    private float _mercyInvincibleTime = 1f;
    /// <summary>
    /// 피격 시 무적 시간(s)
    /// </summary>
    public float MercyInvincibleTime => _mercyInvincibleTime;

    [SerializeField]
    [Tooltip("회피 후 재사용 대기 시간(s)")]
    private float _dodgeCooldown = 1f;
    /// <summary>
    /// 회피 후 재사용 대기 시간(s)
    /// </summary>
    public float DodgeCooldown => _dodgeCooldown;

    [SerializeField]
    [Tooltip("회피 시 무적 시간(s)")]
    private float _dodgeInvincibleTime = 1f;
    /// <summary>
    /// 회피 시 무적 시간(s)
    /// </summary>
    public float DodgeInvincibleTime => _dodgeInvincibleTime;
    
    [SerializeField]
    [Tooltip("회피 시 추가 가속 배율(*)")]
    private float _dodgeDecelerationSpeed = 1f;
    /// <summary>
    /// 회피 시 추가 가속 배율(*)
    /// </summary>
    public float DodgeDecelerationSpeed => _dodgeDecelerationSpeed;

    [SerializeField]
    [Tooltip("회피 거리(unit)")]
    private float _dodgeStopDistance = 1f;
    /// <summary>
    /// 회피 거리(unit)
    /// </summary>
    public float DodgeStopDistance => _dodgeStopDistance;
    
    [SerializeField]
    [Tooltip("차징중 이동 속도감소비율(f)")]
    [Range(0f,1f)]
    private float _chargeMovementDecreasePer = 1f;
    /// <summary>
    /// 차징중 이동 속도감소비율(f)
    /// </summary>
    public float ChargeMovementDecreasePer => _chargeMovementDecreasePer;
}
