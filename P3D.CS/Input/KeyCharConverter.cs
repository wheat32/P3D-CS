using System.Text;
using Microsoft.Xna.Framework.Input;

namespace P3D;

/// <summary>
/// Cross-platform key-to-character converter.
/// Replaces the original Windows-only user32.dll P/Invoke approach with MonoGame's
/// TextInput event. GameController hooks Window.TextInput; each char is enqueued
/// here and dequeued when callers ask about a specific just-pressed key.
/// </summary>
public static class KeyCharConverter
{
    private static readonly Queue<char> _pendingChars = [];

    /// <summary>Called by GameController's TextInput handler for every typed char.</summary>
    public static void EnqueueChar(char c)
    {
        _pendingChars.Enqueue(c);
    }

    /// <summary>
    /// Returns the character associated with a key press, or null if none is available.
    /// Consumes one char from the TextInput queue, so call only once per key per frame.
    /// </summary>
    public static char? GetCharFromKey(Keys key)
    {
        if (_pendingChars.Count > 0)
        {
            return _pendingChars.Dequeue();
        }

        // Fallback: derive char from the key enum for common printable keys.
        // This covers cases where TextInput doesn't fire (e.g. automated tests).
        return KeyToCharFallback(key);
    }

    private static char? KeyToCharFallback(Keys key)
    {
        bool shift = KeyBoardHandler.KeyDown(Keys.LeftShift) ||
                     KeyBoardHandler.KeyDown(Keys.RightShift);
        bool caps = (System.Console.CapsLock);

        switch (key)
        {
            case Keys.Space:
                return ' ';

            case Keys.D0:
                return shift == true ? ')' : '0';

            case Keys.D1:
                return shift == true ? '!' : '1';

            case Keys.D2:
                return shift == true ? '@' : '2';

            case Keys.D3:
                return shift == true ? '#' : '3';

            case Keys.D4:
                return shift == true ? '$' : '4';

            case Keys.D5:
                return shift == true ? '%' : '5';

            case Keys.D6:
                return shift == true ? '^' : '6';

            case Keys.D7:
                return shift == true ? '&' : '7';

            case Keys.D8:
                return shift == true ? '*' : '8';

            case Keys.D9:
                return shift == true ? '(' : '9';

            case Keys.NumPad0:
                return '0';

            case Keys.NumPad1:
                return '1';

            case Keys.NumPad2:
                return '2';

            case Keys.NumPad3:
                return '3';

            case Keys.NumPad4:
                return '4';

            case Keys.NumPad5:
                return '5';

            case Keys.NumPad6:
                return '6';

            case Keys.NumPad7:
                return '7';

            case Keys.NumPad8:
                return '8';

            case Keys.NumPad9:
                return '9';

            case Keys.Multiply:
                return '*';

            case Keys.Add:
                return '+';

            case Keys.Subtract:
                return '-';

            case Keys.Decimal:
                return '.';

            case Keys.Divide:
                return '/';

            default:
            {
                // A-Z range
                if (key >= Keys.A && key <= Keys.Z)
                {
                    bool upperCase = shift != caps;
                    char c = (char)('a' + (key - Keys.A));
                    return upperCase == true ? char.ToUpper(c) : c;
                }
                return null;
            }
        }
    }
}
