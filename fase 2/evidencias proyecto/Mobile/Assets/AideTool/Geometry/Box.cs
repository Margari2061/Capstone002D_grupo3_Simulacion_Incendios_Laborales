using UnityEngine;

namespace AideTool.Geometry
{
    public struct Box
    {
        public Vector3 Origin { get; private set; }
        public Vector3 HalfExtents { get; private set; }
        public Quaternion Rotation { get; private set; }

        public Vector3 LocalFrontTopLeft { get; private set; }
        public Vector3 LocalFrontTopRight { get; private set; }
        public Vector3 LocalFrontBottomLeft { get; private set; }
        public Vector3 LocalFrontBottomRight { get; private set; }
        public Vector3 LocalBackTopLeft { get { return -LocalFrontBottomRight; } }
        public Vector3 LocalBackTopRight { get { return -LocalFrontBottomLeft; } }
        public Vector3 LocalBackBottomLeft { get { return -LocalFrontTopRight; } }
        public Vector3 LocalBackBottomRight { get { return -LocalFrontTopLeft; } }

        public Vector3 FrontTopLeft { get { return LocalFrontTopLeft + Origin; } }
        public Vector3 FrontTopRight { get { return LocalFrontTopRight + Origin; } }
        public Vector3 FrontBottomLeft { get { return LocalFrontBottomLeft + Origin; } }
        public Vector3 FrontBottomRight { get { return LocalFrontBottomRight + Origin; } }
        public Vector3 BackTopLeft { get { return LocalBackTopLeft + Origin; } }
        public Vector3 BackTopRight { get { return LocalBackTopRight + Origin; } }
        public Vector3 BackBottomLeft { get { return LocalBackBottomLeft + Origin; } }
        public Vector3 BackBottomRight { get { return LocalBackBottomRight + Origin; } }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
            Rotation = orientation;
        }

        public Box(Vector3 origin, Vector3 halfExtents)
        {
            Origin = origin;
            HalfExtents = halfExtents;
            Rotation = Quaternion.identity;
            LocalFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            LocalFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            LocalFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            LocalFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        }

        public void Rotate(Quaternion orientation)
        {
            LocalFrontTopLeft = RotatePointAroundPivot(LocalFrontTopLeft, Vector3.zero, orientation);
            LocalFrontTopRight = RotatePointAroundPivot(LocalFrontTopRight, Vector3.zero, orientation);
            LocalFrontBottomLeft = RotatePointAroundPivot(LocalFrontBottomLeft, Vector3.zero, orientation);
            LocalFrontBottomRight = RotatePointAroundPivot(LocalFrontBottomRight, Vector3.zero, orientation);
        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }
}
