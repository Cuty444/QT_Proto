using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using Spine;
using Spine.Unity;

namespace QT.InGame
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private readonly int GainAnimHash = Animator.StringToHash("Gain");
        private readonly int RigidAnimHash = Animator.StringToHash("Rigid");

        private PlayerHPCanvasModel _playerHpCanvas;

        private GlobalData _globalData;

        private Bone _batBone;
        private Bone _batEffectBone;
        private Stat _swingRadStat;
        
        private InputFloatDamper _swingRadDamper = new (100);


        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            
            var skeleton = _ownerEntity.GetComponentInChildren<SkeletonRenderer>().Skeleton;
            _batBone = skeleton.FindBone("bat_size");
            _batEffectBone = skeleton.FindBone("Bat_eff");
            
            _swingRadStat = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingRad);
        }

        public override async void InitializeState()
        {
            _playerHpCanvas = await SystemManager.Instance.UIManager.Get<PlayerHPCanvasModel>();
            
            SystemManager.Instance.EventManager.AddEvent(this, InvokeEvent);
            SystemManager.Instance.PlayerManager.AddItemEvent.AddListener(GainAnimation);
            
            var swingRad = _swingRadStat.Value / _swingRadStat.BaseValue * _globalData.BatSizeMultiplier;
            _swingRadDamper.ResetCurrentValue(swingRad);
            
            HitAbleManager.Instance.Register(_ownerEntity);
        }

        public override void UpdateState()
        {
            _playerHpCanvas.UpdateInfo(_ownerEntity.StatComponent.GetStatus(PlayerStats.HP), _ownerEntity.Inventory.ActiveItem);
            
            OnSwingRadChange();
        }

        public override void ClearState()
        {
            if (SystemManager.Instance == null)
            {
                return;
            }
            
            SystemManager.Instance.EventManager.RemoveEvent(this);
            SystemManager.Instance.PlayerManager.AddItemEvent.RemoveListener(GainAnimation);
            
            HitAbleManager.Instance.UnRegister(_ownerEntity);
        }

        private void InvokeEvent(TriggerTypes triggerTypes, object data)
        {
            if (triggerTypes == TriggerTypes.OnDamage)
            {
                var damageData = ((Vector2 dir, float damage)) data;
                OnDamage(damageData.dir, damageData.damage);
            }
            else if(triggerTypes == TriggerTypes.OnHeal)
            {
                OnHeal((float) data);
            }
        }


        
        private void OnDamage(Vector2 dir, float damage)
        {
            var statComponent = _ownerEntity.StatComponent;
            statComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
            
            _ownerEntity.Animator.SetTrigger(RigidAnimHash);
            //_ownerEntity.PlayerHitEffectPlay();
            
            _ownerEntity.DamageImpulseSource.GenerateImpulse(dir * _ownerEntity.DamageImpulseForce);
            
            if (statComponent.GetStatus(PlayerStats.HP) <= 0)
            {
                _ownerEntity.PlayerDead();
            }
            else
            {
                _ownerEntity.MaterialChanger.SetHitMaterial();
            }
        }

        private void OnHeal(float amount)
        {
            var hp = _ownerEntity.StatComponent.GetStatus(PlayerStats.HP);
            hp.AddStatus(amount);
        }

        private void GainAnimation()
        {
            _ownerEntity.Animator?.SetTrigger(GainAnimHash);
        }

        private void OnSwingRadChange()
        {
            var swingRad = _swingRadStat.Value / _swingRadStat.BaseValue * _globalData.BatSizeMultiplier;
            swingRad = _swingRadDamper.GetDampedValue(swingRad, Time.deltaTime);
            
            _batBone.ScaleX = swingRad;
            _batBone.ScaleY = swingRad;
            
            _batEffectBone.ScaleX = swingRad * 2;
            _batEffectBone.ScaleY = swingRad * 2;
        }

    }

}