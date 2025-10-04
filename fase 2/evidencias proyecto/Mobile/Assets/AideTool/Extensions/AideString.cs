using System.Collections.Generic;
using System.Text;

namespace AideTool
{
    public static class AideString
    {
        private static readonly Dictionary<char, string> ASCIIToUTF8 = new()
        {
            { 'Á', "&Aacute;" },
            { 'É', "&Eacute;" },
            { 'Í', "&Iacute;" },
            { 'Ó', "&Oacute;" },
            { 'Ú', "&Uacute;" },
            { 'á', "&aacute;" },
            { 'é', "&eacute;" },
            { 'í', "&iacute;" },
            { 'ó', "&oacute;" },
            { 'ú', "&uacute;" },
            { 'Ä', "&Auml;" },
            { 'Ö', "&Ouml;" },
            { 'Ü', "&Uuml;" },
            { 'ä', "&auml;" },
            { 'ö', "&ouml;" },
            { 'ü', "&uuml;" },
            { 'Ñ', "&Ntilde;" },
            { 'ñ', "&ntilde;" },
            { '¡', "&iexcl;" },
            { '¿', "&iquest;" },
            { '\"', "&quot;" },
        };

        public static string ToASCII(string utf8)
        {
            foreach (KeyValuePair<char, string> kvp in ASCIIToUTF8)
            {
                utf8 = utf8.Replace(kvp.Key.ToString(), kvp.Value);
            }

            return utf8;
        }

        public static string ToUTF8(string ascii)
        {
            foreach (KeyValuePair<char, string> kvp in ASCIIToUTF8)
            {
                ascii = ascii.Replace(kvp.Value, kvp.Key.ToString());
            }

            return ascii;
        }

        public static string PascalToNormal(this string str)
        {
            string handleChars(char c, int i)
            {
                if (i == 0)
                    return c.ToString().ToUpper();
                if (char.IsUpper(c))
                    return $" {c}";
                return c.ToString();
            }

            StringBuilder builder = new();
            int len = str.Length;
            for (int i = 0; i < len; i++)
                builder.Append(handleChars(str[i], i));

            return builder.ToString();
        }

        public static string NormalToCamel(this string str)
        {
            string handleChars(char c, int i, int e)
            {
                if (i == 0)
                    return c.ToString().ToLower();
                if (e == 0)
                    return c.ToString().ToUpper();
                return c.ToString();
            }

            StringBuilder builder = new();
            string[] parts = str.Split(" ");
            int partsLen = parts.Length;

            for (int i = 0; i < partsLen; i++)
            {
                int len = parts[i].Length;
                for (int e = 0; e < len; e++)
                    builder.Append(handleChars(parts[i][e], i, e));
            }

            return builder.ToString();
        }

        public static string PascalToCamel(this string str)
        {
            string handleChars(char c, int i)
            {
                if (i == 0)
                    return c.ToString().ToLower();
                return c.ToString();
            }

            StringBuilder builder = new();
            int len = str.Length;
            for (int i = 0; i < len; i++)
                builder.Append(handleChars(str[i], i));
            return builder.ToString();
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            string special = "\\|@#~€¬[]{}ª!\"·$%&/()=?¿^*¨ Ç_:;º'¡`+´ç-.,</>";

            foreach (char c in special)
                str = str.Replace(c.ToString(), "");

            return str;
        }
    }
}
