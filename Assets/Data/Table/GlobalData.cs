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
    /// <summary>
    /// ��� ������ ���� �浹���� �� �ּҷ� �� �� �ִ� ���ط� ����.
    /// </summary>
    public int BounceMinDmg { get => _bounceMinDmg; }
    [SerializeField]
    [Tooltip("���� ���ư��� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _ballSpdDecelerationValue = 1f;
    /// <summary>
    /// ���� ���ư��� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float BallSpdDecelerationValue { get => _ballSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("ƨ��߻� �� �ӵ� �پ��� ���� (0�� ƨ��������, 1�� ������ ƨ�� ������ �ս��� 0%)")]
    [Range(0.0f,1.0f)]
    private float _bounceSpdReductionRate = 1f;
    /// <summary>
    /// ƨ��߻� �� �ӵ� �پ��� ���� (0�� ƨ��������, 1�� ������ ƨ�� ������ �ս��� 0%)
    /// </summary>
    public float BounceSpdReductionRate { get => _bounceSpdReductionRate; }
    [SerializeField]
    [Tooltip("��� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _airborneSpdDecelerationValue = 1f;
    /// <summary>
    /// ��� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float AirborneSpdDecelerationValue { get => _airborneSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("���� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��")]
    private float _rigidSpdDecelerationValue = 1f;
    /// <summary>
    /// ���� ���� ������ ���� ms�� �ӵ��� ���ҵǴ� ��
    /// </summary>
    public float RigidSpdDecelerationValue { get => _rigidSpdDecelerationValue; }
    [SerializeField]
    [Tooltip("��Ʈ�� ������ ����(rigid)���¿� �������� �� �� �ӵ�")]
    private float _rigidSpd = 1f;
    /// <summary>
    /// ��Ʈ�� ������ ����(rigid)���¿� �������� �� �� �ӵ�
    /// </summary>
    public float RigidSpd { get => _rigidSpd; }

    [SerializeField]
    [Tooltip("���� �ӵ��� ���޽� �����Ǵ� �ӵ���")]
    private float _ballMinSpdDestroyed = 0.1f;
    /// <summary>
    /// ���� �ӵ��� ���޽� �����Ǵ� �ӵ���
    /// </summary>
    public float BallMinSpdDestroyed { get => _ballMinSpdDestroyed; }
}
