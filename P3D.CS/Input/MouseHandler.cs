using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public static class MouseHandler
{
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton
    }

    private static MouseState _oldState;
    private static MouseState _newState;

    public static MouseState MouseState
    {
        get => _newState;
        set => _newState = value;
    }

    public static void Update()
    {
        _oldState = _newState;
        _newState = Mouse.GetState();
    }

    public static bool ButtonPressed(MouseButtons button)
    {
        if (WindowContainsMouse == false || GameController.IsActiveWindow() == false)
        {
            return false;
        }

        switch (button)
        {
            case MouseButtons.LeftButton:
                return _oldState.LeftButton == ButtonState.Released &&
                       _newState.LeftButton == ButtonState.Pressed;

            case MouseButtons.MiddleButton:
                return _oldState.MiddleButton == ButtonState.Released &&
                       _newState.MiddleButton == ButtonState.Pressed;

            case MouseButtons.RightButton:
                return _oldState.RightButton == ButtonState.Released &&
                       _newState.RightButton == ButtonState.Pressed;

            default:
                return false;
        }
    }

    public static bool ButtonDown(MouseButtons button)
    {
        if (WindowContainsMouse == false || GameController.IsActiveWindow() == false)
        {
            return false;
        }

        switch (button)
        {
            case MouseButtons.LeftButton:
                return _newState.LeftButton == ButtonState.Pressed;

            case MouseButtons.MiddleButton:
                return _newState.MiddleButton == ButtonState.Pressed;

            case MouseButtons.RightButton:
                return _newState.RightButton == ButtonState.Pressed;

            default:
                return false;
        }
    }

    public static bool ButtonUp(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.LeftButton:
                return _newState.LeftButton == ButtonState.Released;

            case MouseButtons.MiddleButton:
                return _newState.MiddleButton == ButtonState.Released;

            case MouseButtons.RightButton:
                return _newState.RightButton == ButtonState.Released;

            default:
                return false;
        }
    }

    public static bool IsInRectangle(Rectangle rec)
    {
        return rec.Contains(_newState.X, _newState.Y);
    }

    public static int GetScrollWheelChange()
    {
        return _newState.ScrollWheelValue - _oldState.ScrollWheelValue;
    }

    public static bool HasMouseInput
    {
        get
        {
            bool hasClick = ButtonDown(MouseButtons.LeftButton) == true ||
                            ButtonDown(MouseButtons.RightButton) == true ||
                            ButtonDown(MouseButtons.MiddleButton) == true ||
                            GetScrollWheelChange() != 0;
            return hasClick == true &&
                   WindowContainsMouse == true &&
                   GameController.IsActiveWindow() == true;
        }
    }

    public static bool WindowContainsMouse =>
        IsInRectangle(Core.GraphicsDevice.Viewport.Bounds);

    public static Point MousePosition =>
        new Point(_newState.X, _newState.Y);
}
