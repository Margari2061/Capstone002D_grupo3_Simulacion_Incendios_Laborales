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
}
