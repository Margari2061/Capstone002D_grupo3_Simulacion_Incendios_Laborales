using UnityEngine;
using UnityEngine.InputSystem;

namespace AideTool.Input.InputSystem
{
    public sealed class InputButton : IButton
    {
        private bool m_value;
        private bool m_trigger;
        private bool m_changed;

        public override ButtonState State 
        {
            get 
            {
                if (m_trigger && !m_changed && m_value)
                {
                    m_changed = true;
                    return ButtonState.Pressed;
                }

                if (m_trigger && m_changed && m_value)
                {
                    if (HoldAction)
                        HeldTime += Time.deltaTime;

                    return ButtonState.Holding;
                }

                if (!m_trigger && m_changed && !m_value)
                {
                    m_changed = false;

                    if (HoldTrigger)
                    {
                        HeldTime = 0f;
                        return ButtonState.Held;
                    }

                    HeldTime = 0f;
                    return ButtonState.Released;
                }

                return ButtonState.Free;
            }
            protected set { }
        }

        public InputButton(float holdTime = 0f) : base(holdTime) 
        {
            m_value = false;
            m_trigger = false;
            m_changed = false;
        }

        public void SetValues(InputAction.CallbackContext context)
        {
            m_value = context.ReadValue<float>() != 0f;
            m_trigger = context.action.triggered;
        }
    }
}
