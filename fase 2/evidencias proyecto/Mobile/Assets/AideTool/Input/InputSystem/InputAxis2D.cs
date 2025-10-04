using UnityEngine;
using UnityEngine.InputSystem;

namespace AideTool.Input.InputSystem
{
    public sealed class InputAxis2D : IAxis2D
    {
        public override bool Released => false;
        //{
        //    get
        //    {
        //        if (m_triggered)
        //        {
        //            m_triggered = false;
        //            return true;
        //        }
        //        return false;
        //    }
        //}

        public InputAxis2D() : base() { }

        public InputAxis2D(float minDeadzone) : base(minDeadzone) { }

        public InputAxis2D(float minDeadzoneX, float minDeadzoneY) : base(minDeadzoneX, minDeadzoneY) { }

        public void SetValues(InputAction.CallbackContext context)
        {
            Vector2 axis = context.ReadValue<Vector2>();
            X = SetAxis(axis.x, MinDeadzoneX);
            Y = SetAxis(axis.y, MinDeadzoneY);
            
            //if (context.action.triggered && axis == Vector2.zero)
            //    m_triggered = true;
        }

        private float SetAxis(float value, float deadzone)
        {
            float absValue = Mathf.Abs(value);
            if (absValue > deadzone)
                return value;
            return 0f;
        }
    }
}