using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;

namespace QT
{
    public class AltarDustEffect : MonoBehaviour
    {
        
        private const string AltarDustEffectPath = "Effect/Prefabs/FX_Altar.prefab";

        public void Dust()
        {
            SystemManager.Instance.ResourceManager.EmitParticle(AltarDustEffectPath, transform.position);
        }
    }
}
