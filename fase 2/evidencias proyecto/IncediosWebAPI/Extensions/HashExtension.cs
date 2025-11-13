using IncediosWebAPI.Model;
using System.Security.Cryptography;
using System.Text;

namespace IncediosWebAPI.Extensions;

public static class HashExtension
{
    public static string Hash(this string input)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public static string ToNormal(this ResultadosPartida result)
    {
        string text = result.ToString();
        string ret = "";
        int len = text.Length;

        for(int i = 0; i < len; i++)
        {
            if (i == 0)
            {
                ret += text[i];
                continue;
            }

            if (text[i].ToString() == text[i].ToString().ToUpper())
                ret += " ";

            ret += text[i];
        }

        return ret;
    }
}
