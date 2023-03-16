using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GlobalData : ScriptableObject
{
    [SerializeField]
    private string _index = "0";
    public string Index => _index;

    [SerializeField]
    [Tooltip("ƨ���� �ּҷ� �� �� �ִ� ���ط� ����.")]
    private int _bounceMinDmg = 100;
    /// <summary>
    /// ƨ���� �ּҷ� �� �� �ִ� ���ط� ����.
    /// </summary>
    public int BounceMinDmg => _bounceMinDmg;

    [SerializeField]
    [Tooltip("���� ���ư��� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _ballSpdDecelerationValue = 1f;
    /// <summary>
    /// ���� ���ư��� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float BallSpdDecelerationValue => _ballSpdDecelerationValue;

    [SerializeField]
    [Tooltip("ƨ��߻� �� �ӵ� �پ��� ���� (0�� ƨ��������, 1�� ������ ƨ�� ������ �ս��� 0%)")]
    [Range(0.0f,1.0f)]
    private float _bounceSpdReductionRate = 1f;
    /// <summary>
    /// ƨ��߻� �� �ӵ� �پ��� ���� (0�� ƨ��������, 1�� ������ ƨ�� ������ �ս��� 0%)
    /// </summary>
    public float BounceSpdReductionRate => _bounceSpdReductionRate;

    [SerializeField]
    [Tooltip("��� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� �� - ������")]
    private float _airborneSpdDecelerationValue = 1f;
    /// <summary>
    /// ��� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float AirborneSpdDecelerationValue => _airborneSpdDecelerationValue;

    [SerializeField]
    [Tooltip("���� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� �� - ������")]
    private float _rigidSpdDecelerationValue = 1f;
    /// <summary>
    /// ���� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float RigidSpdDecelerationValue => _rigidSpdDecelerationValue;

    [SerializeField]
    [Tooltip("��Ʈ�� ������ ����(rigid)���¿� �������� �� �� �ӵ� - ������")]
    private float _rigidSpd = 1f;
    /// <summary>
    /// ��Ʈ�� ������ ����(rigid)���¿� �������� �� �� �ӵ�
    /// </summary>
    public float RigidSpd => _rigidSpd;

    [SerializeField]
    [Tooltip("���� �ӵ��� ���޽� �����Ǵ� �ӵ���")]
    private float _ballMinSpdDestroyed = 0.1f;
    /// <summary>
    /// ���� �ӵ��� ���޽� �����Ǵ� �ӵ���
    /// </summary>
    public float BallMinSpdDestroyed => _ballMinSpdDestroyed;
}
