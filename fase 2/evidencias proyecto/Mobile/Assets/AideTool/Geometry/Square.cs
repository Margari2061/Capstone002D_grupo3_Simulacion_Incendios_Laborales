using UnityEngine;

namespace AideTool.Geometry
{
    public struct Square
    {
        public Vector2 Origin { get; private set; }
        public Vector2 HalfExtents { get; private set; }

        public Vector2 TopLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }

        public Square(Vector2 origin, Vector2 halfExtents) 
        { 
            Origin = origin;
            HalfExtents = halfExtents;
            TopLeft = origin + new Vector2(-halfExtents.x, halfExtents.y);
            TopRight = origin + new Vector2(halfExtents.x, halfExtents.y);
            BottomRight = origin + new Vector2(halfExtents.x, -halfExtents.y);
            BottomLeft = origin + new Vector2(-halfExtents.x, -halfExtents.y);
        }
    }
}
