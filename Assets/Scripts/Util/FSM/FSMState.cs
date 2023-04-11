using System;


namespace QT.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FSMStateAttribute : System.Attribute
    {
        public readonly int Key;
        public readonly bool IncludeStates;

        public FSMStateAttribute(int key, bool includeStates = true)
        {
            this.Key = key;
            this.IncludeStates = includeStates;
        }
    }


    public abstract class FSMState<T> where T : IFSMEntity
    {
        protected readonly T _ownerEntity;

        public virtual void InitializeState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void ClearState() { }

        public FSMState(IFSMEntity owner)
        {
            _ownerEntity = (T)owner;
        }
    }
}