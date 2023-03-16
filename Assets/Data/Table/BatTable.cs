using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Util.Flags;

[CreateAssetMenu(fileName = "BatTableObject", menuName = "Data/BatTable", order = 0)]
public class BatTable : ScriptableObject
{
    [SerializeField]
    private int _index = 300;
    public int Index => _index;

    [SerializeField]
    [Tooltip("휘두르기 적중 피해량 미차징, 차징")]
    private int[] _swingRigidDmg;
    /// <summary>
    /// 공격을 통한 즉발 피해량
    /// </summary>
    public int[] SwingRigidDmg => _swingRigidDmg;

    [SerializeField]
    [Tooltip("공격 범위 반지름 - 미적용")]
    private float _atkRad = 1f;
    /// <summary>
    /// 공격 범위 반지름
    /// </summary>
    public float ATKRad => _atkRad;

    [SerializeField]
    [Tooltip("공격 각도 - 미적용")]
    private float _atkCentralAngle = 1f;
    /// <summary>
    /// 공격 각도
    /// </summary>
    public float AtkCentralAngle => _atkCentralAngle;

    [SerializeField]
    [Tooltip("공격 쿨타임 (s)")]
    private float _atkCooldown = 1f;
    /// <summary>
    /// 공격 쿨타임 (s)
    /// </summary>
    public float AtkCooldown => _atkCooldown;

    [SerializeField]
    [Tooltip("공격 발생 지연 시간 (s) - 미적용")]
    private float _atkAfterDelay = 1f;
    /// <summary>
    /// 공격 발생 지연 시간 (s)
    /// </summary>
    public float AtkAfterDelay => _atkAfterDelay;

    [SerializeField]
    [Tooltip("공격으로 튕겨난 공, 적이 다른 콜리전과 충돌하며 입는 피해량 가중치 - 미적용")]
    private float _batBounceSpdDmgPer = 1f;
    /// <summary>
    /// 공격으로 튕겨난 공, 적이 다른 콜리전과 충돌하며 입는 피해량 가중치
    /// </summary>
    public float BounceSpdDmgPer => _batBounceSpdDmgPer;
    [SerializeField]
    [Tooltip("차징 단계에 따라 걸리는 시간(s)")]
    private float[] _chargingMaxTimes;
    /// <summary>
    /// 차징 단계에 따라 걸리는 시간(s)
    /// </summary>
    public float[] ChargingMaxTimes => _chargingMaxTimes;

    [SerializeField]
    [Tooltip("차징 단계에 따른 고정 속도")]
    private float[] _atkShootSpd;
    /// <summary>
    /// 차징 단계에 따라 걸리는 시간(s)
    /// </summary>
    public float[] AtkShootSpd => _atkShootSpd;

    [SerializeField]
    [Header("차징량에 따른 관통샷 여부")]
    private ChargeAtkPierce _chargeAtkPierce;
    public ChargeAtkPierce ChargeAtkPierce => _chargeAtkPierce;
}
