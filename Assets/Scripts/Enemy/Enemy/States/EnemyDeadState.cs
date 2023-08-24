using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QT.Core;
using QT.Core.Data;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int)Enemy.States.Dead)]
    public class EnemyDeadState : FSMState<Enemy>
    {
        private static readonly int DeadAnimHash = Animator.StringToHash("IsDead");
        private static readonly AnimationCurve enemyFallScaleCurve = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData.EnemyFallScaleCurve;
        
        public EnemyDeadState(IFSMEntity owner) : base(owner)
        {
        }

        public override void InitializeState()
        {
            _ownerEntity.Animator.SetBool(DeadAnimHash, true);
            _ownerEntity.SetPhysics(false);
            _ownerEntity.BallObject.localPosition = Vector3.up * _ownerEntity.BallHeightMin;
            _ownerEntity.HpCanvas.gameObject.SetActive(false);
            PlayerManager _playerManager = SystemManager.Instance.PlayerManager;

            var gold = _ownerEntity.RandomGoldDrop();
            if (gold > 0)
            {
                _playerManager.OnGoldValueChanged.Invoke(gold);
            }

            SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Coin_GetSFX);
            _ownerEntity.ShadowSprite.DOFade(0, 1).SetEase(Ease.InQuad);
            _ownerEntity.DeadSound();
            
            if (_ownerEntity.Steering.IsStuck())
            {
                _ownerEntity.StartCoroutine(ScaleReduce());
                SystemManager.Instance.SoundManager.PlayOneShot(SystemManager.Instance.SoundManager.SoundData.Monster_WaterDrop);
            }
            
            HitAbleManager.Instance.UnRegister(_ownerEntity);
            ProjectileManager.Instance.UnRegister(_ownerEntity);
        }

        public override void ClearState()
        {
            _ownerEntity.Animator.SetBool(DeadAnimHash, false);
            _ownerEntity.SetPhysics(true);
            _ownerEntity.ShadowSprite.DOFade(1, 1).SetEase(Ease.InQuad);
        }
        
        private IEnumerator ScaleReduce()
        {
            float time = 0f;
            while (time < 1f)
            {
                float scale = Mathf.Lerp(0, 1, enemyFallScaleCurve.Evaluate(time / 1f));
                _ownerEntity.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                time += Time.deltaTime;
            }
        }
    }
}
