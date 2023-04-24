using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        public MeshFilter SwingAreaMeshFilter { get; private set; }
        public MeshRenderer SwingAreaMeshRenderer { get; private set; }

        public List<Enemy.Enemy> _enemyList;
        
        public void SetBatActive(bool isActive)
        {
            _batSpriteRenderer.enabled = isActive;
        }
        
        public void PlayBatAnimation()
        {
            _playerManager.ChargeAtkPierce = QT.Data.ChargeAtkPierce.None;
            
            float halfAngle = SwingCentralAngle.Value * 0.5f;
            float startAngle, endAngel;
            float rotationSpeed = 1;
            
            if (PlayerSwingAngle())
            {
                startAngle = -90.0f - halfAngle;
                endAngel = -90.0f + halfAngle;
                _batSpriteRenderer.flipX = true;
            }
            else
            {
                startAngle = -90.0f + halfAngle;
                endAngel = -90.0f - halfAngle;
                rotationSpeed = - 1;
                _batSpriteRenderer.flipX = false;
            }
            
            _batTransform.transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
            rotationSpeed *= Mathf.DeltaAngle(_batTransform.localEulerAngles.z, endAngel) / 0.1f;
            StartCoroutine(BatAnimation(_batTransform, rotationSpeed, endAngel));
            
        }
        
        private IEnumerator BatAnimation(Transform targetTransform, float rotateSpeed, float targetAngle)
        {
            _trailRenderer.emitting = true;
            _batSpriteRenderer.enabled = true;
            SetSwingAnimation();
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            float currentRotationTime = 0.0f;
            while (0.1f > currentRotationTime) // TODO : 0.1f = RotationTime 공속 부분 동기화 필요
            {
                targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation,
                    rotateSpeed * Time.deltaTime);
                yield return null;
                currentRotationTime += Time.deltaTime;
            }

            targetTransform.localRotation = Quaternion.RotateTowards(targetTransform.localRotation, targetRotation,
                rotateSpeed * Time.deltaTime);
            
            yield return new WaitForSeconds(0.1f);
            
            _trailRenderer.emitting = false;
            _batSpriteRenderer.enabled = false;
        }
        
        private bool PlayerSwingAngle()
        {
            float playerRotation = EyeTransform.rotation.z;
            switch (playerRotation)
            {
                case > 0.35f and < 0.7f:
                    return false;
                case > 0.7f and < 0.95f:
                    return true;
                case > 0.95f:
                case < -0.95f:
                    return true;
                case > -0.95f and < -0.7f:
                    return false;
                case > -0.7f and < -0.35f:
                    return true;
                default:
                    return false;
            }
        }
    }
}