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
    [Tooltip("던져서 맞췄을 때 딜")]
    private int _ballRigidDmg = 100;
    /// <summary>
    /// 던져서 맞췄을 때 딜
    /// </summary>
    public int BallRigidDmg { get => _ballRigidDmg; }
    [SerializeField]
    [Tooltip("공 범위. 반지름.")]
    private float _ballRad = 1f;
    /// <summary>
    /// 공 범위. 반지름.
    /// </summary>
    public float BallRad { get => _ballRad; }
    [SerializeField]
    [Tooltip("던질때 속도")]
    private float _throwSpd = 1f;
    /// <summary>
    /// 던질때 속도
    /// </summary>
    public float ThrowSpd { get => _throwSpd; }
    [SerializeField]
    [Tooltip("무게")]
    private float _ballWeight = 1f;
    /// <summary>
    /// 무게
    /// </summary>
    public float BallWeight { get => _ballWeight; }
    [SerializeField]
    [Tooltip("공을 튕겨냈을 때 속도당 데미지 퍼센트")]
    private float _ballBounceSpdDmgPer = 1f;
    /// <summary>
    /// 공을 튕겨냈을 때 속도당 데미지 퍼센트
    /// </summary>
    public float BallBounceSpdDmgPer { get => _ballBounceSpdDmgPer; }
}
