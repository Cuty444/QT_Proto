using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class DeathDust : MonoBehaviour
    {
        private const string PlayerDeathDustPath = "Effect/Prefabs/FX_P_Death_Dust.prefab";
        private const string PlayerDeathSmallDustPath = "Effect/Prefabs/FX_P_Death_S_Dust.prefab";
        [SerializeField] private Transform playerTransform;

        public void DeathDustCreate()
        {
            SystemManager.Instance.ResourceManager.EmitParticle(PlayerDeathDustPath, playerTransform.position);
        }

        public void DeathSmallDustCreate()
        {
            SystemManager.Instance.ResourceManager.EmitParticle(PlayerDeathSmallDustPath, playerTransform.position);
        }
    }
}
