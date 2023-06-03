using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using QT.Core;
using QT.Core.Map;
using QT.InGame;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerChasingCamera : MonoBehaviour
{
    #region Inspector_Definition

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private CinemachineConfiner _cinemachineConfiner;

    [SerializeField] private Volume _globalVolume;
    [SerializeField] private float _vignetteTimeScale;
    #endregion

    #region Global_Declaration

    private float _maxIntensity;

    private Vignette _vignette;

    private Coroutine _vignetteCoroutine = null;
    #endregion

    private void Start()
    {
        PlayerManager playerManager = SystemManager.Instance.PlayerManager;
        playerManager.PlayerDoorEnterCameraShapeChange.AddListener((colliderTemp) =>
        {
            _cinemachineConfiner.m_BoundingShape2D = colliderTemp;
        });
        playerManager.PlayerCreateEvent.AddListener((obj) =>
        {
            _cinemachineVirtualCamera.Follow = obj.transform;
            playerManager.OnDamageEvent.AddListener(VignetteOn);
        });
        SystemManager.Instance.GetSystem<DungeonMapSystem>().DungeonStart();
        _globalVolume.profile.TryGet(out _vignette);
        _maxIntensity = _vignette.intensity.value;
    }

    private void VignetteOn(Vector2 dir, float damage)
    {
        if (_vignetteCoroutine != null)
        {
            return;
        }
        _vignette.active = true;
        _vignetteCoroutine = StartCoroutine(VignetteOff(_vignette,_vignetteTimeScale));
    }

    private IEnumerator VignetteOff(Vignette vignette,float time)
    {
        float currentTime = 0f;
        vignette.intensity.value = 0f;
        float halfTime = time * 0.5f;
        while (halfTime > currentTime)
        {
            currentTime += Time.deltaTime;
            vignette.intensity.value = Unity.Mathematics.math.remap(0f, halfTime, 0f, _maxIntensity, currentTime);
            yield return null;
        }

        currentTime = halfTime;
        
        while (0f <= currentTime)
        {
            currentTime -= Time.deltaTime;
            vignette.intensity.value = Unity.Mathematics.math.remap(0f, halfTime, 0f, _maxIntensity, currentTime);
            yield return null;
        }

        _vignette.active = false;
        _vignetteCoroutine = null;
    }

}