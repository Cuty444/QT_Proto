using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using QT.Core;
using QT.Util;
using QT.Sound;

namespace QT.InGame
{
    [FSMState((int) Saddy.States.SideStep)]
    public class SaddySideStepState : FSMState<Saddy>
    {
        private enum StepState
        {
            Side1,
            Side2,
            End
        }
        
        private static readonly int MoveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private static readonly int IsDodgeAnimHash = Animator.StringToHash("IsDodge");
        private static readonly int ChargeLevelAnimHash = Animator.StringToHash("SwingLevel");
        private static readonly int AttackAnimHash = Animator.StringToHash("Attack");

        private List<EnemyAtkGameData> _atkList;
        private SaddyData _data;
        
        private Transform _targetTransform;
        private SoundManager _soundManager;
        
        private StepState _stepState;

        private Vector2 _dir;

        private float _timer;
        private float _stepTime;

        private float _speed;
        private bool _isEnd;

        private bool _side;
        

        public SaddySideStepState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.SaddyData.ThrowAtkId);
            _data = _ownerEntity.SaddyData;
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;
            _targetTransform = SystemManager.Instance.PlayerManager.Player.transform;
            
            _ownerEntity.Animator.SetInteger(ChargeLevelAnimHash, 0);

            _side = Random.value >= 0.5f;
            _stepState = StepState.Side1;
            SetNextState();
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;

            if (!_isEnd)
            {
                if (_timer > _stepTime)
                {
                    if (_stepState == StepState.End)
                    {
                        _ownerEntity.Animator.SetInteger(ChargeLevelAnimHash, 1);
                        Shoot(false);
                        
                        _ownerEntity.ChangeState(Saddy.States.Normal);
                        return;
                    }
                    
                    Shoot(true);

                    _speed = 0;
                    _timer = 0;
                    _isEnd = true;
                }
                else
                {
                    var progress = _timer / _stepTime;
                    progress *= progress * progress; // easeInQuad
                    
                    _speed = Mathf.Lerp(_data.SideStepSpeed, 0, progress);
                    _ownerEntity.Animator.SetFloat(MoveSpeedAnimHash, 1 + progress);
                }
            }
            else
            {
                if (_timer > _data.SideStepAtkDelay)
                {
                    _stepState += 1;
                    SetNextState();
                }
            }
        }

        public override void FixedUpdateState()
        {
            _ownerEntity.Rigidbody.velocity = _dir * (_speed * _data.SideStepSpeed);
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, false);
        }

        private void SetNextState()
        {
            _dir = GetDir();
                    
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, true);
            _ownerEntity.SetDir(_dir,2);
                    
            _stepTime = GetStepTime();
            _timer = 0;
            _isEnd = false;
        }
        
        private void Shoot(bool focus)
        {
            if (focus)
            {
                _dir = (_targetTransform.position - _ownerEntity.transform.position).normalized;
            }
            _ownerEntity.SetDir(_dir,4);
                        
            _ownerEntity.Animator.SetBool(IsDodgeAnimHash, false);
            _ownerEntity.Shooter.PlayEnemyAtkSequence(GetAtkIndex(), ProjectileOwner.Boss);
                        
            _speed = 0;
        }
        
        private float GetStepTime()
        {
            switch (_stepState)
            {
                case StepState.Side1:
                case StepState.Side2:
                    return _data.SideStepTime;
            }
            return _data.SideStepEndTime;
        }

        private Vector2 GetDir()
        {
            Vector2 dir = (_targetTransform.position - _ownerEntity.transform.position).normalized;
            
            switch (_stepState)
            {
                case StepState.Side1:
                case StepState.Side2:
                    _side = !_side;
                    dir = Math.Rotate90Degree(dir, _side);
                    break;
            }

            return dir;
        }
        
        private int GetAtkIndex()
        {
            switch (_stepState)
            {
                case StepState.Side1:
                case StepState.Side2:
                    return _data.SideStepAtkId;
            }

            return _data.SideStepEndAtkId;
        }

    }
}