using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core
{
    public abstract class InputValueDamper<T>
    {
        protected float _dampTime = 10;
        
        protected T _current;
        protected T _velocity;

        public abstract T GetDampedValue(T target, float deltaTime);
        
        public void ResetCurrentValue(T value)
        {
            _current = value;
        }
    }
    
    public class InputVector2Damper : InputValueDamper<Vector2>
    {
        public InputVector2Damper(float dampTime)
        {
            _dampTime = dampTime;
        }

        public InputVector2Damper()
        {
            
        }
        
        public override Vector2 GetDampedValue(Vector2 target, float deltaTime)
        {
            _current = Vector2.SmoothDamp(_current, target, ref _velocity, _dampTime * deltaTime);

            return _current;
        }
    }
    
    public class InputFloatDamper : InputValueDamper<float>
    {
        public InputFloatDamper(float dampTime)
        {
            _dampTime = dampTime;
        }

        public InputFloatDamper()
        {
            
        }
        
        public override float GetDampedValue(float target, float deltaTime)
        {
            _current = Mathf.SmoothDamp(_current, target, ref _velocity, _dampTime * deltaTime);

            return _current;
        }
    }
    
    public class InputAngleDamper : InputValueDamper<float>
    {
        public InputAngleDamper(float dampTime)
        {
            _dampTime = dampTime;
        }

        public InputAngleDamper()
        {
            
        }
        
        public override float GetDampedValue(float target, float deltaTime)
        {
            _current = Mathf.SmoothDampAngle(_current, target, ref _velocity, _dampTime * deltaTime);
            _current %= 360;
            
            return _current;
        }
    }
}
