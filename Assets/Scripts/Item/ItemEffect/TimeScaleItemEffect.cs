using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        public TimeScaleItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _targetTimeScale = new StatModifier( specialEffectData.Param1, StatModifier.ModifierType.Multiply, this);
            //_duration = (int) (specialEffectData.Param2 / _targetTimeScale * 1000);
            _duration = (int) (specialEffectData.Param2 * 1000);
        }

        public override void OnEquip()
        {
            _lastTime = 0;
        }

        protected override void OnTriggerAction()
        {
            CurrentTimeScale.RemoveModifier(_targetTimeScale);
            CurrentTimeScale.AddModifier(_targetTimeScale);

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
