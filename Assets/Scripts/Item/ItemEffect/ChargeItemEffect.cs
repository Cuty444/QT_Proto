using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    public class ChargeItemEffect : ItemEffect
    {
        private readonly LayerMask WallLayer = LayerMask.GetMask("Wall", "HardCollider");

        private readonly Player _player;
        private readonly Transform _playerTransform;

        private CancellationTokenSource _cancellationTokenSource;

        private float _speed = 10;
        private float _maxSteerAngle = 10;
        
        private bool _isCharging;

        private Vector2 _dir;

        public ChargeItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _playerTransform = player.transform;
        }

        public override void OnEquip()
        {
        }

        protected override void OnTriggerAction()
        {
            _isCharging = true;
            
        }

        public override void OnRemoved()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async UniTaskVoid Charging()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            
            while (true)
            {
                _playerTransform.Translate(_dir * (_speed * Time.deltaTime));
                
                
                await UniTask.NextFrame(_cancellationTokenSource.Token);
                
            }
        }
        
    }
}
