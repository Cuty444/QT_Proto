using System.Timers;
using Cinemachine;
using QT.Core;
using QT.Sound;
using QT.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Dead)]
    public class DullahanDeadState : FSMState<Dullahan>
    {
        private readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");
        private const string BossEndEffectPath = "Effect/Prefabs/FX_Sunising_Sun.prefab";
        private const string BossDeadEffectPath = "Effect/Prefabs/FX_Boss_Dead.prefab";
        
        private float _time;
        private int _state;
        
        private SoundManager _soundManager;
        
        public DullahanDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            
            _time = 0;
            _state = 0;
            
            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, true);

            SystemManager.Instance.ResourceManager.EmitParticle(BossEndEffectPath, _ownerEntity.CenterTransform.position);
            _ownerEntity.DeadImpulseSource.GenerateImpulse(1);
            
            _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Dead, _ownerEntity.transform.position);
            
            var camera = GameObject.FindObjectOfType<PlayerChasingCamera>();
            camera.SetTarget(_ownerEntity.CenterTransform);

            HitAbleManager.Instance.UnRegister(_ownerEntity);
            
            foreach (var changer in _ownerEntity.MaterialChanger)
            {
                changer.SetRigidMaterial();
            }
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;
            
            switch (_state)
            {
                case 0:
                    if (_time > 5)
                    {
                        SystemManager.Instance.ResourceManager.EmitParticle(BossDeadEffectPath,
                            _ownerEntity.CenterTransform.position);
                        _ownerEntity.ExplosionImpulseSource.GenerateImpulse(2);
                        
                        _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Landing,
                            _ownerEntity.transform.position);
                        
                        _state++;
                        _time = 0;
                        
                        foreach (var changer in _ownerEntity.MaterialChanger)
                        {
                            changer.ClearMaterial();
                        }
                    }
                    break;
                
                case 1:
                    if (_time > 5)
                    {
                        SystemManager.Instance.UIManager.GetAndShow<EndingCanvasModel>();
                        _state++;
                    }
                    break;
            }
            
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, false);
        }
    }
}