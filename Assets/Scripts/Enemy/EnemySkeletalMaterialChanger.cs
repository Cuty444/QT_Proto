using System;
using System.Collections;
using System.Collections.Generic;
using QT.Core;
using QT.Core.Data;
using UnityEngine;
using Spine.Unity;

namespace QT
{
    public class EnemySkeletalMaterialChanger : MonoBehaviour
    {
        private static readonly int MainTexture = Shader.PropertyToID("_MainTex");
        private static readonly int HitProgress = Shader.PropertyToID("_Progress");
        
        private SkeletonRenderer _skeletonRenderer;
        
        [SerializeField] private Material _baseMaterial;
        
        [SerializeField] private Material _hitMaterial;
        [SerializeField] private Material _rigidMaterial;

        private float _hitDuration = 0.1f;
        private AnimationCurve _hitCurve = new ();

        private GlobalData _globalData;
        private void Awake()
        {
            _globalData = SystemManager.Instance.GetSystem<GlobalDataSystem>().GlobalData;
            
            _hitDuration = _globalData.EnemyHitEffectDuration;
            _hitCurve = _globalData.EnemyHitEffectCurve;
            
            _skeletonRenderer = GetComponent<SkeletonRenderer>();

            var texture = _baseMaterial.GetTexture(MainTexture);
            
            _hitMaterial = new Material(_hitMaterial);
            _hitMaterial.SetTexture(MainTexture, texture);
            
            _rigidMaterial = new Material(_rigidMaterial);
            _rigidMaterial.SetTexture(MainTexture, texture);
        }
        
        public void SetRigidMaterial()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            StopAllCoroutines();
            
            _skeletonRenderer.CustomMaterialOverride.Remove(_baseMaterial);
            _skeletonRenderer.CustomMaterialOverride.Add(_baseMaterial, _rigidMaterial);
        }
        
        public void SetHitMaterial()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            StopAllCoroutines();
            
            _skeletonRenderer.CustomMaterialOverride.Remove(_baseMaterial);
            _skeletonRenderer.CustomMaterialOverride.Add(_baseMaterial, _hitMaterial);
            
            StartCoroutine(Hit());
        }

        private IEnumerator Hit()
        {
            float time = 0;
            
            while (time < _hitDuration)
            {
                _hitMaterial.SetFloat(HitProgress, Mathf.Lerp(0, 1, _hitCurve.Evaluate(time / _hitDuration)));
                
                yield return 0;
                time += Time.deltaTime; 
            }

            ClearMaterial();
        }
        
        
        public void ClearMaterial()
        {
            _skeletonRenderer.CustomMaterialOverride.Clear();
        }

        public void SetHitDuration(float delta)
        {
            _hitDuration = delta;
            _hitCurve = _globalData.PlayerHitEffectCurve;
        }
    }
}
