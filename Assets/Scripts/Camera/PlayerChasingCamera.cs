using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using UnityEngine;

public class PlayerChasingCamera : MonoBehaviour
{
    #region Inspector_Definition

    [SerializeField] private float _minDistance = 0.5f; // �ּ� �Ÿ�
    [SerializeField] private float _maxDistance = 5f; // �ִ� �Ÿ�
    [SerializeField] private float _moveSpeed = 10f;

    #endregion

    #region Global_Declaration

    private Transform _player;

    #endregion

    private void Start()
    {
        _player = null;
        PlayerSystem playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
        playerSystem.PlayerCreateEvent.AddListener((obj) => { _player = obj.transform; });
        playerSystem.OnPlayerCreate();
    }

    private void FixedUpdate()
    {
        if (_player == null)
            return;
        ChasingCamera();
    }

    private void ChasingCamera()
    {
        Vector2 midPoint;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = _player.position;
        Angle(mousePos);

        // ī�޶�� �÷��̾��� ���� �Ÿ�
        float currentDistance = Vector2.Distance(mousePos, playerPos) / 2.5f;
        float t = Mathf.Clamp01((currentDistance - _minDistance) / (_maxDistance - _minDistance));

        // ���� �Ÿ��� �ּ� �Ÿ����� ������ �ּ� �Ÿ��� ����
        if (currentDistance < _minDistance)
        {
            midPoint = Vector2.zero;
        }
        // ���� �Ÿ��� �ִ� �Ÿ����� ũ�� �ִ� �Ÿ��� ����
        else if (currentDistance > _maxDistance)
        {
            midPoint = (mousePos - playerPos).normalized * (_maxDistance * t);
        }
        else // �Ÿ��� �ּ�~�ִ� ���� �ȿ� ������ ���� �Ÿ��� ����
        {
            midPoint = (mousePos - playerPos) / 2.5f * t;
        }

        // midpoint_as_v3 ���
        Vector3 midPoint_as_V3 = new Vector3(midPoint.x, midPoint.y, -10);

        // ī�޶� ��ġ ����
        transform.position = Vector3.Lerp(transform.position, _player.position + midPoint_as_V3,
            Time.fixedDeltaTime * _moveSpeed);
    }

    private void Angle(Vector2 mousePos) //���� ���
    {
        float playerAngleDegree = QT.Util.Math.GetDegree(_player.position, mousePos);
        _player.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
    }
}