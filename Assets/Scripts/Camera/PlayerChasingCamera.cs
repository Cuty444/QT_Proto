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
using EventType = QT.Core.EventType;

[Serializable]
public class VolumData
{
    public Color VolumeColor;
    [Range(0f, 1f)] public float Intensity;
    [Range(0f, 1f)] public float Smooothness;
}

public class PlayerChasingCamera : MonoBehaviour
{
    #region Inspector_Definition

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    
    [SerializeField] private VolumData[] _volumDatas;
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private float _vignetteTimeScale;
    #endregion

    #region Global_Declaration

    private float _maxIntensity;

    private Vignette _vignette;

    private Coroutine _vignetteCoroutine = null;
    #endregion

    private void Awake()
    {
        SystemManager.Instance.EventManager.AddEvent(this, InvokeEvent);
        
        var playerManager = SystemManager.Instance.PlayerManager;
        playerManager.PlayerCreateEvent.AddListener((obj) =>
        {
            _cinemachineVirtualCamera.Follow = obj.transform;
        });
        playerManager.OnMapCellChanged.AddListener((data, cameraSize) =>
        {
            _globalVolume.profile = data;
            _globalVolume.profile.TryGet(out _vignette);
            _cinemachineVirtualCamera.m_Lens.OrthographicSize = cameraSize;
        });
        _globalVolume.profile.TryGet(out _vignette);
        _maxIntensity = _volumDatas[1].Intensity;
    }

    private void OnDestroy()
    {
        SystemManager.Instance?.EventManager.RemoveEvent(this);
    }

    private void InvokeEvent(EventType eventType, object data)
    {
        if (eventType == EventType.OnDamage)
        {
            VignetteOn();
        }
    }
    

    private void Update()
    {
        if (_vignetteCoroutine == null)
        {
            SetVolum(0);
        }
    }

    private void VignetteOn()
    {
        if (_vignetteCoroutine != null)
        {
            return;
        }
        SetVolum(1);
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
        SetVolum(0);
        _vignetteCoroutine = null;
    }

    private void SetVolum(int index)
    {
        _vignette.color.value = _volumDatas[index].VolumeColor;
        _vignette.intensity.value = _volumDatas[index].Intensity;
        _vignette.smoothness.value = _volumDatas[index].Smooothness;
    }

    public void SetTarget(Transform target)
    {
        _cinemachineVirtualCamera.Follow = target;
    }
    
}