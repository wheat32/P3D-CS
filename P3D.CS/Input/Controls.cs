using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public static class Controls
{
    private const float DIRECTION_RESET_DELAY = 4.0f;
    private const float HOLD_DOWN_DELAY_STEP = 0.1f;
    private const float HOLD_DOWN_REPEAT_DELAY = 0.3f;

    private enum PressDirections
    {
        Up,
        Left,
        Down,
        Right,
        None
    }

    private static PressDirections _lastPressedDirection = PressDirections.None;
    private static float _pressedKeyDelay;

    private static void ResetDirectionPressed(PressDirections direction)
    {
        if (_lastPressedDirection == direction)
        {
            _pressedKeyDelay = DIRECTION_RESET_DELAY;
            _lastPressedDirection = PressDirections.None;
        }
    }

    private static void ChangeDirectionPressed(PressDirections direction)
    {
        if (_lastPressedDirection != direction)
        {
            _pressedKeyDelay = DIRECTION_RESET_DELAY;
            _lastPressedDirection = direction;
        }
    }

    private static bool HoldDownPress(PressDirections direction)
    {
        if (_lastPressedDirection == direction)
        {
            _pressedKeyDelay -= HOLD_DOWN_DELAY_STEP;
            if (_pressedKeyDelay <= 0f)
            {
                _pressedKeyDelay = HOLD_DOWN_REPEAT_DELAY;
                return true;
            }
        }
        return false;
    }

    private static bool CheckDirectionalPress(
        PressDirections direction,
        Keys wasdKey, Keys arrowKey,
        Buttons thumbStickButton, Buttons dPadButton,
        bool useArrowKeys, bool useWASD, bool useThumbStick, bool useDPad)
    {
        bool command = false;

        if (useWASD == true)
        {
            if (KeyBoardHandler.KeyDown(wasdKey) == true)
            {
                command = true;
                if (HoldDownPress(direction) == true)
                {
                    return true;
                }
                if (KeyBoardHandler.KeyPressed(wasdKey) == true)
                {
                    ChangeDirectionPressed(direction);
                    return true;
                }
            }
        }

        if (useArrowKeys == true)
        {
            if (KeyBoardHandler.KeyDown(arrowKey) == true)
            {
                command = true;
                if (HoldDownPress(direction) == true)
                {
                    return true;
                }
                if (KeyBoardHandler.KeyPressed(arrowKey) == true)
                {
                    ChangeDirectionPressed(direction);
                    return true;
                }
            }
        }

        if (useThumbStick == true)
        {
            if (ControllerHandler.ButtonDown(thumbStickButton) == true)
            {
                command = true;
                if (HoldDownPress(direction) == true)
                {
                    return true;
                }
                if (ControllerHandler.ButtonPressed(thumbStickButton) == true)
                {
                    ChangeDirectionPressed(direction);
                    return true;
                }
            }
        }

        if (useDPad == true)
        {
            if (ControllerHandler.ButtonDown(dPadButton) == true)
            {
                command = true;
                if (HoldDownPress(direction) == true)
                {
                    return true;
                }
                if (ControllerHandler.ButtonPressed(dPadButton) == true)
                {
                    ChangeDirectionPressed(direction);
                    return true;
                }
            }
        }

        if (command == false)
        {
            ResetDirectionPressed(direction);
        }
        return false;
    }

    public static bool Left(
        bool pressed,
        bool arrowKeys = true, bool scroll = true,
        bool wasd = true, bool thumbStick = true, bool dPad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (MouseHandler.WindowContainsMouse == true && scroll == true)
        {
            if (MouseHandler.GetScrollWheelChange() > 0)
            {
                return true;
            }
        }
        if (pressed == true)
        {
            return CheckDirectionalPress(PressDirections.Left,
                KeyBindings.LeftMoveKey, KeyBindings.LeftKey,
                Buttons.LeftThumbstickLeft, Buttons.DPadLeft,
                arrowKeys, wasd, thumbStick, dPad);
        }
        if (wasd == true && KeyBoardHandler.KeyDown(KeyBindings.LeftMoveKey) == true)
        {
            return true;
        }
        if (arrowKeys == true && KeyBoardHandler.KeyDown(KeyBindings.LeftKey) == true)
        {
            return true;
        }
        if (thumbStick == true && ControllerHandler.ButtonDown(Buttons.LeftThumbstickLeft) == true)
        {
            return true;
        }
        if (dPad == true && ControllerHandler.ButtonDown(Buttons.DPadLeft) == true)
        {
            return true;
        }
        return false;
    }

    public static bool Right(
        bool pressed,
        bool arrowKeys = true, bool scroll = true,
        bool wasd = true, bool thumbStick = true, bool dPad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (MouseHandler.WindowContainsMouse == true && scroll == true)
        {
            if (MouseHandler.GetScrollWheelChange() < 0)
            {
                return true;
            }
        }
        if (pressed == true)
        {
            return CheckDirectionalPress(PressDirections.Right,
                KeyBindings.RightMoveKey, KeyBindings.RightKey,
                Buttons.LeftThumbstickRight, Buttons.DPadRight,
                arrowKeys, wasd, thumbStick, dPad);
        }
        if (wasd == true && KeyBoardHandler.KeyDown(KeyBindings.RightMoveKey) == true)
        {
            return true;
        }
        if (arrowKeys == true && KeyBoardHandler.KeyDown(KeyBindings.RightKey) == true)
        {
            return true;
        }
        if (thumbStick == true && ControllerHandler.ButtonDown(Buttons.LeftThumbstickRight) == true)
        {
            return true;
        }
        if (dPad == true && ControllerHandler.ButtonDown(Buttons.DPadRight) == true)
        {
            return true;
        }
        return false;
    }

    public static bool Up(
        bool pressed,
        bool arrowKeys = true, bool scroll = true,
        bool wasd = true, bool thumbStick = true, bool dPad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (MouseHandler.WindowContainsMouse == true && scroll == true)
        {
            if (MouseHandler.GetScrollWheelChange() > 0)
            {
                return true;
            }
        }
        if (pressed == true)
        {
            return CheckDirectionalPress(PressDirections.Up,
                KeyBindings.ForwardMoveKey, KeyBindings.UpKey,
                Buttons.LeftThumbstickUp, Buttons.DPadUp,
                arrowKeys, wasd, thumbStick, dPad);
        }
        if (wasd == true && KeyBoardHandler.KeyDown(KeyBindings.ForwardMoveKey) == true)
        {
            return true;
        }
        if (arrowKeys == true && KeyBoardHandler.KeyDown(KeyBindings.UpKey) == true)
        {
            return true;
        }
        if (thumbStick == true && ControllerHandler.ButtonDown(Buttons.LeftThumbstickUp) == true)
        {
            return true;
        }
        if (dPad == true && ControllerHandler.ButtonDown(Buttons.DPadUp) == true)
        {
            return true;
        }
        return false;
    }

    public static bool Down(
        bool pressed,
        bool arrowKeys = true, bool scroll = true,
        bool wasd = true, bool thumbStick = true, bool dPad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (MouseHandler.WindowContainsMouse == true && scroll == true)
        {
            if (MouseHandler.GetScrollWheelChange() < 0)
            {
                return true;
            }
        }
        if (pressed == true)
        {
            return CheckDirectionalPress(PressDirections.Down,
                KeyBindings.BackwardMoveKey, KeyBindings.DownKey,
                Buttons.LeftThumbstickDown, Buttons.DPadDown,
                arrowKeys, wasd, thumbStick, dPad);
        }
        if (wasd == true && KeyBoardHandler.KeyDown(KeyBindings.BackwardMoveKey) == true)
        {
            return true;
        }
        if (arrowKeys == true && KeyBoardHandler.KeyDown(KeyBindings.DownKey) == true)
        {
            return true;
        }
        if (thumbStick == true && ControllerHandler.ButtonDown(Buttons.LeftThumbstickDown) == true)
        {
            return true;
        }
        if (dPad == true && ControllerHandler.ButtonDown(Buttons.DPadDown) == true)
        {
            return true;
        }
        return false;
    }

    public static bool ShiftDown(String pressedFlag = "LR", bool triggerButtons = true)
    {
        if (pressedFlag.Contains('L') == true && KeyBoardHandler.KeyDown(Keys.LeftShift) == true)
        {
            return true;
        }
        if (pressedFlag.Contains('L') == true &&
            ControllerHandler.ButtonDown(Buttons.LeftTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true && KeyBoardHandler.KeyDown(Keys.RightShift) == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true &&
            ControllerHandler.ButtonDown(Buttons.RightTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        return false;
    }

    public static bool ShiftPressed(String pressedFlag = "LR", bool triggerButtons = true)
    {
        if (pressedFlag.Contains('L') == true && KeyBoardHandler.KeyPressed(Keys.LeftShift) == true)
        {
            return true;
        }
        if (pressedFlag.Contains('L') == true &&
            ControllerHandler.ButtonPressed(Buttons.LeftTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true && KeyBoardHandler.KeyPressed(Keys.RightShift) == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true &&
            ControllerHandler.ButtonPressed(Buttons.RightTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        return false;
    }

    /// <summary>Returns true when a Ctrl key is held. AltGr (Ctrl+Alt) is excluded.</summary>
    public static bool CtrlPressed(String pressedFlag = "LR", bool triggerButtons = true)
    {
        bool leftCtrl = KeyBoardHandler.KeyDown(Keys.LeftControl);
        bool rightCtrl = KeyBoardHandler.KeyDown(Keys.RightControl);
        bool altDown = KeyBoardHandler.KeyDown(Keys.LeftAlt) ||
                       KeyBoardHandler.KeyDown(Keys.RightAlt);

        // AltGr is Ctrl+Alt on some keyboards — treat as no Ctrl press
        if (leftCtrl == true && altDown == true)
        {
            return false;
        }

        if (pressedFlag.Contains('L') == true && leftCtrl == true)
        {
            return true;
        }
        if (pressedFlag.Contains('L') == true &&
            ControllerHandler.ButtonDown(Buttons.LeftTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true && rightCtrl == true)
        {
            return true;
        }
        if (pressedFlag.Contains('R') == true &&
            ControllerHandler.ButtonDown(Buttons.RightTrigger) == true &&
            triggerButtons == true)
        {
            return true;
        }
        return false;
    }

    public static bool HasKeyboardInput()
    {
        Keys[] keyArr =
        {
            KeyBindings.ForwardMoveKey, KeyBindings.LeftMoveKey,
            KeyBindings.BackwardMoveKey, KeyBindings.RightMoveKey,
            KeyBindings.UpKey, KeyBindings.LeftKey,
            KeyBindings.DownKey, KeyBindings.RightKey
        };
        foreach (Keys key in keyArr)
        {
            if (KeyBoardHandler.KeyPressed(key) == true)
            {
                return true;
            }
        }
        return false;
    }

    public static bool Accept(bool doMouse = true, bool doKeyboard = true, bool doGamePad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (doKeyboard == true)
        {
            if (KeyBoardHandler.KeyPressed(KeyBindings.EnterKey1) == true ||
                KeyBoardHandler.KeyPressed(KeyBindings.EnterKey2) == true)
            {
                return true;
            }
        }
        if (doMouse == true)
        {
            if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
            {
                return true;
            }
        }
        if (doGamePad == true)
        {
            if (ControllerHandler.ButtonPressed(Buttons.A) == true)
            {
                return true;
            }
        }
        return false;
    }

    public static bool Dismiss(bool doMouse = true, bool doKeyboard = true, bool doGamePad = true)
    {
        if (GameController.IsActiveWindow() == false)
        {
            return false;
        }
        if (doKeyboard == true)
        {
            if (KeyBoardHandler.KeyPressed(KeyBindings.BackKey1) == true ||
                KeyBoardHandler.KeyPressed(KeyBindings.BackKey2) == true)
            {
                return true;
            }
        }
        if (doMouse == true)
        {
            if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) == true)
            {
                return true;
            }
        }
        if (doGamePad == true)
        {
            if (ControllerHandler.ButtonPressed(Buttons.B) == true)
            {
                return true;
            }
        }
        return false;
    }

    public static void MakeMouseVisible()
    {
        MouseState mState = Mouse.GetState();
        if (Core.GameInstance.IsMouseVisible == false && Core.CurrentScreen.MouseVisible == true)
        {
            if (mState.X != MouseHandler.MousePosition.X || mState.Y != MouseHandler.MousePosition.Y)
            {
                Core.GameInstance.IsMouseVisible = true;
            }
        }
        else if (Core.GameInstance.IsMouseVisible == true)
        {
            if (ControllerHandler.HasControllerInput() == true || Controls.HasKeyboardInput() == true)
            {
                Core.GameInstance.IsMouseVisible = false;
            }
        }
    }
}
