using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Sound;
using UnityEngine;

namespace QT.InGame
{
    [FSMState((int) Dullahan.States.Throw)]
    public class DullahanThrowState : FSMState<Dullahan>
    {
        private readonly int ThrowAnimHash = Animator.StringToHash("Throw");

        private List<EnemyAtkGameData> _atkList;
        
        private SoundManager _soundManager;

        public DullahanThrowState(IFSMEntity owner) : base(owner)
        {
            _atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(_ownerEntity.DullahanData.ThrowAtkId);
        }

        public override void InitializeState()
        {
            _soundManager = SystemManager.Instance.SoundManager;

            _ownerEntity.Rigidbody.velocity = Vector2.zero;
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            
            Vector2 dir = (SystemManager.Instance.PlayerManager.Player.transform.position - _ownerEntity.transform.position);
            var side = _ownerEntity.SetDir(dir,2);
            _ownerEntity.Shooter.ShootPoint = _ownerEntity.CenterTransform;
            
            _ownerEntity.StartCoroutine(AttackSequence());
        }

        public override void ClearState()
        {
            _ownerEntity.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private IEnumerator AttackSequence()
        {
            foreach (var data in _atkList)
            {  
                _ownerEntity.Animator.SetTrigger(ThrowAnimHash);
                yield return new WaitForSeconds(data.BeforeDelay);

                _soundManager.PlayOneShot(_soundManager.SoundData.Boss_Throw, _ownerEntity.transform.position);

                _ownerEntity.Shooter.Shoot(data.ShootDataId, data.AimType, ProjectileOwner.Boss);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }

            _ownerEntity.ChangeState(Dullahan.States.Normal);
        }
    }
}