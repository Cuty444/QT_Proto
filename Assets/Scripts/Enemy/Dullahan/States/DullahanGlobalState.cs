using Cinemachine;
using QT.Core;
using QT.Core.Data;
using QT.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Global, false)]
    public class DullahanGlobalState : FSMState<Dullahan>
    {
        private static readonly int HitAnimHash = Animator.StringToHash("Hit");
        
        private BossHPCanvasModel _hpCanvas;
        
        private float _rigidTime;
        private float _rigidTimer;
        private bool _isRigid;

        public DullahanGlobalState(IFSMEntity owner) : base(owner)
        {
            _ownerEntity.OnDamageEvent.AddListener(OnDamage);
            
            _rigidTime = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.RigidTime;
        }
        
        public override async void InitializeState()
        {
            _hpCanvas = await SystemManager.Instance.UIManager.Get<DullahanBossHPCanvas>();
            _hpCanvas.SetHPGuage(_ownerEntity.HP);
            _hpCanvas.Show();
        }

        public override void ClearState()
        {
            base.ClearState();
            _hpCanvas?.ReleaseUI();
        }

        private void OnDamage(Vector2 dir, float power,AttackType attackType)
        {
            if (_ownerEntity.CurrentStateIndex == (int)Dullahan.States.Dead)
            {
                return;
            }

            _ownerEntity.HP.AddStatus(-power);

            _hpCanvas.SetHPGuage(_ownerEntity.HP);
            
            foreach (var changer in _ownerEntity.MaterialChanger)
            {
                changer.SetHitMaterial();
            }
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero; 
            _ownerEntity.Rigidbody.AddForce(-dir, ForceMode2D.Impulse);
            
            _ownerEntity.Animator.SetTrigger(HitAnimHash);
            
            _isRigid = true;
            _rigidTime = 0;
        }

        public override void UpdateState()
        {
            if (!_isRigid)
            {
                return;
            }
            
            _rigidTime += Time.deltaTime;
            
            if(_rigidTime >= _rigidTimer)
            {
                _ownerEntity.Animator.ResetTrigger(HitAnimHash);

                if (_ownerEntity.HP <= 0)
                {
                    _ownerEntity.ChangeState(Dullahan.States.Dead);
                }

                _isRigid = false;
            }
        }
    }
}