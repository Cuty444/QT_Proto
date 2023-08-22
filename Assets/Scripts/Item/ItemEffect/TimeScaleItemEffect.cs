using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1: 타겟 타임스케일
    // Param2: 지속시간
    public class TimeScaleItemEffect : ItemEffect
    {
        private static Stat CurrentTimeScale = new (1);
        
        private static readonly float DefaultFixedDeltaTime = Time.fixedDeltaTime;
        
        private readonly StatModifier _targetTimeScale;
        private readonly int _duration;

        private CancellationTokenSource _cancellationTokenSource;
        
        private SkeletonGhost _ghostEffect;
        
        public TimeScaleItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _targetTimeScale = new StatModifier( specialEffectData.Param1, StatModifier.ModifierType.Multiply, this);
            //_duration = (int) (specialEffectData.Param2 / _targetTimeScale * 1000);
            _duration = (int) (specialEffectData.Param2 * 1000);

            _ghostEffect = player.GhostEffect;
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
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
            
            await UniTask.Delay(_duration, cancellationToken: _cancellationTokenSource.Token);
            
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
