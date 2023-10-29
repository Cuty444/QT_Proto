using QT.Core;
using QT.InGame;
using UnityEngine;

namespace QT.InGame
{
    //외부적인 요인으로 플레이어의 위치를 조작할 때 사용 (보스 스킬 등)
    [FSMState((int)Player.States.KnockBack)]
    public class PlayerKnockBackState : FSMState<Player>
    {
        private readonly int DodgeLayer = LayerMask.NameToLayer("PlayerDodge");
        private readonly int PlayerLayer = LayerMask.NameToLayer("Player");
        
        private readonly int KnockBackAnimHash = Animator.StringToHash("IsFall");
        
        private LayerMask _playerLayer;

        private Vector2 _targetPos;

        private float _targetTime;
        private float _timer;

        private Vector2 _startPos;
        
        public PlayerKnockBackState(IFSMEntity owner) : base(owner)
        {
        }

        public void InitializeState(Vector2 targetPos, float duration)
        {
            _ownerEntity.Rigidbody.simulated = false;
            _ownerEntity.Animator.SetBool(KnockBackAnimHash, true);
            
            _ownerEntity.gameObject.layer = DodgeLayer;
            _startPos = _ownerEntity.transform.position;
            _targetPos = targetPos;

            _targetTime = duration;
            _timer = 0;
        }

        public override void UpdateState()
        {
            _timer += Time.deltaTime;
            
            var progress = 1 - _timer / _targetTime;
            progress = 1 - progress * progress;

            _ownerEntity.transform.position = Vector2.Lerp(_startPos, _targetPos, progress);

            if (_timer >= _targetTime)
            {
                _ownerEntity.ChangeState(Player.States.Move);
            }
        }
        
        public override void ClearState()
        {
            _ownerEntity.gameObject.layer = _playerLayer;
            _ownerEntity.Rigidbody.simulated = true;
            _ownerEntity.Animator.SetBool(KnockBackAnimHash, false);

            _ownerEntity.StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);
            
            _ownerEntity.gameObject.layer = PlayerLayer;
        }
    }
}
