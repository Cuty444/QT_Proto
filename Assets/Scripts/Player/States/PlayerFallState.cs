using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using QT.InGame;
using QT.UI;
using QT.Util;
using UnityEngine;

namespace QT
{
    [FSMState((int)Player.States.Fall)]
    public class PlayerFallState : FSMState<Player>
    {
        private readonly int DodgeLayer = LayerMask.NameToLayer("PlayerDodge");
        private readonly int PlayerLayer = LayerMask.NameToLayer("Player");
        
        private readonly int AnimationFallHash = Animator.StringToHash("PlayerFall");
        private readonly int AnimationFallEndHash = Animator.StringToHash("PlayerFallEnd");
        
        private PlayerHPCanvas _playerHpCanvas;
        private AnimationCurve _scaleCurve;
        private LayerMask _playerLayer;

        public PlayerFallState(IFSMEntity owner) : base(owner)
        {
            _playerHpCanvas = SystemManager.Instance.UIManager.GetUIPanel<PlayerHPCanvas>();
            _scaleCurve = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.EnemyFallScaleCurve;

        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetTrigger(AnimationFallHash);
            _ownerEntity.StartCoroutine(ScaleReduce());
            
            _ownerEntity.gameObject.layer = DodgeLayer;
        }

        private IEnumerator ScaleReduce()
        {
            float time = 0f;
            while (time < 1f)
            {
                float scale = Mathf.Lerp(0, 1, _scaleCurve.Evaluate(time / 1f));
                _ownerEntity.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                time += Time.deltaTime;
            }

            yield return new WaitForSeconds(0.5f);
            _ownerEntity.Animator.SetTrigger(AnimationFallEndHash);
            
            var hp = _ownerEntity.StatComponent.GetStatus(PlayerStats.HP);
            hp.AddStatus(-25);
            
            _playerHpCanvas.CurrentHpImageChange(hp);
            if (hp <= 0)
            {
                _ownerEntity.PlayerDead();
            }
            else
            {
                _ownerEntity.ChangeState(Player.States.Move);
                _ownerEntity.MaterialChanger.SetHitMaterial();
            }
        }
        public override void ClearState()
        {
            _ownerEntity.gameObject.layer = _playerLayer;
            _ownerEntity.Animator.ResetTrigger(AnimationFallHash);
            _ownerEntity.StartCoroutine( Util.UnityUtil.WaitForFunc(() =>
            {
                _ownerEntity.Animator.ResetTrigger(AnimationFallEndHash);

            },0.2f));

            _ownerEntity.StatComponent.GetStatus(PlayerStats.MercyInvincibleTime).SetStatus(0);

            _ownerEntity.transform.localScale = new Vector3(1f, 1f, 1f);
            _ownerEntity.transform.position = _ownerEntity.LastSafePosition;
            
            _ownerEntity.gameObject.layer = PlayerLayer;
        }
    }
}
