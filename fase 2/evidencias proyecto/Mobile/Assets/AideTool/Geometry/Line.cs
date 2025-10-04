using UnityEngine;

namespace AideTool.Geometry
{
    public struct Line
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }
        public bool IsVertical { get { return Start.x == End.x; } }
        public float Slope { get { return (End.y - Start.y) / (End.x - Start.x); } }
        public float Coefficient { get { return (-Slope * Start.x) + Start.y; } }

        public Line(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        public bool Intersects(Line b, out Vector3 point, float tolerance = 0f)
        {
            point = Vector3.zero;
            if (IsParallel(b, tolerance))
                return false;

            float x = DefineX(b);
            float y = DefineY(b, x);

            point = new Vector3(x, y);

            return true;
        }

        private float DefineX(Line b)
        {
            if (IsVertical)
                return Start.x;
            if (b.IsVertical)
                return b.Start.x;

            return (Coefficient - b.Coefficient) / (b.Slope - Slope);
        }

        private float DefineY(Line b, float x)
        {
            if (IsVertical)
                return b.Coefficient + (b.Slope * Start.x);
            if (b.IsVertical)
                return Coefficient + (Slope * b.Start.x);

            return b.Coefficient + (b.Slope * x);
        }

        public bool IsParallel(Line b, float tolerance = 0f)
        {
            if (IsParallelByX(b, tolerance) || IsParallelByY(b, tolerance))
                return true;
            return false;
        }

        public bool IsParallelByX(Line b, float tolerance = 0f)
        {
            if (Mathf.Abs(Start.x - End.x) < tolerance && Mathf.Abs(b.Start.x - b.End.x) < tolerance && Mathf.Abs(Start.x - b.Start.x) < tolerance)
                return true;
            return false;
        }

        public bool IsParallelByY(Line b, float tolerance = 0f)
        {
            if (Mathf.Abs(Start.y - End.y) < tolerance && Mathf.Abs(b.Start.y - b.End.y) < tolerance && Mathf.Abs(Start.y - b.Start.y) < tolerance)
                return true;
            return false;
        }
    }
}