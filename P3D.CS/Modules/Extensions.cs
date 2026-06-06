using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

internal static class Extensions
{
    public static void Move<T>(this List<T> list, int fromIndex, int toIndex)
    {
        T item = list[fromIndex];
        list.RemoveAt(fromIndex);
        list.Insert(toIndex, item);
    }

    public static Texture2D Copy(this Texture2D t)
    {
        Texture2D newT = new Texture2D(Core.GraphicsDevice, t.Width, t.Height);
        Color[] data = new Color[newT.Width * newT.Height];
        t.GetData(data);
        newT.SetData(data);
        return newT;
    }

    public static String GetSplit(this String s, int index, String separator)
    {
        if (s.Contains(separator) == false)
        {
            return s;
        }
        String[] parts = s.Split([ separator ], StringSplitOptions.None);
        return parts.Length - 1 >= index ? parts[index] : s;
    }

    public static String GetSplit(this String s, int index)
    {
        return s.GetSplit(index, ",");
    }

    public static int CountSplits(this String s, String separator)
    {
        if (s.Contains(separator) == false)
        {
            return 1;
        }
        int count = 0;
        foreach (char c in s)
        {
            if (c == separator[0])
            {
                count++;
            }
        }
        return count + 1;
    }

    public static int CountSplits(this String s)
    {
        return s.CountSplits(",");
    }

    public static int CountSeperators(this String s, String separator)
    {
        if (s.Contains(separator) == false)
        {
            return 0;
        }
        int count = 0;
        foreach (char c in s)
        {
            if (c == separator[0])
            {
                count++;
            }
        }
        return count;
    }

    public static String ArrayToString<T>(this T[] array, bool newLine = false)
    {
        if (newLine == true)
        {
            return String.Join(Environment.NewLine, array.Select(x => x!.ToString()));
        }
        return "{" + String.Join(",", array.Select(x => x!.ToString())) + "}";
    }

    public static String ToNumberString(this bool b)
    {
        return b == true ? "1" : "0";
    }

    public static String[] ToArray(this String s, String separator)
    {
        return s.Replace(Environment.NewLine, separator).Split(separator[0]);
    }

    public static int ToPositive(this int i)
    {
        return i < 0 ? -i : i;
    }

    public static int Clamp(this int i, int min, int max)
    {
        return Math.Clamp(i, min, max);
    }

    public static float Clamp(this float f, float min, float max)
    {
        return Math.Clamp(f, min, max);
    }

    public static decimal Clamp(this decimal d, decimal min, decimal max)
    {
        return Math.Clamp(d, min, max);
    }

    public static double Clamp(this double d, double min, double max)
    {
        return Math.Clamp(d, min, max);
    }

    public static String CropStringToWidth(this String s, SpriteFont font, float scale, int width)
    {
        if (font.MeasureString(s).X * scale <= width)
        {
            return s;
        }

        if (s.Contains(' ') == false)
        {
            String newText = "";
            foreach (char ch in s)
            {
                if (font.MeasureString(newText + ch).X * scale > width)
                {
                    newText += Environment.NewLine;
                }
                newText += ch;
            }
            return newText;
        }

        String output = "";
        String currentLine = "";
        String currentWord = "";
        String fulltext = s;

        while (fulltext.Length > 0)
        {
            if (fulltext.StartsWith(Environment.NewLine) == true)
            {
                if (currentLine.Equals("") == false)
                {
                    currentLine += " ";
                }
                currentLine += currentWord;
                output += currentLine + Environment.NewLine;
                currentLine = "";
                currentWord = "";
                fulltext = fulltext[Environment.NewLine.Length..];
            }
            else if (fulltext.StartsWith(' ') == true)
            {
                if (currentLine.Equals("") == false)
                {
                    currentLine += " ";
                }
                currentLine += currentWord;
                currentWord = "";
                fulltext = fulltext[1..];
            }
            else
            {
                currentWord += fulltext[0];
                if (font.MeasureString(currentLine + currentWord).X * scale >= width)
                {
                    if (currentLine.Equals(""))
                    {
                        output += currentWord + Environment.NewLine;
                        currentWord = "";
                    }
                    else
                    {
                        output += currentLine + Environment.NewLine;
                        currentLine = "";
                    }
                }
                fulltext = fulltext[1..];
            }
        }

        if (currentWord.Equals("") == false)
        {
            if (currentLine.Equals("") == false)
            {
                currentLine += " ";
            }
            currentLine += currentWord;
        }
        if (currentLine.Equals("") == false)
        {
            output += currentLine;
        }

        return output;
    }

    public static String CropStringToWidth(this String s, SpriteFont font, int width)
    {
        return s.CropStringToWidth(font, 1f, width);
    }

    public static Color ToColor(this Vector3 v)
    {
        return new Color((int)(v.X * 255), (int)(v.Y * 255), (int)(v.Z * 255));
    }

    public static Texture2D ReplaceColors(this Texture2D t, Color[] inputColors, Color[] outputColors)
    {
        Texture2D newTexture = new Texture2D(Core.GraphicsDevice, t.Width, t.Height);
        if (inputColors.Length != outputColors.Length || inputColors.Length == 0)
        {
            return t;
        }

        Color[] data = new Color[t.Width * t.Height];
        t.GetData(0, null, data, 0, t.Width * t.Height);

        for (int i = 0; i < data.Length; i++)
        {
            for (int c = 0; c < inputColors.Length; c++)
            {
                if (data[i] == inputColors[c])
                {
                    data[i] = outputColors[c];
                    break;
                }
            }
        }

        newTexture.SetData(data);
        return newTexture;
    }

    public static double xRoot(this int root, double number)
    {
        return Math.Pow(number, 1.0 / root);
    }

    public static String[] SplitAtNewline(this String s)
    {
        return s
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');
    }

    public static String[] Split(this String s, String splitAt)
    {
        return s.Split([ splitAt ], StringSplitOptions.None);
    }

    public static List<T> Randomize<T>(this List<T> list)
    {
        return Randomize(list.ToArray()).ToList();
    }

    public static T[] Randomize<T>(this T[] arr)
    {
        Random r = new Random();
        for (int i = 0; i < arr.Length - 1; i++)
        {
            int j = r.Next(i, arr.Length);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    public static int GetRandomChance(List<int> chances)
    {
        int total = chances.Sum();
        int roll = Core.Random.Next(0, total + 1);
        int x = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            x += chances[i];
            if (roll <= x)
            {
                return i;
            }
        }
        return -1;
    }

    public static Vector2 ProjectPoint(this Vector3 v, Matrix view, Matrix projection)
    {
        Matrix mat = Matrix.Identity * view * projection;
        Vector4 v4 = Vector4.Transform(v, mat);
        return new Vector2(
            (float)((v4.X / v4.W + 1) * (Core.windowSize.Width / 2)),
            (float)((1 - v4.Y / v4.W) * (Core.windowSize.Height / 2)));
    }

    public static int ToInteger(this float f)
    {
        return (int)f;
    }

    public static Color Invert(this Color c)
    {
        return new Color(255 - c.R, 255 - c.G, 255 - c.B, c.A);
    }

    public static String ReplaceDecSeparator(this String s)
    {
        return s.Replace(GameController.DecSeparator, ".");
    }

    public static String InsertDecSeparator(this String s)
    {
        return s.Replace(".", GameController.DecSeparator);
    }
}
