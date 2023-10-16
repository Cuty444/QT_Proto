using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace QT
{
    public class DestructibleObject : MonoBehaviour, IHitAble
    {
        public int InstanceId => gameObject.GetInstanceID();
        public Vector2 Position => transform.position;
        [field: SerializeField] public float ColliderRad { get; private set; }
        public bool IsClearTarget => false;
        public bool IsDead => false;
        
        [SerializeField] private  ParticleSystem _effect;
        [SerializeField] private  GameObject _object;
        [SerializeField] private  Transform _fragmentsParent;
        [SerializeField] private bool _isReverse;
        
        private Fragment[] _fragments;
        private Collider2D _collider2D;
        
        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            _fragments = _fragmentsParent.GetComponentsInChildren<Fragment>(true);
        }
        
        private void OnEnable()
        {
            HitAbleManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            HitAbleManager.Instance.UnRegister(this);
            _effect.gameObject.SetActive(false);
        }
        

        public void Hit(Vector2 dir, float power, AttackType attackType)
        {
            if (_object != null)
            {
                _object.SetActive(false);
            }

            if (_effect != null)
            {
                _effect.gameObject.SetActive(true);
                _effect.Play();
            }

            if (_collider2D != null)
            {
                _collider2D.enabled = false;
            }
            
            if (_fragments != null)
            {
                if (_isReverse)
                {
                    dir *= -1;
                }
                
                foreach (var fragment in _fragments)
                {
                    fragment.Hit(dir, power);
                }
            }
            
            HitAbleManager.Instance.UnRegister(this);
        }
        
    }
}
