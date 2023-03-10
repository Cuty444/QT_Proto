using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BallTableObject", menuName = "Data/BallTable", order = 0)]
public class BallTable : ScriptableObject
{
    [SerializeField]
    private int _index = 200;
    public int Index { get => _index; }
    [SerializeField]
    [Tooltip("������ ������ �� ��")]
    private int _ballRigidDmg = 100;
    /// <summary>
    /// ������ ������ �� ��
    /// </summary>
    public int BallRigidDmg { get => _ballRigidDmg; }
    [SerializeField]
    [Tooltip("�� ����. ������.")]
    private float _ballRad = 1f;
    /// <summary>
    /// �� ����. ������.
    /// </summary>
    public float BallRad { get => _ballRad; }
    [SerializeField]
    [Tooltip("������ �ӵ�")]
    private float _throwSpd = 1f;
    /// <summary>
    /// ������ �ӵ�
    /// </summary>
    public float ThrowSpd { get => _throwSpd; }
    [SerializeField]
    [Tooltip("����")]
    private float _ballWeight = 1f;
    /// <summary>
    /// ����
    /// </summary>
    public float BallWeight { get => _ballWeight; }
    [SerializeField]
    [Tooltip("���� ƨ�ܳ��� �� �ӵ��� ������ �ۼ�Ʈ")]
    private float _ballBounceSpdDmgPer = 1f;
    /// <summary>
    /// ���� ƨ�ܳ��� �� �ӵ��� ������ �ۼ�Ʈ
    /// </summary>
    public float BallBounceSpdDmgPer { get => _ballBounceSpdDmgPer; }
}
