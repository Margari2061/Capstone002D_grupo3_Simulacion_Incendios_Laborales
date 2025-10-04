using AideTool.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace AideTool
{
    public static class AideGizmo
    {
        public static void DrawBox(Box box)
        {
            Gizmos.DrawLine(box.FrontTopLeft, box.FrontTopRight);
            Gizmos.DrawLine(box.FrontTopRight, box.FrontBottomRight);
            Gizmos.DrawLine(box.FrontBottomRight, box.FrontBottomLeft);
            Gizmos.DrawLine(box.FrontBottomLeft, box.FrontTopLeft);

            Gizmos.DrawLine(box.BackTopLeft, box.BackTopRight);
            Gizmos.DrawLine(box.BackTopRight, box.BackBottomRight);
            Gizmos.DrawLine(box.BackBottomRight, box.BackBottomLeft);
            Gizmos.DrawLine(box.BackBottomLeft, box.BackTopLeft);

            Gizmos.DrawLine(box.FrontTopLeft, box.BackTopLeft);
            Gizmos.DrawLine(box.FrontTopRight, box.BackTopRight);
            Gizmos.DrawLine(box.FrontBottomRight, box.BackBottomRight);
            Gizmos.DrawLine(box.FrontBottomLeft, box.BackBottomLeft);
        }

        public static void DrawBoxGuidelinesX(Box box)
        {
            Gizmos.DrawLine(box.Origin + Vector3.left, box.Origin + Vector3.right);
            Gizmos.DrawLine(box.Origin + Vector3.left + box.HalfExtents.y * Vector3.up, box.Origin + Vector3.right + +box.HalfExtents.y * Vector3.up);
            Gizmos.DrawLine(box.Origin + Vector3.left + box.HalfExtents.y * Vector3.down, box.Origin + Vector3.right + +box.HalfExtents.y * Vector3.down);
        }

        public static void DrawSquare(Square square)
        {
            Gizmos.DrawLine(square.TopLeft, square.TopRight);
            Gizmos.DrawLine(square.TopLeft, square.BottomLeft);
            Gizmos.DrawLine(square.TopRight, square.BottomRight);
            Gizmos.DrawLine(square.BottomLeft, square.BottomRight);
        }
    }
}
