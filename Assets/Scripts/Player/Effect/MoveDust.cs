using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class MoveDust : MonoBehaviour
    {
        private const string MoveDustEffectPath = "Effect/Prefabs/FX_P_Move_Dust.prefab";
        [SerializeField] private Transform playerTransform;
        public void LeftDust()
        {
            SystemManager.Instance.ResourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
        }

        public void RightDust()
        {
            SystemManager.Instance.ResourceManager.EmitParticle(MoveDustEffectPath, playerTransform.position);
        }
    }
}
