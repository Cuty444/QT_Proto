using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BatTableObject", menuName = "Data/BatTable", order = 0)]
public class BatTable : ScriptableObject
{
    [SerializeField]
    private int _index = 300;
    public int Index { get => _index; }
    [SerializeField]
    [Tooltip("공격을 통한 즉발 피해량")]
    private int _atkDmg = 100;
    /// <summary>
    /// 공격을 통한 즉발 피해량
    /// </summary>
    public int AtkDmg { get => _atkDmg; }
    [SerializeField]
    [Tooltip("공격 범위 반지름")]
    private float _atkRad = 1f;
    /// <summary>
    /// 공격 범위 반지름
    /// </summary>
    public float ATKRad { get => _atkRad; }
    [SerializeField]
    [Tooltip("공격 각도")]
    private float _atkCentralAngle = 1f;
    /// <summary>
    /// 공격 각도
    /// </summary>
    public float AtkCentralAngle { get => _atkCentralAngle; }
    [SerializeField]
    [Tooltip("공격 쿨타임 (s)")]
    private float _atkCooldown = 1f;
    /// <summary>
    /// 공격 쿨타임 (s)
    /// </summary>
    public float AtkCooldown { get => _atkCooldown; }
    [SerializeField]
    [Tooltip("공격 발생 지연 시간 (s)")]
    private float _atkAfterDelay = 1f;
    /// <summary>
    /// 공격 발생 지연 시간 (s)
    /// </summary>
    public float AtkAfterDelay { get => _atkAfterDelay; }
    [SerializeField]
    [Tooltip("공격으로 ‘튕김’발생 시 투사체 이동 속도")]
    private float _atkSpd = 1f;
    /// <summary>
    /// 공격으로 ‘튕김’발생 시 투사체 이동 속도
    /// </summary>
    public float AtkSpd { get => _atkSpd; }
    [SerializeField]
    [Tooltip("공격으로 튕겨난 적이 다른 콜리전과 충돌하며 입는 피해량 가중치")]
    private float _bounceSpdDmgPer = 1f;
    /// <summary>
    /// 공격으로 튕겨난 적이 다른 콜리전과 충돌하며 입는 피해량 가중치
    /// </summary>
    public float BounceSpdDmgPer { get => _bounceSpdDmgPer; }
}
