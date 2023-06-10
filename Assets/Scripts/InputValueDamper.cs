using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Core
{
    public abstract class InputValueDamper<T>
    {
        protected T current;
        protected T velocity;

        public abstract T GetDampedValue(T target, float deltaTime);
        
        public void ResetCurrentValue(T value)
        {
            current = value;
        }
    }
    
    public class InputVector2Damper : InputValueDamper<Vector2>
    {
        private readonly float dampTime = 10;

        public InputVector2Damper(float dampTime)
        {
            this.dampTime = dampTime;
        }
        
        public InputVector2Damper()
        {
        }
        
        public override Vector2 GetDampedValue(Vector2 target, float deltaTime)
        {
            current = Vector2.SmoothDamp(current, target, ref velocity, dampTime * deltaTime);

            return current;
        }
    }
}
