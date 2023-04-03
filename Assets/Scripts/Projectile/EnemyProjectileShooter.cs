using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class EnemyProjectileShooter : ProjectileShooter
    {
        private Coroutine _coroutine = null;
        
        public void PlayEnemyAtkSequence(int atkDataId, bool canOverlap = false)
        {
            var atkList = SystemManager.Instance.DataManager.GetDataBase<EnemyAtkGameDataBase>().GetData(atkDataId);  
            if (atkList == null)
            {
                return;
            }

            if (!canOverlap && _coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(AttackSequence(atkList));
        }

        private IEnumerator AttackSequence(List<EnemyAtkGameData> atkList)
        {
            foreach (var data in atkList)
            {
                yield return new WaitForSeconds(data.BeforeDelay);
                
                Shoot(data.ShootDataId, data.AimType);
                
                yield return new WaitForSeconds(data.AfterDelay);
            }
            
        }
    }
}
