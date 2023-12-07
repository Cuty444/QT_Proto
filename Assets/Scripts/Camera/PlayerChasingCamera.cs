using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
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
    [SerializeField] private Volume _globalVolume;
    
    
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _damageIntensity = 0.4f;
    [SerializeField] private float _damagePlayTime = 0.45f;
    
    #endregion
    
    private Vignette _vignette;
    
    private Color _vignetteColor;
    private float _vignetteIntensity;
    
    private void Awake()
    {
        SystemManager.Instance.EventManager.AddEvent(this, InvokeEvent);
        
        SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
        SystemManager.Instance.PlayerManager.OnMapCellChanged.AddListener(OnMapCellChanged);

        SetVignette();
    }

    private void OnDestroy()
    {
        if (SystemManager.Instance == null)
        {
            return;
        }
        SystemManager.Instance.EventManager.RemoveEvent(this);
        
        SystemManager.Instance.PlayerManager.PlayerCreateEvent.AddListener(PlayerCreateEvent);
        SystemManager.Instance.PlayerManager.OnMapCellChanged.AddListener(OnMapCellChanged);
    }

    private void InvokeEvent(TriggerTypes triggerTypes, object data)
    {
        if (triggerTypes == TriggerTypes.OnDamage)
        {
            PlayDamageEffect();
        }
    }
    
    private void PlayerCreateEvent(Player obj)
    {
        _cinemachineVirtualCamera.Follow = obj.transform;
    }

    private void OnMapCellChanged(VolumeProfile data, float cameraSize)
    {
        _globalVolume.profile = data;
        _cinemachineVirtualCamera.m_Lens.OrthographicSize = cameraSize;

        SetVignette();
    }
    
    private void PlayDamageEffect()
    {
        var duration = _damagePlayTime * 0.5f;
        var sequence = DOTween.Sequence();
        
        var seq = DOTween.Sequence();
        seq.Join(DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
            _damageIntensity, duration).SetEase(Ease.OutBounce));
        
        seq.Join(DOTween.To(() => _vignette.color.value, x => _vignette.color.value = x,
            _damageColor, duration).SetEase(Ease.OutBounce));

        sequence.Append(seq);
        
        seq = DOTween.Sequence();
        
        seq.Join(DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x,
            _vignetteIntensity, duration).SetEase(Ease.OutQuad));
        
        seq.Join(DOTween.To(() => _vignette.color.value, x => _vignette.color.value = x,
            _vignetteColor, duration).SetEase(Ease.OutQuad));
        
        sequence.Append(seq);
        
        sequence.Play();
    }

    private void SetVignette()
    {
        _globalVolume.profile.TryGet(out _vignette);
        
        _vignetteColor = _vignette.color.value;
        _vignetteIntensity = _vignette.intensity.value;
    }
    
    public void SetTarget(Transform target)
    {
        _cinemachineVirtualCamera.Follow = target;
    }
    
}