using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GlobalData : ScriptableObject
{
    [SerializeField]
    private string _index = "0";
    public string Index { get => _index; }
    [SerializeField]
    [Tooltip("��� ������ ���� �浹���� �� �ּҷ� �� �� �ִ� ���ط� ����.")]
    private int _bounceMinDmg = 100;
    public int BounceMinDmg { get => _bounceMinDmg; }
    [SerializeField]
    [Tooltip("���� ���ư��� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _ballSpdDecelerationValue = 1f;
    public float BallSpdDecelerationValue { get => _ballSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("��� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _airborneSpdDecelerationValue = 1f;
    public float AirborneSpdDecelerationValue { get => _airborneSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("���� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _rigidSpdDecelerationValue = 1f;
    public float RigidSpdDecelerationValue { get => _rigidSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("ƨ��߻� �� �ӵ� �پ��� ����")]
    private float _bounceSpdReductionRate = 1f;
    public float BounceSpdReductionRate { get => _bounceSpdReductionRate; }
    [SerializeField]
    [Tooltip("��Ʈ�� ������ ����(rigid)���¿� �������� �� �� �ӵ�")]
    private float _rigidSpd = 1f;
    public float RigidSpd { get => _rigidSpd; }
}
