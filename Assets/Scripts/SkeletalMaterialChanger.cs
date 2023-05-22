using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace QT
{
    public class SkeletalMaterialChanger : MonoBehaviour
    {
        private static readonly int MainTexture = Shader.PropertyToID("_MainTex");
        
        private SkeletonRenderer _skeletonRenderer;
        
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private Material _targetMaterial;

        private void Awake()
        {
            _skeletonRenderer = GetComponent<SkeletonRenderer>();

            _targetMaterial = new Material(_targetMaterial);
            _targetMaterial.SetTexture(MainTexture, _baseMaterial.GetTexture(MainTexture));
        }
        
        public void ChangeMaterial()
        {
            _skeletonRenderer.CustomMaterialOverride.Add(_baseMaterial, _targetMaterial);
        }
        
        public void ClearMaterial()
        {
            _skeletonRenderer.CustomMaterialOverride.Clear();
        }
    }
}
