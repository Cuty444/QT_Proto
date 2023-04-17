using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace QT
{
    public class SkeletalMaterialChanger : MonoBehaviour
    {
        private SkeletonRenderer _skeletonRenderer;
        
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private Material _targetMaterial;
        
        private void Awake()
        {
            _skeletonRenderer = GetComponent<SkeletonRenderer>();
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
