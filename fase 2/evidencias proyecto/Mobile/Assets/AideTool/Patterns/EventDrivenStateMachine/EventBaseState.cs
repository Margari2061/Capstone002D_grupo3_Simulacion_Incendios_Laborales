using System;

namespace AideTool.Patterns
{
    public class EventBaseState
    {
        public event Action OnEnterState;
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnLateUpdate;
        public event Action OnExitState;

        public void EnterState() => OnEnterState?.Invoke();
        public void Update() => OnUpdate?.Invoke();
        public void FixedUpdate() => OnFixedUpdate?.Invoke();
        public void LateUpdate() => OnLateUpdate?.Invoke();
        public void ExitState() => OnExitState?.Invoke();

        public void Dispose()
        {
            OnEnterState = null;
            OnUpdate = null;
            OnFixedUpdate = null;
            OnLateUpdate = null;
            OnExitState = null;
        }
    }
}
