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

        public void SetBatActive(bool isActive)
        {
            _batSpriteRenderer.enabled = isActive;
        }
        
        public void PlayBatAnimation()
        {
            _playerManager.ChargeAtkPierce = QT.Data.ChargeAtkPierce.None;
            
            float halfAngle = SwingCentralAngle.Value * 0.5f;
            float startAngle, EndAngel;
            float rotationSpeed = 1;
            
            if (PlayerSwingAngle())
            {
                startAngle = -90.0f + halfAngle;
                EndAngel = -90.0f - halfAngle;
                rotationSpeed = - 1;
            }
            else
            {
                startAngle = -90.0f - halfAngle;
                EndAngel = -90.0f + halfAngle;
            }
            
            _batTransform.transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
            rotationSpeed *= Mathf.DeltaAngle(_batTransform.localEulerAngles.z, EndAngel) / 0.1f;
            StartCoroutine(BatAnimation(_batTransform, rotationSpeed, EndAngel));
            
        }
        
        private IEnumerator BatAnimation(Transform targetTransform, float rotateSpeed, float targetAngle)
        {
            _trailRenderer.emitting = true;
            _batSpriteRenderer.enabled = true;
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