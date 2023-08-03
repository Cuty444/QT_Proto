using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using QT.Core;
using UnityEngine;

namespace QT.InGame
{
    // Param1 : 흡수 
    // Param2 : 공전거리
    // Param3 : 최대 차징 시간
    public class AbsorbItemEffect : ItemEffect
    {
        private LayerMask TargetBounceLayer = LayerMask.GetMask("Player");
        
        private readonly Player _player;
        private readonly float _absorbRange;
        private readonly float _orbitDistance;
        private readonly float _maxChargingDuration;

        private CancellationTokenSource _cancellationTokenSource;
        
        private List<IProjectile> _targets;

        private bool _isCharging;
        private float _chargingTime;

        public AbsorbItemEffect(Player player, ItemEffectGameData effectData, SpecialEffectGameData specialEffectData) : base(player, effectData, specialEffectData)
        {
            _player = player;
            _absorbRange = specialEffectData.Param1;
            _orbitDistance = specialEffectData.Param2;
            _maxChargingDuration = specialEffectData.Param3;
        }

        public override void OnEquip()
        {
            _player.OnActive.AddListener(OnActive);
        }

        protected override void OnTriggerAction()
        {
            _isCharging = true;
            _chargingTime = 0;

            Orbit();
        }

        public override void OnRemoved()
        {
            _player.OnActive.RemoveListener(OnActive);
        }

        
        private async UniTaskVoid Orbit()
        {
            while (_chargingTime < _maxChargingDuration)
            {
                CheckProjectiles();
                
                await UniTask.NextFrame();
            }
        }

        private void OnActive(bool isOn)
        {
            
        }

        private void CheckProjectiles()
        {
            List<IProjectile> projectiles = new List<IProjectile>();
            ProjectileManager.Instance.GetInRange(_player.transform.position, _absorbRange, ref projectiles, TargetBounceLayer);

            foreach (var projectile in projectiles)
            {
                ProjectileManager.Instance.UnRegister(projectile);
                (projectile as MonoBehaviour).enabled = false;
            }
            
            _targets.AddRange(projectiles);
        }
    }
}
