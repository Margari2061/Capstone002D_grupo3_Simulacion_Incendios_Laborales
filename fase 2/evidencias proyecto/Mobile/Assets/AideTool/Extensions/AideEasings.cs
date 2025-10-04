using System;
using UnityEngine;

namespace AideTool
{
    public static class AideEasings
    {
        public static float Linear(float a, float b, float t)
        {
            t = t.Clamp();
            return a + (b - a) * t;
        }

        public static float LinearTValue(float a, float b, float l)
        {
            if (a == b)
                return l;
            return (l - a) / (b - a);
        }

        public static float Quadratic(float a, float b, float t)
        {
            t = t.Clamp();
            float aX = -1f * (b - a);
            float bX = 2f * (b - a);
            float cX = a;
            float result = aX * Mathf.Pow(t, 2f) + bX * t + cX;

            return result;
        }

        public static float QuadraticTValue(float a, float b, float l) => throw new NotImplementedException();

        public static float Verlet(float previous, float next) => ((2f * previous) + next) * 0.5f;
            
        public static float InSine(float t)
        {
            t = t.Clamp();
            return 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
        }
    }
}
