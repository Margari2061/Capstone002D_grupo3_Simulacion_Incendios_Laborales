using System;

namespace AideTool
{
    public static class AideMath
    {
        /// <summary>
        /// Returns true or false if a number is between two other numbers
        /// </summary>
        /// <param name="value">Number to be inspected</param>
        /// <param name="min">Lowest number in range of inspection</param>
        /// <param name="max">Highest number in range of inspection</param>
        /// <returns></returns>
        public static bool Between(this float value, float min, float max)
        {
            if(value >= min && value <= max) 
                return true;
            return false;
        }

        /// <summary>
        /// Returns true or false if a number is between two other numbers
        /// </summary>
        /// <param name="value">Number to be inspected</param>
        /// <param name="min">Lowest number in range of inspection</param>
        /// <param name="max">Highest number in range of inspection</param>
        /// <returns></returns>
        public static bool Between(this int value, int min, int max)
        {
            if(value >= min && value <= max) 
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the difference between two numbers is near zero
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="offset">How near the difference between numbers can be for the result to be true</param>
        /// <returns></returns>
        public static bool MoreOrLess(float a, float b, float offset)
        {
            float result = Math.Abs(a - b);
            if (result <= offset)
                return true;
            return false;
        }

        /// <summary>
        /// Returns a number between two numbers
        /// </summary>
        /// <param name="value">Value to return</param>
        /// <param name="min">Lowest number to return</param>
        /// <param name="max">Highest number to return</param>
        /// <returns></returns>
        public static float Clamp (this float value, float min = 0f, float max = 1f)
        {
            if (value < min)
                return min;
            if (value > max) 
                return max;
            return value;
        }

        public static int Clamp(this int value, int min = 0, int max = 1)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float Roulette(this float value, float min, float max, float offset)
        {
            if (MoreOrLess(value, min, offset))
                return max;
            if (MoreOrLess(value, max, offset))
                return min;
            return value;
        }
        
        public static float Roulette(this float value, float min, float max)
        {
            if (value < min)
                return max;
            if (value > max)
                return min;
            return value;
        }

        public static int Roulette(this int value, int min, int max)
        {
            if (value < min)
                return max;
            if (value > max)
                return min;
            return value;
        }
    }
}
