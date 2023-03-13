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
    [Tooltip("������ ���� ��� ���ط�")]
    private int _atkDmg = 100;
    /// <summary>
    /// ������ ���� ��� ���ط�
    /// </summary>
    public int AtkDmg => _atkDmg;

    [SerializeField]
    [Tooltip("���� ���� ������ - ������")]
    private float _atkRad = 1f;
    /// <summary>
    /// ���� ���� ������
    /// </summary>
    public float ATKRad => _atkRad;

    [SerializeField]
    [Tooltip("���� ���� - ������")]
    private float _atkCentralAngle = 1f;
    /// <summary>
    /// ���� ����
    /// </summary>
    public float AtkCentralAngle => _atkCentralAngle;

    [SerializeField]
    [Tooltip("���� ��Ÿ�� (s)")]
    private float _atkCooldown = 1f;
    /// <summary>
    /// ���� ��Ÿ�� (s)
    /// </summary>
    public float AtkCooldown => _atkCooldown;

    [SerializeField]
    [Tooltip("���� �߻� ���� �ð� (s) - ������")]
    private float _atkAfterDelay = 1f;
    /// <summary>
    /// ���� �߻� ���� �ð� (s)
    /// </summary>
    public float AtkAfterDelay => _atkAfterDelay;

    [SerializeField]
    [Tooltip("�������� ��ƨ�衯�߻� �� ����ü �̵� �ӵ�")]
    private float _atkSpd = 1f;
    /// <summary>
    /// �������� ��ƨ�衯�߻� �� ����ü �̵� �ӵ�
    /// </summary>
    public float AtkSpd => _atkSpd;

    [SerializeField]
    [Tooltip("�������� ƨ�ܳ� ���� �ٸ� �ݸ����� �浹�ϸ� �Դ� ���ط� ����ġ")]
    private float _bounceSpdDmgPer = 1f;
    /// <summary>
    /// �������� ƨ�ܳ� ���� �ٸ� �ݸ����� �浹�ϸ� �Դ� ���ط� ����ġ
    /// </summary>
    public float BounceSpdDmgPer => _bounceSpdDmgPer;
    [SerializeField]
    [Tooltip("��¡ �ܰ迡 ���� �ɸ��� �ð�(s)")]
    private float[] _chargingMaxTimes;
    /// <summary>
    /// ��¡ �ܰ迡 ���� �ɸ��� �ð�(s)
    /// </summary>
    public float[] ChargingMaxTimes => _chargingMaxTimes;

    [SerializeField]
    [Header("��¡���� ���� ���뼦 ����")]
    private ChargeAtkPierce _chargeAtkPierce;
    public ChargeAtkPierce ChargeAtkPierce => _chargeAtkPierce;
}
