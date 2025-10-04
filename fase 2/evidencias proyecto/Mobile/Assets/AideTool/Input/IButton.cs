using UnityEngine;

namespace AideTool.Input
{
    public abstract class IButton
    {
        private readonly float m_holdActionTime;
        protected bool HoldAction => m_holdActionTime > 0f;
        public float HeldTime { get; protected set; }
        protected bool HoldTrigger => HoldAction && HeldTime > m_holdActionTime;
        public virtual ButtonState State { get; protected set; }

        protected IButton(float holdTime)
        {
            m_holdActionTime = holdTime;
        }

        protected void HandleState(bool value)
        {
            if(State == ButtonState.Free && value)
            {
                State = ButtonState.Pressed;
                return;
            }

            if (State == ButtonState.Pressed && value)
            {
                State = ButtonState.Holding;

                if (HoldAction)
                    HeldTime += Time.deltaTime;

                return;
            }

            if(State == ButtonState.Holding && !value)
            {
                if(HoldAction && HoldTrigger)
                {
                    State = ButtonState.Held;
                    HeldTime = 0f;
                    return;
                }

                State = ButtonState.Released;
                HeldTime = 0f;
                return;
            }

            State = ButtonState.Free;
        }

        public bool IsFree => State == ButtonState.Free;
        public bool IsPressed => State == ButtonState.Pressed;
        public bool IsHolding => State == ButtonState.Holding;
        public bool WasHeld => State == ButtonState.Held;
        public bool IsReleased => State == ButtonState.Released;

        public override string ToString() => $"InputButton State: {State}";
    }
}
