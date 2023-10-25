using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using QT.Core;
using QT.Util;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 타겟 타임스케일
    // Param2: 지속시간
    public class TimeScaleItemEffect : ItemEffect
    {
        private readonly int CyberEffectOpacityHash = Shader.PropertyToID("_Opacity");
        private const string CyberEffectPath = "Effect/Prefabs/FX_Active_Cyber.prefab";
        
        private static Stat CurrentTimeScale = new (1);
        
        private static readonly float DefaultFixedDeltaTime = Time.fixedDeltaTime;
        
        private readonly StatModifier _targetTimeScale;
        private readonly int _duration;

        private CancellationTokenSource _cancellationTokenSource;
        
        private SkeletonGhost _ghostEffect;
        
        public TimeScaleItemEffect(Item item, Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(item, player, effectData, specialEffectData)
        {
            _targetTimeScale = new StatModifier( specialEffectData.Param1, StatModifier.ModifierType.Multiply, this);
            //_duration = (int) (specialEffectData.Param2 / _targetTimeScale * 1000);
            _duration = (int) (specialEffectData.Param2 * 1000);

            _ghostEffect = player.GhostEffect;
        }

        public override void OnEquip()
        {
        }

        public override void OnTrigger(bool success)
        {
            if (!success)
            {
                return;
            }
            
            CurrentTimeScale.RemoveModifier(_targetTimeScale);
            CurrentTimeScale.AddModifier(_targetTimeScale);
            
            _ghostEffect.ghostingEnabled = true;
            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData
                .ActiveTimeStopSFX);
            SystemManager.Instance.SoundManager.PlaySFX(SystemManager.Instance.SoundManager.SoundData
                .ActiveTimeStopPauseSFX);
            UpdateTimeScale();
            Timer();
        }

        private async UniTaskVoid Timer()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            
            var cyberEffect = await SystemManager.Instance.ResourceManager.GetFromPool<Renderer>(CyberEffectPath, _ghostEffect.transform);
            cyberEffect.transform.ResetLocalTransform();
            
            var opacity = 0f;
            DOTween.To(() => opacity, x =>
            {
                opacity = x;
                cyberEffect.material.SetFloat(CyberEffectOpacityHash, opacity);
            }, 0.5f, 0.5f).SetUpdate(true);


            await UniTask.Delay(_duration, cancellationToken: _cancellationTokenSource.Token);
            
            DOTween.To(() => opacity, x =>
            {
                opacity = x;
                cyberEffect.material.SetFloat(CyberEffectOpacityHash, opacity);
            }, 0f, 0.2f).SetUpdate(true);;
            
            SystemManager.Instance.ResourceManager.ReleaseObjectWithDelay(CyberEffectPath, cyberEffect, 0.5f);
            
            OnRemoved();
        }

        public override void OnRemoved()
        {
            _ghostEffect.ghostingEnabled = false;
            //_ghostEffect.ClearGhosting();
            SystemManager.Instance.SoundManager.StopSFX(SystemManager.Instance.SoundManager.SoundData
                .ActiveTimeStopPauseSFX);
            CurrentTimeScale.RemoveModifier(_targetTimeScale);
            UpdateTimeScale();

            _cancellationTokenSource?.Cancel();
        }

        private void UpdateTimeScale()
        {
            Time.timeScale = CurrentTimeScale;
            Time.fixedDeltaTime = DefaultFixedDeltaTime * Time.timeScale;
        }
    }
}
