using System.Linq;

namespace AideTool
{
    public static class AideBool
    {
        public static bool And(this bool[] args) => !args.Contains(false);

        public static bool Or(this bool[] args) => args.Contains(true);

        public static bool AnyNull(this object[] args)
        {
            foreach (object obj in args)
                if (obj == null)
                    return true;
            return false;
        }

        public static bool ToBool(this int i) => i == 1;

        public static int ToInt(this bool b)
        {
            if(b)
                return 1;
            return 0;
        }

        public static string StringifyAsInt(this bool b)
        {
            if (b)
                return "1";
            return "0";
        }

        public static bool TryParseBool(this string str, out bool b)
        {
            b = false;
            str = str
                .Trim()
                .ToLower();

            if (str == "true" || str == "1")
            {
                b = true;
                return true;
            }

            if (str == "false" || str == "0")
                return true;

            return false;
        }
    }
}
