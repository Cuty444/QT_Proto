using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT.Player
{
    public partial class Player
    {
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }

        private void SwingAreaCreate()
        {
            MeshFilter.mesh = SwingAreaCreateMesh(SwingRadius.Value, SwingCentralAngle.Value, 32);
            MeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            MeshRenderer.material.color = new Color(0f, 0f, 1f, 0.2f);
            MeshRenderer.enabled = false;
        }
        
        private Mesh SwingAreaCreateMesh(float radius, float angle, int segments)
        {
            Mesh mesh = new Mesh();
            int vertexCount = segments + 2;
            int indexCount = segments * 3;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] indices = new int[indexCount];
            float angleRad = angle * Mathf.Deg2Rad;
            float angleStep = angleRad / segments;
            float currentAngle = -angleRad / 2f;
            vertices[0] = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                vertices[i + 1] = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * radius;
                currentAngle += angleStep;
            }
            for (int i = 0; i < segments; i++)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = i + 2;
            }
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            return mesh;
        }
        
        public void SwingAreaInBallLineDraw(float beforeChargingTime) // 공격 범위내 공의 궤적들 라인 Draw
        {
            if (beforeChargingTime < ChargeTimes[0].Value)
            {
                foreach(KeyValuePair<Projectile,PlayerLineDrawer> val in lineRendererDictionary) // TODO : 이 부분 추후에 오브젝트 풀 시스템으로 바꾸기
                {
                    val.Value.LineRenderer.positionCount = 0;
                }
                return;
            }

            Vector2 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int bounce = (int)ChargeBounceCount[0].Value;
            for (int j = ChargeTimes.Length - 1; j >= 0; j--)
            {
                if (ChargeTimes[j].Value < beforeChargingTime)
                {
                    bounce = (int)ChargeBounceCount[j].Value;
                    break;
                }
            }
            for (int i = 0; i < ProjectTileList.Count; i++)
            {
                if (SwingAreaCollision(transform, ProjectTileList[i].transform))
                {
                    if (!LineProjectTileList.Contains(ProjectTileList[i]))
                    {
                        //if (lineRendererDictionary.ContainsKey(ProjectTileList[i]))
                        //{
                        //    lineRendererDictionary[ProjectTileList[i]].positionCount = 0;
                        //    Destroy(lineRendererDictionary[ProjectTileList[i]].gameObject);
                        //    lineRendererDictionary.Remove(ProjectTileList[i]);
                        //}
                        CreateLine(ProjectTileList[i]);
                        LineProjectTileList.Add(ProjectTileList[i]);
                    }

                    if (lineRendererDictionary.ContainsKey(ProjectTileList[i]))
                    {
                        Vector2 dir = (startPos - (Vector2) ProjectTileList[i].transform.position).normalized;
                        dir = PlayerMouseAngleCorrectionBall(dir, transform, ProjectTileList[i].transform, startPos);

                        RayCastAngleIncidence(ProjectTileList[i],
                            lineRendererDictionary[ProjectTileList[i]].LineRenderer, dir, bounce);
                    }
                }
                else
                {
                    if (LineProjectTileList.Contains(ProjectTileList[i]))
                    {
                        lineRendererDictionary[ProjectTileList[i]].LineRenderer.positionCount = 0;
                        SystemManager.Instance.ResourceManager.ReleaseObject(lineRendererDictionary[ProjectTileList[i]]);
                        lineRendererDictionary.Remove(ProjectTileList[i]);
                        LineProjectTileList.Remove(ProjectTileList[i]);
                    }
                    
                }
            }
        }

        private Vector2 PlayerMouseAngleCorrectionBall(Vector2 dir, Transform playerTransform,
            Transform targetTransform, Vector2 mousePos) // 플레이어와 탄막 사이의 마우스 각도 보정처리
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;

            float pt = Vector2.Distance(playerPos, targetPos);
            float pm = Vector2.Distance(playerPos, mousePos);
            if (pt > pm)
            {
                dir = (mousePos - (Vector2) playerTransform.position).normalized;
            }


            return dir;
        }
        private const string LineRendererPrefabPath = "Prefabs/LineRenderer.prefab";

        private async void CreateLine(Projectile projectile)
        {
            var lineDrawer = await SystemManager.Instance.ResourceManager.GetFromPool<PlayerLineDrawer>(LineRendererPrefabPath,_lineRendersTransform);
            lineDrawer.LineRenderer.positionCount = 0;
            lineRendererDictionary.Add(projectile,lineDrawer);
        }
        
        private void RayCastAngleIncidence(Projectile projectile,LineRenderer lineRenderer,
                Vector2 reflectDirection,int chargeBounceValue) // 레이캐스트 입사각 처리후 반사각 계산
        {
            if (lineRenderer == null)
                return;
            lineRenderer.enabled = true;
            int reflectCount = 1;
            lineRenderer.positionCount = reflectCount;
            float radius = transform.localScale.x * 0.5f;
            lineRenderer.SetPosition(0, projectile.transform.position);
            for (; reflectCount < chargeBounceValue + 2; reflectCount++)
            {
                if (!BallBounceRayCast(lineRenderer, ref reflectDirection, reflectCount,radius))
                {
                    break;
                }
            }
            if (reflectCount > 3)
            {
                float endTime = QT.Util.Math.Remap(3, reflectCount, 0);
                Gradient gradient = new Gradient();
                gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, endTime), new GradientAlphaKey(0.0f, endTime + 0.03f), new GradientAlphaKey(0f, 1f) });
                lineRenderer.colorGradient = gradient;
            }
            else
            {
                Gradient gradient = new Gradient();
                gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1f, 1f) });
                lineRenderer.colorGradient = gradient;
            }
        }

        private bool BallBounceRayCast(LineRenderer lineRenderer, ref Vector2 reflectDirection, int reflectCount, float radius)
        {
            RaycastHit2D hit2D = Physics2D.CircleCast(lineRenderer.GetPosition((lineRenderer.positionCount - 1)),radius ,reflectDirection, Mathf.Infinity,
                (1 << LayerMask.NameToLayer("Wall")));
            if (hit2D.collider != null)
            {
                Vector2 hitPointMinusRadius = hit2D.point + (hit2D.normal * radius);
                reflectDirection = Vector2.Reflect(reflectDirection, hit2D.normal);
                lineRenderer.positionCount = reflectCount + 1;
                lineRenderer.SetPosition(reflectCount, hitPointMinusRadius);
                //lineRenderer.SetPosition(reflectCount, hitPointMinusRadius + reflectDirection);
                return true;
            }

            return false;
        }
        
        
        private bool PlayerMouseAngleCorrectionBallCheck(Transform playerTransform, Transform targetTransform,
            Vector2 mousePos)
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 targetPos = targetTransform.position;
            return Vector2.Distance(playerPos, targetPos) > Vector2.Distance(playerPos, mousePos);
        }
        
        private IEnumerator BatSwing(Transform targetTransform, float rotateSpeed, float targetZ,Vector2 attackDirection,float beforeChargingTime)
        {
            _trailRenderer.emitting = true;
            _batSpriteRenderer.enabled = true;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
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
            BallPositionSwingCheck(attackDirection,beforeChargingTime);
            if (ChargeTimes[ChargeTimes.Length - 1].Value < beforeChargingTime && CollisionProjectTileList.Count > 0)
            {
                _playerSystem.BatSwingTimeScaleEvent.Invoke(true);
                Time.timeScale = 0.1f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.2f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.3f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.5f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 0.75f;
                yield return new WaitForSeconds(0.01f);
                Time.timeScale = 1.0f;
                _playerSystem.BatSwingTimeScaleEvent.Invoke(false);
                CollisionProjectTileList.Clear();
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            _trailRenderer.emitting = false;
            _batSpriteRenderer.enabled = false;

        }

        public void AttackBatSwing(float beforeChargingTime,Action func)
        {
            float rotationSpeed;
            Vector2 attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _playerSystem.ChargeAtkPierce = QT.Data.ChargeAtkPierce.None;
            float halfAngle = SwingCentralAngle.Value * 0.5f;
            float upAtkCentralAngle = -90.0f - halfAngle;
            float downAtkCentralAngle = -90.0f + halfAngle;
            float rotationTime = 0.1f; // TODO : 0.1f = RotationTime 공속 부분 동기화 필요
            if (PlayerSwingAngle())
            {
                _batTransform.transform.localRotation = Quaternion.Euler(0f, 0f, upAtkCentralAngle);
                rotationSpeed = Mathf.DeltaAngle(_batTransform.localEulerAngles.z, downAtkCentralAngle) / rotationTime;
                StartCoroutine(BatSwing(_batTransform, rotationSpeed, downAtkCentralAngle,attackDirection,beforeChargingTime));
            }
            else
            {
                _batTransform.transform.localRotation = Quaternion.Euler(0f, 0f, downAtkCentralAngle);
                rotationSpeed = (Mathf.DeltaAngle(_batTransform.localEulerAngles.z, upAtkCentralAngle) / rotationTime) * -1f;
                StartCoroutine(BatSwing(_batTransform, rotationSpeed, upAtkCentralAngle,attackDirection,beforeChargingTime));
            }
            //_currentSwingCoolTime = 0f;
            func?.Invoke();
            //if (!_chargingBarBackground.gameObject.activeSelf)
            //{
            //    _playerSystem.ChargeAtkShootEvent.Invoke(_atkShootSpd[0]);
            //    _playerSystem.BatSwingRigidHitEvent.Invoke(_swingRigidDmg[0]);
            //    _playerSystem.ChargeBounceValueEvent.Invoke(_chargeBounceValues[0]);
            //}
        }
        
        public void ChargingBatSwing(float beforeChargingTime,Action func)
        {
            AttackBatSwing(beforeChargingTime,func);

            //_chargingBarBackground.gameObject.SetActive(false);
        }

        private void BallPositionSwingCheck(Vector2 AttackDirection,float beforeChargingTime)
        {
            for (int i = 0; i < ProjectTileList.Count; i++)
            {

                if (SwingAreaCollision(transform, ProjectTileList[i].transform))
                {
                    //Vector2 _attackDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 direction = Vector2.zero;
                    if (PlayerMouseAngleCorrectionBallCheck(transform, ProjectTileList[i].transform, AttackDirection))
                    {
                        direction = (AttackDirection - (Vector2)transform.position).normalized;
                        //bulletAngleDegree = QT.Util.Math.GetDegree(transform.position, AttackDirection);
                    }
                    else
                    {
                        direction = (AttackDirection - (Vector2)ProjectTileList[i].transform.position).normalized;
                        //bulletAngleDegree = QT.Util.Math.GetDegree(ProjectTileList[i].transform.position, AttackDirection);
                    }

                    LayerMask layerMask = (1 << LayerMask.NameToLayer("Wall"));
                    float speed = ChargeShootSpd[0].Value;
                    int bounce = (int)ChargeBounceCount[0].Value;
                    int damage = (int) ChargeProjectileDmg[0].Value;
                    bool isChargeAtkPierce = false;
                    for (int j = ChargeTimes.Length - 1; j >= 0; j--)
                    {
                        if (ChargeTimes[j].Value < beforeChargingTime)
                        {
                            speed = ChargeShootSpd[j].Value;
                            bounce = (int)ChargeBounceCount[j].Value;
                            damage = (int) ChargeProjectileDmg[j].Value;
                            if (j != 0)
                                isChargeAtkPierce = true;
                            break;
                        }
                    }

                    if (!isChargeAtkPierce)
                        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
                    //                       이속                                  튕김횟수  대미지            관통여부
                    ProjectTileList[i].Init(speed,0f,0.5f,direction,bounce,damage,true,layerMask);
                    CollisionProjectTileList.Add(ProjectTileList[i]);
                    //if (ChargeAtkPierce.None == _playerSystem.ChargeAtkPierce)
                    //{
                    //    gameObject.layer = LayerMask.NameToLayer("Ball");
                    //}
                    //else if (_globalDataSystem.BatTable.ChargeAtkPierce.HasFlag(_playerSystem.ChargeAtkPierce))
                    //{
                    //    gameObject.layer = LayerMask.NameToLayer("BallHit");
                    //}
                    //else
                    //{
                    //    gameObject.layer = LayerMask.NameToLayer("Ball");
                    //}

                    //_playerSystem.BatSwingBallHitEvent.Invoke();
                }
            }
        }
        
        public bool SwingAreaCollision(Transform originalTransform, Transform targetTransform)
        {
            Vector2 interV = targetTransform.position - originalTransform.position;
            float targetRadius = targetTransform.localScale.x / 2f;
            // target과 나 사이의 거리가 radius 보다 작다면
            if (interV.magnitude <= SwingRadius.Value + targetRadius)
            {
                // '타겟-나 벡터'와 '내 정면 벡터'를 내적
                float dot = Vector2.Dot(interV.normalized, _eyeTransform.right);
                // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
                float theta = Mathf.Acos(dot);
                // angleRange와 비교하기 위해 degree로 변환
                float degree = Mathf.Rad2Deg * theta;

                // 시야각 판별
                if (degree <= SwingCentralAngle.Value / 2f)
                    return true;
            }

            return false;
        }
        
        public void BatSpriteRenderOnOff(bool isActive)
        {
            _batSpriteRenderer.enabled = isActive;
        }
        
        public bool PlayerSwingAngle()
        {
            float playerRotation = _eyeTransform.rotation.z;
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