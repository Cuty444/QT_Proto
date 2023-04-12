using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Player;
using UnityEngine;

public class PlayerChasingCamera : MonoBehaviour
{
    #region Inspector_Definition

    [SerializeField] private float _minDistance = 0.5f; // 최소 거리
    [SerializeField] private float _maxDistance = 5f; // 최대 거리
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _cameraSakeDiameter = 1f;
    [SerializeField] private bool _isChasing;
    #endregion

    #region Global_Declaration

    private Transform _player;
    private SpriteRenderer _spriteRenderer;
    private PlayerAttack _playerAttack;
    private Transform _playerEyeTransform;

    private Vector3 _beforePosition;

    private bool _isCameraShaking;
    #endregion

    private void Start()
    {
        _player = null;
        PlayerManager playerManager = SystemManager.Instance.PlayerManager;
        playerManager.PlayerCreateEvent.AddListener((obj) => { _player = obj.transform;
            _spriteRenderer = _player.GetComponent<SpriteRenderer>();
            _playerAttack = _player.GetComponent<PlayerAttack>();
            if (_playerAttack == null)
            {
                _playerEyeTransform = _player.GetComponentsInChildren<Transform>()[1];
            }
        });
        playerManager.OnPlayerCreate();
        playerManager.BatSwingTimeScaleEvent.AddListener(CameraShaking);
        _beforePosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (_player == null)
            return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Angle(mousePos);

         if (_isChasing)
         {
             ChasingCamera(mousePos);
         }
    }

     private void LateUpdate()
     {
         if (!_isChasing)
         {
             //transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
             if(_isCameraShaking)
             {
                 transform.position = UnityEngine.Random.insideUnitSphere * _cameraSakeDiameter + transform.position;
             }
             else
             {
                 transform.position = _beforePosition;
             }
         }
     }

    private void ChasingCamera(Vector2 mousePos)
    {
        Vector2 midPoint;
        Vector2 playerPos = _player.position;

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
        //Debug.Log(playerAngleDegree);
        switch (playerAngleDegree)
        {
            case > 45.0f and < 135.0f:
                //_spriteRenderer.sprite = _playerSprites[0];
                break;
            case > -135.5f and < -45.0f:
                //_spriteRenderer.sprite = _playerSprites[1];
                break;
            case > 135.0f:
            case < -135.0f:
                //_spriteRenderer.sprite = _playerSprites[2];
                break;
            default:
                //_spriteRenderer.sprite = _playerSprites[3];
                break;
        }

        if (_playerAttack == null)
        {
            _playerEyeTransform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
            
        }
        else
        {
            _playerAttack.EyeTransform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
        }
    }

    private void CameraShaking(bool isCheck)
    {
        _isCameraShaking = isCheck;
    }

}