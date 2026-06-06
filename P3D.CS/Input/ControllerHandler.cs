using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public static class ControllerHandler
{
    private const int THUMBSTICK_ANGLE_DEGREES = 57;
    private const int THUMBSTICK_NO_INPUT = 999;
    private const int THUMBSTICK_NOT_ENGAGED = -1;
    private const int DIRECTION_UP = 0;
    private const int DIRECTION_LEFT = 1;
    private const int DIRECTION_DOWN = 2;
    private const int DIRECTION_RIGHT = 3;
    private const int ANGLE_UPPER_BOUND = 45;
    private const int ANGLE_UP_UPPER = 135;
    private const int ANGLE_LEFT_LOWER = -135;
    private const int ANGLE_LEFT_UPPER = 180;
    private const int ANGLE_DOWN_LOWER = -45;
    private const int ANGLE_DOWN_UPPER = -135;

    private static readonly Buttons[] ALL_BUTTONS =
    {
        Buttons.A, Buttons.B, Buttons.Back, Buttons.BigButton,
        Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp,
        Buttons.LeftShoulder, Buttons.LeftStick,
        Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft,
        Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp,
        Buttons.LeftTrigger, Buttons.RightShoulder, Buttons.RightStick,
        Buttons.RightThumbstickDown, Buttons.RightThumbstickLeft,
        Buttons.RightThumbstickRight, Buttons.RightThumbstickUp,
        Buttons.RightTrigger, Buttons.Start, Buttons.X, Buttons.Y
    };

    private static readonly Buttons[] LEFT_STICK_BUTTONS =
    {
        Buttons.LeftThumbstickUp, Buttons.LeftThumbstickLeft,
        Buttons.LeftThumbstickDown, Buttons.LeftThumbstickRight
    };

    private static GamePadState _oldState;
    private static GamePadState _newState;

    public static GamePadState GamePadState
    {
        get => _newState;
        set => _newState = value;
    }

    public static void Update()
    {
        _oldState = _newState;
        _newState = GamePad.GetState(PlayerIndex.One);
    }

    public static bool ButtonPressed(Buttons button)
    {
        return ButtonPressed(button, Core.GameOptions.GamePadEnabled);
    }

    public static bool ButtonPressed(Buttons button, bool gamePadEnabled)
    {
        if (gamePadEnabled == false)
        {
            return false;
        }

        if (LEFT_STICK_BUTTONS.Contains(button) == true)
        {
            int oldDir = LeftThumbstickDirection(_oldState, gamePadEnabled);
            int newDir = LeftThumbstickDirection(_newState, gamePadEnabled);

            switch (button)
            {
                case Buttons.LeftThumbstickUp:
                    return oldDir == THUMBSTICK_NOT_ENGAGED && newDir == DIRECTION_UP;

                case Buttons.LeftThumbstickLeft:
                    return oldDir == THUMBSTICK_NOT_ENGAGED && newDir == DIRECTION_LEFT;

                case Buttons.LeftThumbstickDown:
                    return oldDir == THUMBSTICK_NOT_ENGAGED && newDir == DIRECTION_DOWN;

                case Buttons.LeftThumbstickRight:
                    return oldDir == THUMBSTICK_NOT_ENGAGED && newDir == DIRECTION_RIGHT;

                default:
                    return false;
            }
        }

        return _oldState.IsButtonDown(button) == false && _newState.IsButtonDown(button) == true;
    }

    public static bool ButtonDown(Buttons button)
    {
        return ButtonDown(button, Core.GameOptions.GamePadEnabled);
    }

    public static bool ButtonDown(Buttons button, bool gamePadEnabled)
    {
        if (gamePadEnabled == false)
        {
            return false;
        }

        if (LEFT_STICK_BUTTONS.Contains(button) == true)
        {
            int dir = LeftThumbstickDirection(_newState, gamePadEnabled);

            switch (button)
            {
                case Buttons.LeftThumbstickUp:
                    return dir == DIRECTION_UP;

                case Buttons.LeftThumbstickLeft:
                    return dir == DIRECTION_LEFT;

                case Buttons.LeftThumbstickDown:
                    return dir == DIRECTION_DOWN;

                case Buttons.LeftThumbstickRight:
                    return dir == DIRECTION_RIGHT;

                default:
                    return false;
            }
        }

        return _newState.IsButtonDown(button);
    }

    public static int LeftThumbstickAngle(GamePadState state, bool gamePadEnabled)
    {
        if (gamePadEnabled == true)
        {
            if (state.ThumbSticks.Left.X != 0 || state.ThumbSticks.Left.Y != 0)
            {
                return (int)(Math.Atan2(state.ThumbSticks.Left.Y, state.ThumbSticks.Left.X) * THUMBSTICK_ANGLE_DEGREES);
            }
        }
        return THUMBSTICK_NO_INPUT;
    }

    public static int LeftThumbstickDirection(GamePadState state, bool gamePadEnabled)
    {
        if (gamePadEnabled == false || LeftThumbstickAngle(state, gamePadEnabled) == THUMBSTICK_NO_INPUT)
        {
            return THUMBSTICK_NOT_ENGAGED;
        }

        int angle = LeftThumbstickAngle(state, gamePadEnabled);

        if (angle > ANGLE_UPPER_BOUND && angle <= ANGLE_UP_UPPER)
        {
            return DIRECTION_UP;
        }

        if ((angle <= ANGLE_LEFT_LOWER && angle >= -ANGLE_LEFT_UPPER) ||
            (angle > ANGLE_UP_UPPER && angle <= ANGLE_LEFT_UPPER))
        {
            return DIRECTION_LEFT;
        }

        if (angle < -ANGLE_UPPER_BOUND && angle > ANGLE_DOWN_UPPER)
        {
            return DIRECTION_DOWN;
        }

        if (angle >= -ANGLE_UPPER_BOUND && angle < ANGLE_UPPER_BOUND)
        {
            return DIRECTION_RIGHT;
        }

        return THUMBSTICK_NOT_ENGAGED;
    }

    public static bool IsConnected(int index = 0)
    {
        return GamePad.GetState((PlayerIndex)index).IsConnected == true &&
               Core.GameOptions.GamePadEnabled == true;
    }

    public static bool HasControllerInput(int index = 0)
    {
        if (IsConnected() == false)
        {
            return false;
        }

        GamePadState state = GamePad.GetState((PlayerIndex)index);
        foreach (Buttons b in ALL_BUTTONS)
        {
            if (state.IsButtonDown(b) == true)
            {
                return true;
            }
        }
        return false;
    }
}
