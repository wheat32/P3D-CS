using System.Globalization;

namespace P3D;

public static class StringHelper
{
    public static char GetChar(int charCode)
    {
        return Char.ConvertFromUtf32(charCode)[0];
    }

    public static char Tab => GetChar(9);
    public static char LineFeed => GetChar(10);
    public static String CrLf => GetChar(13).ToString() + LineFeed;

    public static bool IsNumeric(Object obj)
    {
        if (obj is String s)
        {
            return IsNumeric(s.InsertDecSeparator());
        }
        return IsNumeric(obj?.ToString() ?? "");
    }

    public static bool IsNumeric(String str)
    {
        return Decimal.TryParse(str, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out _);
    }
}
