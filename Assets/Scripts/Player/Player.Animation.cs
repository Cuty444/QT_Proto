using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        private Animator _animator;
        private const string _animatorValue = "MouseRotate";
        public void AngleAnimation() //각도 계산
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float playerAngleDegree = QT.Util.Math.GetDegree(transform.position, mousePos);
            Debug.Log(playerAngleDegree);
            switch (playerAngleDegree)
            {
                case > 22.5f and < 67.5f:
                    _animator.transform.rotation = Quaternion.Euler(0f, 180f,0f);
                    _animator.SetFloat(_animatorValue,3f);
                    //_spriteRenderer.sprite = _playerSprites[0];
                    break;
                case > 67.5f and < 112.5f:
                    _animator.transform.rotation = quaternion.identity;
                    _animator.SetFloat(_animatorValue,4f);
                    //_spriteRenderer.sprite = _playerSprites[0];
                    break;
                case > 112.5f and < 157.5f:
                    _animator.transform.rotation = quaternion.identity;
                    _animator.SetFloat(_animatorValue,3f);
                    //_spriteRenderer.sprite = _playerSprites[0];
                    break;
                case > -157.5f and < -112.5f:
                    //_spriteRenderer.sprite = _playerSprites[1];
                    _animator.transform.rotation = quaternion.identity;
                    _animator.SetFloat(_animatorValue,1f);
                    break;
                case > -112.5f and < -67.5f:
                    _animator.transform.rotation = quaternion.identity;
                    _animator.SetFloat(_animatorValue,0f);
                    //_spriteRenderer.sprite = _playerSprites[1];
                    break;
                case > -67.5f and < -22.5f:
                    _animator.transform.rotation = Quaternion.Euler(0f, 180f,0f);
                    _animator.SetFloat(_animatorValue,1f);
                    //_spriteRenderer.sprite = _playerSprites[1];
                    break;
                case > 157.5f:
                case < -157.5f:
                    _animator.transform.rotation = quaternion.identity;
                    _animator.SetFloat(_animatorValue,2f);
                    //_spriteRenderer.sprite = _playerSprites[2];
                    break;
                default:
                    _animator.transform.rotation = Quaternion.Euler(0f, 180f,0f);
                    _animator.SetFloat(_animatorValue,2f);
                    //_spriteRenderer.sprite = _playerSprites[3];
                    break;
            }

            //if (_playerAttack == null)
            //{
            //    _playerEyeTransform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
            //
            //}
            //else
            //{
            //    _playerAttack.EyeTransform.rotation = Quaternion.Euler(0, 0, playerAngleDegree);
            //}
        }
    }
}
