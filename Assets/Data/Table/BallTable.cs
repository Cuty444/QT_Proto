using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BallTableObject", menuName = "Data/BallTable", order = 1)]
public class BallTable : ScriptableObject
{
    [SerializeField]
    private int _index = 200;
    public int Index => _index;

    [SerializeField]
    [Tooltip("�� �ݶ��̴� ����. ������.")]
    private float _ballColliderRad = 1f;
    /// <summary>
    /// �� �ݶ��̴� ����. ������.
    /// </summary>
    public float BallColliderRad => _ballColliderRad;

    [SerializeField]
    [Tooltip("������ �ӵ�")]
    private float _throwSpd = 1f;
    /// <summary>
    /// ������ �ӵ�
    /// </summary>
    public float ThrowSpd => _throwSpd;

    [SerializeField]
    [Tooltip("���� - ������")]
    private float _ballWeight = 1f;
    /// <summary>
    /// ����
    /// </summary>
    public float BallWeight => _ballWeight;

    [SerializeField]
    [Tooltip("�� ƨ�� ���ط� ����ġ")]
    private float _ballBounceSpdDmgPer = 1f;
    /// <summary>
    /// �� ƨ�� ���ط� ����ġ
    /// </summary>
    public float BallBounceSpdDmgPer => _ballBounceSpdDmgPer;
}
