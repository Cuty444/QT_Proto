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
    [Tooltip("공 콜라이더 범위. 반지름.")]
    private float _ballColliderRad = 1f;
    /// <summary>
    /// 공 콜라이더 범위. 반지름.
    /// </summary>
    public float BallColliderRad => _ballColliderRad;

    [SerializeField]
    [Tooltip("던질때 속도")]
    private float _throwSpd = 1f;
    /// <summary>
    /// 던질때 속도
    /// </summary>
    public float ThrowSpd => _throwSpd;

    [SerializeField]
    [Tooltip("무게 - 미적용")]
    private float _ballWeight = 1f;
    /// <summary>
    /// 무게
    /// </summary>
    public float BallWeight => _ballWeight;

    [SerializeField]
    [Tooltip("볼 튕김 피해량 가중치")]
    private float _ballBounceSpdDmgPer = 1f;
    /// <summary>
    /// 볼 튕김 피해량 가중치
    /// </summary>
    public float BallBounceSpdDmgPer => _ballBounceSpdDmgPer;
}
