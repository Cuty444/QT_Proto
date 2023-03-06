using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterTableObject",menuName = "Data/CharacterTable", order = 0)]
public class CharacterTable : ScriptableObject
{
    [SerializeField]
    private int _index = 100;
    public int Index { get => _index; }
    [SerializeField]
    [Tooltip("�ִ� ü��")]
    private int _hpMax = 100;
    public int HPMax { get => _hpMax; }
    [SerializeField]
    [Tooltip("ĳ���� ��Ʈ�ڽ� ���� ��������")]
    private float _pcHitBoxRad = 1f;
    public float PCHitBoxRad { get => _pcHitBoxRad; }
    [SerializeField]
    [Tooltip("�̵� �ӵ�. unit")]
    private float _movementSpd = 1f;
    public float MovementSpd { get => _movementSpd; }
    [SerializeField]
    [Tooltip("�ǰ� �� ���� �ð�(s)")]
    private float _mercyInvincibleTime = 1f;
    public float MercyInvincibleTime { get => _mercyInvincibleTime; }
    [SerializeField]
    [Tooltip("ȸ�� �� ���� ��� �ð�(s)")]
    private float _dodgeCooldown = 1f;
    public float DodgeCooldown { get => _dodgeCooldown; }
    [SerializeField]
    [Tooltip("ȸ�� �� ���� �ð�(ms)")]
    private float _dodgeInvincibleTime = 1f;
    public float DodgeInvincibleTime { get => _dodgeInvincibleTime; }
}
