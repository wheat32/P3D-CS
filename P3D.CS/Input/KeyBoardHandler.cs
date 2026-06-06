using Microsoft.Xna.Framework.Input;

namespace P3D;

public static class KeyBoardHandler
{
    private static KeyboardState _oldState;
    private static KeyboardState _newState;

    public static KeyboardState KeyBoardState
    {
        get => _newState;
        set => _newState = value;
    }

    public static void Update()
    {
        _oldState = _newState;
        _newState = Keyboard.GetState();
    }

    public static bool KeyPressed(Keys key)
    {
        return _oldState.IsKeyDown(key) == false && _newState.IsKeyDown(key) == true;
    }

    public static bool KeyDown(Keys key)
    {
        return _newState.IsKeyDown(key);
    }

    public static bool KeyUp(Keys key)
    {
        return _newState.IsKeyUp(key);
    }

    public static bool HasKeyboardInput()
    {
        return _newState.GetPressedKeys().Length > 0;
    }

    public static Keys[] GetPressedKeys()
    {
        return _newState.GetPressedKeys();
    }
}
