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
        private const string BossEndEffectPath = "Effect/Prefabs/Boss/FX_Sunising_Sun.prefab";
        private const string BossDeadEffectPath = "Effect/Prefabs/Boss/FX_Boss_Dead.prefab";
        
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
            
            _ownerEntity.SetPhysics(false);
            
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
            
            _ownerEntity.MapData.BossWave.Kill();
            
            PlayerPrefs.SetInt(Constant.ProgressDataKey, (int)Progress.Clear);
        }

        public override void UpdateState()
        {
            _time += Time.deltaTime;
            
            switch (_state)
            {
                case 0:
                    if (_time > 0.5f)
                    {
                        SystemManager.Instance.UIManager.SetState(UIState.None);
                        _state++;
                    }
                    break;
                
                case 1:
                    if (_time > 4.5f)
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
                
                case 2:
                    if (_time > 3)
                    {
                        PlayEnding();
                        _state++;
                    }
                    break;
            }
        }

        public override void ClearState()
        {
            _ownerEntity.SetPhysics(true);
            _ownerEntity.Animator.SetBool(IsDeadAnimHash, false);
        }

        private async void PlayEnding()
        {
            SystemManager.Instance.SoundManager.PlayBGM(SystemManager.Instance.SoundManager.SoundData.ClearBGM);
            
            var videoCanvas = await SystemManager.Instance.UIManager.Get<EndingVideoCanvas>();
            videoCanvas.OnFinish += () =>
            {
                SystemManager.Instance.UIManager.Show<CreditCanvasModel>();
            };
            videoCanvas.Show();
        }
    }
}