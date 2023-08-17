using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public partial class Player
    {
        private readonly int AnimationSwingHash = Animator.StringToHash("PlayerSwing");
        
        public MeshFilter SwingAreaMeshFilter { get; private set; }
        public MeshRenderer SwingAreaMeshRenderer { get; private set; }
        
        public void SetBatActive(bool isActive)
        {
            _batSpriteRenderer.enabled = isActive;
        }
        
        public void PlayBatAnimation()
        {
            float halfAngle = StatComponent.GetStat(PlayerStats.SwingCentralAngle) * 0.5f;
            float startAngle, endAngel;
            float rotationSpeed = 1;
            
            if (PlayerSwingAngle())
            {
                startAngle = -90.0f - halfAngle;
                endAngel = -90.0f + halfAngle;
                _batSpriteRenderer.flipX = true;
               swingSlashEffectPlay(true);
            }
            else
            {
                startAngle = -90.0f + halfAngle;
                endAngel = -90.0f - halfAngle;
                rotationSpeed = - 1;
                _batSpriteRenderer.flipX = false;
               swingSlashEffectPlay(false);
            }
            
            _batTransform.transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
            rotationSpeed *= Mathf.DeltaAngle(_batTransform.localEulerAngles.z, endAngel) / 0.1f;
            StartCoroutine(BatAnimation(_batTransform, rotationSpeed, endAngel));
            
        }

        private IEnumerator BatAnimation(Transform targetTransform, float rotateSpeed, float targetAngle)
        {
            _batSpriteRenderer.enabled = true;
            Animator.SetTrigger(AnimationSwingHash);
            
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
            
            _batSpriteRenderer.enabled = false;
        }
        
        
        private bool PlayerSwingAngle()
        {
            float playerRotation = EyeTransform.rotation.z;
            switch (playerRotation)
            {
                case > 0.35f and < 0.7f:
                    _batSpriteRenderer.sortingOrder = 21;
                    return false;
                case > 0.7f and < 0.95f:
                    _batSpriteRenderer.sortingOrder = 21;
                    return true;
                case > 0.95f:
                case < -0.95f:
                    _batSpriteRenderer.sortingOrder = 21;
                    return true;
                case > -0.95f and < -0.7f:
                    _batSpriteRenderer.sortingOrder = 20;
                    return false;
                case > -0.7f and < -0.35f:
                    _batSpriteRenderer.sortingOrder = 20;
                    return true;
                default:
                    _batSpriteRenderer.sortingOrder = 20;
                    return false;
            }
        }

        private bool PlayerFlipAngle()
        {
            Vector3 playerEulerAngles = EyeTransform.rotation.eulerAngles;
            float playerRotation = playerEulerAngles.z;

            switch (playerRotation)
            {
                case > 90f and < 270f:
                    return true;
                default:
                    return false;
            }
        }
    }
}