using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace QT
{
    public class Explosion : MonoBehaviour
    {
        private const string ExplosionEffectPath = "Effect/Prefabs/FX_Active_Boom.prefab";
        
        public static async void MakeExplosion(Vector2 position, IHitAble ignore = null)
        {
            Debug.LogWarning(position);
            var exp = await SystemManager.Instance.ResourceManager.GetFromPool<Explosion>(ExplosionEffectPath);
            
            exp.transform.position = position;
            exp.Bomb(ignore);

            var duration = exp.GetComponent<ParticleSystem>().main.duration;

            SystemManager.Instance.ResourceManager.ReleaseObjectWithDelay(ExplosionEffectPath, exp, duration);
        }
        
        
        [SerializeField] private float _range;
        [SerializeField] private int _damage;
        
        public void Bomb(IHitAble ignore = null)
        {
            Vector2 pos = transform.position;
            
            var hitAbles = new List<IHitAble>();
            HitAbleManager.Instance.GetInRange(pos, _range, ref hitAbles);
            foreach (var hit in hitAbles)
            {
                if (hit != ignore)
                {
                    hit.Hit(hit.Position - pos, _damage);
                }
            }
        }
    }
}
