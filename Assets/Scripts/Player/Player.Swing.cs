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

        public List<IHitable> _hitableList = new ();
        public Enemy _rigidTeleportEnemy;
        public Enemy _rigidTargetEnemy;
        public void SetBatActive(bool isActive)
        {
            _batSpriteRenderer.enabled = isActive;
        }
        
        public void PlayBatAnimation()
        {
            float halfAngle = GetStat(PlayerStats.SwingCentralAngle) * 0.5f;
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
        
        public bool RigidEnemyCheck()
        {
            foreach (var hitable in _hitableList)
            {
                if (hitable is not Enemy)
                {
                    continue;
                }

                var enemy = (Enemy) hitable;
                
                if (enemy.CurrentStateIndex == (int) Enemy.States.Rigid && enemy.HP <= 0)
                {
                    if (GetStat(PlayerStats.TeleportAllowableDistance) <
                        Vector2.Distance(transform.position, enemy.transform.position))
                    {
                        continue;
                    }
                    _rigidTeleportEnemy = enemy;
                    _rigidTargetEnemy = null;
                    float lowHp = float.MaxValue;
                    foreach (var targetHitable in _hitableList)
                    {
                        if (targetHitable is not Enemy)
                        {
                            continue;
                        }

                        var targetEnemy = (Enemy) targetHitable;
                        
                        if (targetEnemy.CurrentStateIndex < (int) Enemy.States.Projectile)
                        {
                            if (targetEnemy.HP > 0)
                            {
                                float percentageHp = targetEnemy.HP / targetEnemy.HP.BaseValue;
                                if (lowHp > percentageHp)
                                {
                                    lowHp = percentageHp;
                                    _rigidTargetEnemy = targetEnemy;
                                }
                            }
                        }
                    }

                    if (_rigidTargetEnemy != null)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return true;
        }

        // public (List<Enemy>,List<Enemy>) GetRigidEnemyList()
        // {
        //     List<Enemy> rigidList = new List<Enemy>();
        //     List<Enemy> distanceList = new List<Enemy>();
        //     foreach (var enemy in _hitableList)
        //     {
        //         if (enemy.CurrentStateIndex == (int) Enemy.States.Rigid && enemy.HP <= 0)
        //         {
        //             if (GetStat(PlayerStats.TeleportAllowableDistance) <
        //                 Vector2.Distance(transform.position, enemy.transform.position))
        //             {
        //                 rigidList.Add(enemy);
        //             }
        //             else
        //             {
        //                 distanceList.Add(enemy);
        //             }
        //         }
        //     }
        //
        //     return (rigidList,distanceList);
        // }
        
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