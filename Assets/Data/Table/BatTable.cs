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
    [Tooltip("������ ���� ��� ���ط�")]
    private int _atkDmg = 100;
    public int AtkDmg { get => _atkDmg; }
    [SerializeField]
    [Tooltip("���� ���� ������")]
    private float _atkRad = 1f;
    public float ATKRad { get => _atkRad; }
    [SerializeField]
    [Tooltip("���� ����")]
    private float _atkCentralAngle = 1f;
    public float AtkCentralAngle { get => _atkCentralAngle; }
    [SerializeField]
    [Tooltip("���� ��Ÿ�� (s)")]
    private float _atkCooldown = 1f;
    public float AtkCooldown { get => _atkCooldown; }
    [SerializeField]
    [Tooltip("���� �߻� ���� �ð� (s)")]
    private float _atkAfterDelay = 1f;
    public float AtkAfterDelay { get => _atkAfterDelay; }
    [SerializeField]
    [Tooltip("�������� ��ƨ�衯�߻� �� ����ü �̵� �ӵ�")]
    private float _atkSpd = 1f;
    public float AtkSpd { get => _atkSpd; }
    [SerializeField]
    [Tooltip("�������� ƨ�ܳ� ���� �ٸ� �ݸ����� �浹�ϸ� �Դ� ���ط� ����ġ")]
    private float _bounceSpdDmgPer = 1f;
    public float BounceSpdDmgPer { get => _bounceSpdDmgPer; }
}
