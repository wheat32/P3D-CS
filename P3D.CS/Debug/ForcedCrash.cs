using Microsoft.Xna.Framework.Input;

namespace P3D;

internal static class ForcedCrash
{
    private const float CRASH_DELAY_INITIAL = 14.0f;
    private const float CRASH_DELAY_DECREMENT = 0.1f;

    private static float _delay = CRASH_DELAY_INITIAL;

    public static void Update()
    {
        if (KeyBoardHandler.KeyDown(KeyBindings.DebugKey) == true &&
            KeyBoardHandler.KeyDown(Keys.C) == true)
        {
            System.Diagnostics.Debug.Print("CRASH IN: " + _delay);
            _delay -= CRASH_DELAY_DECREMENT;
            if (_delay <= 0f)
            {
                Crash();
            }
        }
        else
        {
            _delay = CRASH_DELAY_INITIAL;
        }
    }

    private static void Crash()
    {
        bool canCrash = true;
        if (Core.Player.loadedSave == true)
        {
            if (Core.Player.IsGameJoltSave == true || Core.Player.SandBoxMode == false)
            {
                canCrash = false;
            }
        }
        if (canCrash == true)
        {
            throw new Exception("Forced the game to crash.");
        }
        else
        {
            _delay = CRASH_DELAY_INITIAL;
        }
    }
}
