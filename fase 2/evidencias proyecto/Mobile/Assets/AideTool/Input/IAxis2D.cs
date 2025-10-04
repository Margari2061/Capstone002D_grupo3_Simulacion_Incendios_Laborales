using UnityEngine;

namespace AideTool.Input
{
    public abstract class IAxis2D
    {
        private readonly float m_minDeadzoneX;
        protected float MinDeadzoneX => m_minDeadzoneX;
        
        private readonly float m_minDeadzoneY;
        protected float MinDeadzoneY;

        private float m_lastX;
        private float m_x;
        public float X
        {
            get => m_x;
            protected set
            {
                if(value != 0f)
                    m_lastX = value;
                m_x = value;
            }
        }

        private float m_lastY;
        private float m_y;
        public float Y
        {
            get => m_y;
            protected set
            {
                if (value != 0f)
                    m_lastY = value;
                m_y = value;
            }
        }

        public bool IsHorizontalActive => X != 0f;
        public bool IsVerticalActive => Y != 0f;
        public bool IsActive => IsHorizontalActive || IsVerticalActive;
        public abstract bool Released { get; }

        public Vector2 Axis => new(X, Y);

        public Direction DirectionOnRelease
        {
            get
            {
                if(m_lastX == 0f && m_lastY == 0f)
                    return Direction.None;

                float absX = Mathf.Abs(m_lastX);
                float absY = Mathf.Abs(m_lastY);

                if (absX == absY)
                    return Direction.None;
                
                if(absX > absY)
                {
                    if (m_lastX > 0)
                        return Direction.Right;
                    return Direction.Left;
                }

                if(m_lastY > 0)
                    return Direction.Up;

                return Direction.Down;
            }
        }

        protected IAxis2D() : this(0f) { }

        protected IAxis2D(float minDeadzone) : this(minDeadzone, minDeadzone) { }

        protected IAxis2D(float minDeadzoneX, float minDeadzoneY)
        {
            m_minDeadzoneX = minDeadzoneX;
            MinDeadzoneY = minDeadzoneY;
        }
    }
}
