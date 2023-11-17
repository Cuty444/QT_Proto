using Cinemachine;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Jello.States.Global, false)]
    public class JelloGlobalState : FSMState<Jello>
    {
        private static readonly int HitAnimHash = Animator.StringToHash("Hit");
        
        private BossHPCanvasModel _hpCanvas;
        
        private JelloData _data;

        private float _rigidTime;
        private float _timer;
        private bool _isRigid;

        private float _lastMaxHp;

        public JelloGlobalState(IFSMEntity owner) : base(owner)
        {
            _data = _ownerEntity.JelloData;
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
            _ownerEntity.OnHealEvent.AddListener(OnHeal);
            
            _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
        }
        
        public override async void InitializeState()
        {
            _hpCanvas = await SystemManager.Instance.UIManager.Get<JelloBossHPCanvas>();
            _hpCanvas.SetHPGuage(_ownerEntity.HP);
            _hpCanvas.Show();

            _lastMaxHp = _ownerEntity.HP.Value;
            
            SystemManager.Instance.UIManager.Show<JelloIntroVidioCanvas>();
        }

        public override void ClearState()
        {
            _ownerEntity.OnDamageEvent.RemoveListener(OnDamage);
            _ownerEntity.OnHealEvent.RemoveListener(OnHeal);
            
            _hpCanvas?.ReleaseUI();
        }

        private void OnDamage(Vector2 dir, float power,AttackType attackType)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Jello.States.Dead)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);

            _hpCanvas.SetHPGuage(_ownerEntity.HP);
            
            foreach (var changer in _ownerEntity.MaterialChanger)
            {
                changer.SetHitMaterial();
            }
            
            // _ownerEntity.Rigidbody.velocity = Vector2.zero; 
            // _ownerEntity.Rigidbody.AddForce(-dir, ForceMode2D.Impulse);
            
            _ownerEntity.Animator.SetTrigger(HitAnimHash);
            
            _isRigid = true;
            _timer = 0;
        }

        private void OnHeal(float amount)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Jello.States.Dead)
            {
                return;
            }
            
            _ownerEntity.HP.AddStatus(amount);
            _hpCanvas.SetHPGuage(_ownerEntity.HP);
        }

        public override void UpdateState()
        {
            if (!_isRigid)
            {
                return;
            }
            
            _timer += Time.deltaTime;
            
            if(_rigidTime < _timer)
            {
                _ownerEntity.Animator.ResetTrigger(HitAnimHash);

                var hp = _ownerEntity.HP;

                if (hp <= 0)
                {
                    _ownerEntity.ChangeState(Jello.States.Dead);
                    _hpCanvas?.ReleaseUI();
                }
                else if (IsSplit())
                {
                    _lastMaxHp = hp.StatusValue;
                }
                else if ((_lastMaxHp - _ownerEntity.HP) > (_data.SplitCondition * _ownerEntity.HP.Value))
                {
                    _ownerEntity.ChangeState(Jello.States.Split);
                }
                
                _isRigid = false;
            }
        }
        
        private bool IsSplit()
        {
            if ((Jello.States) _ownerEntity.PreviousStateIndex == Jello.States.Merge)
            {
                return true;
            }
            
            return (Jello.States) _ownerEntity.CurrentStateIndex is >= Jello.States.Split and <= Jello.States.Restore;
        }
    }
}