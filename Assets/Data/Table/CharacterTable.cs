using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterTableObject",menuName = "Data/CharacterTable", order = 2)]
public class CharacterTable : ScriptableObject
{
    [SerializeField]
    private int _index = 100;
    public int Index => _index;

    [SerializeField]
    [Tooltip("�ִ� ü��")]
    private int _hpMax = 100;
    public int HPMax => _hpMax;

    [SerializeField]
    [Tooltip("ĳ���� ��Ʈ�ڽ� ���� ��������")]
    private float _pcHitBoxRad = 0.5f;
    /// <summary>
    /// ĳ���� ��Ʈ�ڽ� ���� ��������
    /// </summary>
    public float PCHitBoxRad => _pcHitBoxRad;

    [SerializeField]
    [Tooltip("�̵� �ӵ�. unit")]
    private float _movementSpd = 1f;
    /// <summary>
    /// �̵� �ӵ�. unit
    /// </summary>
    public float MovementSpd => _movementSpd;

    [SerializeField]
    [Tooltip("�ǰ� �� ���� �ð�(s)")]
    private float _mercyInvincibleTime = 1f;
    /// <summary>
    /// �ǰ� �� ���� �ð�(s)
    /// </summary>
    public float MercyInvincibleTime => _mercyInvincibleTime;

    [SerializeField]
    [Tooltip("ȸ�� �� ���� ��� �ð�(s) - ������")]
    private float _dodgeCooldown = 1f;
    /// <summary>
    /// ȸ�� �� ���� ��� �ð�(s)
    /// </summary>
    public float DodgeCooldown => _dodgeCooldown;

    [SerializeField]
    [Tooltip("ȸ�� �� ���� �ð�(s) - ������")]
    private float _dodgeInvincibleTime = 1f;
    /// <summary>
    /// ȸ�� �� ���� �ð�(s)
    /// </summary>
    public float DodgeInvincibleTime => _dodgeInvincibleTime;
}
