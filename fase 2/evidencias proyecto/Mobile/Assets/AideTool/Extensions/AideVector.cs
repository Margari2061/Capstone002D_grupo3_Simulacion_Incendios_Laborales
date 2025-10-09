using System;
using UnityEngine;

namespace AideTool
{
    public static class AideVector
    {
        public static float[] ToFloatArray(this Vector3 vector) => new float[] { vector.x, vector.y, vector.z };
        
        public static Vector2 ToXZVector2(this Vector3 vector) => new(vector.x, vector.z);
        
        public static Vector3 ToXZVector3(this Vector2 vector, float y) => new(vector.x, y, vector.y);

        public static Vector3 ToXZVector3(this Vector2 vector) => new(vector.x, 0f, vector.y);
        
        public static Vector3 ToXZVector3(this Vector3 vector, float y) => new(vector.x, y, vector.z);
        
        public static Vector3 ToXZVector3(this Vector3 vector) => ToXZVector3(vector, 0f);

        public static Vector2 ToVector2(this Vector3 vector) => (Vector2)vector;

        public static Vector3 Interpolate(this Vector3 vector, Func<float, float> xPredicate, Func<float, float> yPredicate, Func<float, float> zPredicate)
        {
            float x = xPredicate.Invoke(vector.x);
            float y = yPredicate.Invoke(vector.y);
            float z = zPredicate.Invoke(vector.z);

            return new Vector3(x, y, z);
        }
    }
}
