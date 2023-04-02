using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Player;
using QT.Player;
using UnityEngine;

public class PlayerChasingCamera : MonoBehaviour
{
    #region Inspector_Definition

    [SerializeField] private float _minDistance = 0.5f; // 최소 거리
    [SerializeField] private float _maxDistance = 5f; // 최대 거리
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private Sprite[] _playerSprites;
    #endregion

    #region Global_Declaration

    private Transform _player;
    private SpriteRenderer _spriteRenderer;
    private PlayerAttack _playerAttack;
    #endregion

    private void Start()
    {
        _player = null;
        PlayerSystem playerSystem = SystemManager.Instance.GetSystem<PlayerSystem>();
        playerSystem.PlayerCreateEvent.AddListener((obj) => { _player = obj.transform;
            _spriteRenderer = _player.GetComponent<SpriteRenderer>();
            _playerAttack = _player.GetComponent<PlayerAttack>();
        });
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

        // 카메라와 플레이어의 현재 거리
        float currentDistance = Vector2.Distance(mousePos, playerPos) / 2.5f;
        float t = Mathf.Clamp01((currentDistance - _minDistance) / (_maxDistance - _minDistance));

        // 현재 거리가 최소 거리보다 작으면 최소 거리로 조정
        if (currentDistance < _minDistance)
        {
            midPoint = Vector2.zero;
        }
        // 현재 거리가 최대 거리보다 크면 최대 거리로 조정
        else if (currentDistance > _maxDistance)
        {
            midPoint = (mousePos - playerPos).normalized * (_maxDistance * t);
        }
        else // 거리가 최소~최대 범위 안에 있으면 현재 거리로 설정
        {
            midPoint = (mousePos - playerPos) / 2.5f * t;
        }

        // midpoint_as_v3 계산
        Vector3 midPoint_as_V3 = new Vector3(midPoint.x, midPoint.y, -10);

        // 카메라 위치 설정
        transform.position = Vector3.Lerp(transform.position, _player.position + midPoint_as_V3,
            Time.fixedDeltaTime * _moveSpeed);
    }

    private void Angle(Vector2 mousePos) //각도 계산
    {
        float playerAngleDegree = QT.Util.Math.GetDegree(_player.position, mousePos);
        switch (playerAngleDegree)
        {
            case > 45.0f and < 135.0f:
                _spriteRenderer.sprite = _playerSprites[0];
                break;
            case > -135.5f and < -45.0f:
                _spriteRenderer.sprite = _playerSprites[1];
                break;
            case > 135.0f:
            case < -135.0f:
                _spriteRenderer.sprite = _playerSprites[2];
                break;
            default:
                _spriteRenderer.sprite = _playerSprites[3];
                break;
        }
        _playerAttack.EyeTransform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
    }
}