using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyTableObject", menuName = "Data/EnemyTable", order = 5)]
public class EnemyTable : ScriptableObject
{
    [SerializeField]
    private int _index = 600;
    public int Index => _index;

    [SerializeField]
    [Tooltip("�ִ� ü��")]
    private int _hpMax = 100;
    public int HPMax => _hpMax;

    [SerializeField]
    [Tooltip("���ݷ�")]
    private int _atkDmg = 100;
    public int ATKDmg => _atkDmg;
    
    [SerializeField]
    [Tooltip("�̵� �ӵ�")]
    private float _movementSpd = 100;
    public float MovementSpd => _movementSpd;
}
