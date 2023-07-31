using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace QT.Core
{
    public interface IFSMEntity
    {

    }

    public abstract class FSMPlayer<T> : MonoBehaviour where T : IFSMEntity
    {
        private Dictionary<int, FSMState<T>> _states;

        private FSMState<T> _currentState = null;
        private FSMState<T> _globalState = null;

        public int PreviousStateIndex { get; private set; }
        public int CurrentStateIndex { get; private set; }

        protected virtual void Update()
        {
            _globalState?.UpdateState();
            _currentState?.UpdateState();
        }

        protected virtual void FixedUpdate()
        {
            _globalState?.FixedUpdateState();
            _currentState?.FixedUpdateState();
        }

        protected void SetUp(ValueType firstState)
        {
            _states = new Dictionary<int, FSMState<T>>();
            var stateTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(FSMState<T>) != t && typeof(FSMState<T>).IsAssignableFrom(t));

            foreach (var stateType in stateTypes)
            {
                var attribute = stateType.GetCustomAttribute<FSMStateAttribute>();

                if (attribute == null || !attribute.IncludeStates)
                {
                    continue;
                }

                var state = Activator.CreateInstance(stateType, this as IFSMEntity) as FSMState<T>;

                if (!_states.TryAdd(attribute.Key, state))
                {
                    Debug.LogError($"{typeof(T)} 의 {attribute.Key} 키가 중복되었습니다.");
                }
            }

            ChangeState(firstState);
        }

        public FSMState<T> ChangeState(ValueType enumValue)
        {
            if (!_states.TryGetValue((int)enumValue, out var state))
            {
                Debug.LogError($"{GetType()} : 사용할 수 없는 상태입니다. {enumValue}");
                return null;
            }

            if (state == null || state == _currentState)
            {
                return _currentState;
            }

            if (_currentState != null)
            {
                _currentState.ClearState();

                //Debug.Log($"{this.GetType()} : {_currentState.GetType()} 상태 클리어");
            }

            PreviousStateIndex = CurrentStateIndex;
            CurrentStateIndex = (int)enumValue;
            
            _currentState = state;
            _currentState.InitializeState();

            //Debug.Log($"{this.GetType()} : {state.GetType()} 상태로 전환");

            return _currentState;
        }

        
        public void SetGlobalState(FSMState<T> state)
        {
            _globalState?.ClearState();
            
            _globalState = state;

            _globalState?.InitializeState();
        }

        public void RevertToPreviousState()
        {
            ChangeState(PreviousStateIndex);
        }
    }
}