using System.Text;

namespace P3D;

internal static class StringObfuscation
{
    public static String Obfuscate(String s) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(s));

    public static String DeObfuscate(String s) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(s));
}
