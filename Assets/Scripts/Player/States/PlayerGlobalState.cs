using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.Ranking;
using QT.UI;
using Spine;
using Spine.Unity;
using UnityEngine.UI;
using EventType = QT.Core.EventType;

namespace QT.InGame
{
    [FSMState((int)Player.States.Global, false)]
    public class PlayerGlobalState : FSMState<Player>
    {
        private readonly int GainAnimHash = Animator.StringToHash("Gain");
        private readonly int RotatationAnimHash = Animator.StringToHash("Rotation");
        private readonly int RigidAnimHash = Animator.StringToHash("Rigid");

        private PlayerHPCanvas _playerHpCanvas;
        private RankingManager _rankingManager;

        private InputAngleDamper _roationDamper = new (5);

        private GlobalData _globalData;

        private Bone _batBone;
        private Stat _swingRadStat;
        
        private InputFloatDamper _swingRadDamper = new ();


        public PlayerGlobalState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _playerHpCanvas.gameObject.SetActive(true);
            _rankingManager = SystemManager.Instance.RankingManager;
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            
            _batBone = _ownerEntity.GetComponentInChildren<SkeletonRenderer>().Skeleton.FindBone("bat_size");
            _swingRadStat = _ownerEntity.StatComponent.GetStat(PlayerStats.SwingRad);
        }

        public override void InitializeState()
        {
            SystemManager.Instance.EventManager.AddEvent(this, InvokeEvent);
            SystemManager.Instance.PlayerManager.AddItemEvent.AddListener(GainAnimation);
            _ownerEntity.OnAim.AddListener(OnAim);
            
            _swingRadDamper.ResetCurrentValue(_swingRadStat.Value);
        }

        public override void UpdateState()
        {
            _playerHpCanvas.SetHpUpdate(_ownerEntity.StatComponent.GetStatus(PlayerStats.HP));
            _rankingManager.RankingDeltaTimeUpdate.Invoke(Time.deltaTime);
            
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
            _ownerEntity.OnAim.RemoveListener(OnAim);
            
            _swingRadStat.OnValueChanged.RemoveListener(OnSwingRadChange);
        }

        private void InvokeEvent(EventType eventType, object data)
        {
            if(eventType == EventType.OnDamage)
            {
                var damageData = ((Vector2 dir, float damage)) data;
                OnDamage(damageData.dir, damageData.damage);
            }
        }


        private void OnAim(Vector2 aimPos)
        {
            var aimDir = ((Vector2) _ownerEntity.transform.position - aimPos).normalized;

            if (_ownerEntity.IsReverseLookDir)
            {
                aimDir *= -1;
            }
            
            float flip = 180;
            if (_ownerEntity.IsDodge)
            {
                flip = _ownerEntity.IsFlip ? 180f : 0f;
                _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
                return;
            }
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
            angle = _roationDamper.GetDampedValue(angle, Time.deltaTime);
            
            if (angle < 0)
            {
                angle += 360;
            }

            _ownerEntity.EyeTransform.rotation = Quaternion.Euler(0, 0, angle + 270);

            if (angle > 180)
            {
                angle = 360 - angle;
                flip = 0;
            }

            var aimValue = angle / 180 * 5;
            
            _ownerEntity.Animator.SetFloat(RotatationAnimHash, aimValue);
            _ownerEntity.Animator.transform.rotation = Quaternion.Euler(0f, flip, 0f);
        }
        
        private void OnDamage(Vector2 dir, float damage)
        {
            _ownerEntity.Animator.SetTrigger(RigidAnimHash);
            //_ownerEntity.PlayerHitEffectPlay();
            
            var hp = _ownerEntity.StatComponent.GetStatus(PlayerStats.HP);
            hp.AddStatus(-damage);
            _playerHpCanvas.CurrentHpImageChange(hp);

            _ownerEntity.DamageImpulseSource.GenerateImpulse(dir * _ownerEntity.DamageImpulseForce);
            
            if (hp <= 0)
            {
                _ownerEntity.PlayerDead();
            }
            else
            {
                _ownerEntity.MaterialChanger.SetHitMaterial();
            }
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
        }

    }

}