using Microsoft.Xna.Framework.Input;
using TextCopy;

namespace P3D;

public static class KeyBindings
{
    private const float HOLD_DELAY_INITIAL = 3.0f;
    private const float HOLD_DELAY_DECREMENT = 0.1f;
    private const int NO_MAX_LENGTH = -1;

    public static Keys ForwardMoveKey = Keys.W;
    public static Keys LeftMoveKey = Keys.A;
    public static Keys BackwardMoveKey = Keys.S;
    public static Keys RightMoveKey = Keys.D;
    public static Keys RunKey = Keys.LeftShift;

    public static Keys OpenInventoryKey = Keys.E;
    public static Keys ChatKey = Keys.T;
    public static Keys SpecialKey = Keys.Q;

    public static Keys UpKey = Keys.Up;
    public static Keys DownKey = Keys.Down;
    public static Keys RightKey = Keys.Right;
    public static Keys LeftKey = Keys.Left;

    public static Keys CameraLockKey = Keys.C;
    public static Keys MuteAudioKey = Keys.M;
    public static Keys OnlineStatusKey = Keys.Tab;

    public static Keys GUIControlKey = Keys.F1;
    public static Keys ScreenshotKey = Keys.F2;
    public static Keys DebugKey = Keys.F3;
    public static Keys LightKey = Keys.F4;
    public static Keys PerspectiveSwitchKey = Keys.F5;
    public static Keys DisableControllerKey = Keys.F6;
    public static Keys FullScreenKey = Keys.F11;
    public static Keys DebugWalkKey = Keys.LeftControl;

    public static Keys EnterKey1 = Keys.Enter;
    public static Keys EnterKey2 = Keys.Space;
    public static Keys BackKey1 = Keys.E;
    public static Keys BackKey2 = Keys.Back;
    public static Keys EscapeKey = Keys.Escape;

    private static float _holdDelay = HOLD_DELAY_INITIAL;
    private static Keys _holdKey = Keys.A;

    public static void LoadKeys()
    {
        String keysPath = Path.Combine(AppPaths.ConfigDir, "Keyboard.dat");
        if (File.Exists(keysPath) == false)
        {
            return;
        }

        String[] lines = File.ReadAllLines(keysPath);
        foreach (String line in lines)
        {
            if (line.StartsWith("[") == false)
            {
                continue;
            }

            String keyName = line.GetSplit(0, "=");
            Keys binding = GetKey(line.GetSplit(1, "="));

            keyName = keyName[1..^1];

            switch (keyName.ToLower())
            {
                case "moveforward":
                case "forwardmove":
                    ForwardMoveKey = binding;
                    break;

                case "moveleft":
                case "leftmove":
                    LeftMoveKey = binding;
                    break;

                case "movebackward":
                case "backwardmove":
                    BackwardMoveKey = binding;
                    break;

                case "moveright":
                case "rightmove":
                    RightMoveKey = binding;
                    break;

                case "run":
                    RunKey = binding;
                    break;

                case "openmenu":
                case "inventory":
                    OpenInventoryKey = binding;
                    break;

                case "chat":
                    ChatKey = binding;
                    break;

                case "special":
                case "phone":
                case "pokegear":
                    SpecialKey = binding;
                    break;

                case "muteaudio":
                case "mutemusic":
                    MuteAudioKey = binding;
                    break;

                case "cameraleft":
                case "left":
                    LeftKey = binding;
                    break;

                case "cameraright":
                case "right":
                    RightKey = binding;
                    break;

                case "cameraup":
                case "up":
                    UpKey = binding;
                    break;

                case "cameradown":
                case "down":
                    DownKey = binding;
                    break;

                case "cameralock":
                    CameraLockKey = binding;
                    break;

                case "guicontrol":
                    GUIControlKey = binding;
                    break;

                case "screenshot":
                    ScreenshotKey = binding;
                    break;

                case "debugcontrol":
                    DebugKey = binding;
                    break;

                case "debugwalk":
                    DebugWalkKey = binding;
                    break;

                case "perspectiveswitch":
                    PerspectiveSwitchKey = binding;
                    break;

                case "fullscreen":
                    FullScreenKey = binding;
                    break;

                case "enter1":
                    EnterKey1 = binding;
                    break;

                case "enter2":
                    EnterKey2 = binding;
                    break;

                case "back1":
                    BackKey1 = binding;
                    break;

                case "back2":
                    BackKey2 = binding;
                    break;

                case "escape":
                case "esc":
                    EscapeKey = binding;
                    break;

                case "onlinestatus":
                    OnlineStatusKey = binding;
                    break;

                case "lighting":
                case "lightning":
                    LightKey = binding;
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>Converts the name of a key to the actual Keys enum value. Returns Keys.None by default.</summary>
    public static Keys GetKey(String keyStr)
    {
        foreach (Keys k in Enum.GetValues<Keys>())
        {
            if (k.ToString().ToLower() == keyStr.ToLower())
            {
                return k;
            }
        }
        return Keys.None;
    }

    /// <summary>Returns the display name of a key.</summary>
    public static String GetKeyName(Keys key)
    {
        return key.ToString();
    }

    /// <summary>Creates the default Keyboard.dat file.</summary>
    public static void CreateKeySave(bool force)
    {
        Directory.CreateDirectory(AppPaths.ConfigDir);
        String keysPath = Path.Combine(AppPaths.ConfigDir, "Keyboard.dat");
        if (File.Exists(keysPath) == true && force == false)
        {
            return;
        }

        String s = String.Join(Environment.NewLine, new[]
        {
            $"[MoveForward]={GetKeyName(Keys.W)}",
            $"[MoveLeft]={GetKeyName(Keys.A)}",
            $"[MoveBackward]={GetKeyName(Keys.S)}",
            $"[MoveRight]={GetKeyName(Keys.D)}",
            $"[Run]={GetKeyName(Keys.LeftShift)}",
            $"[OpenMenu]={GetKeyName(Keys.E)}",
            $"[Chat]={GetKeyName(Keys.T)}",
            $"[Special]={GetKeyName(Keys.Q)}",
            $"[MuteAudio]={GetKeyName(Keys.M)}",
            $"[Up]={GetKeyName(Keys.Up)}",
            $"[Down]={GetKeyName(Keys.Down)}",
            $"[Left]={GetKeyName(Keys.Left)}",
            $"[Right]={GetKeyName(Keys.Right)}",
            $"[CameraLock]={GetKeyName(Keys.C)}",
            $"[GUIControl]={GetKeyName(Keys.F1)}",
            $"[ScreenShot]={GetKeyName(Keys.F2)}",
            $"[DebugControl]={GetKeyName(Keys.F3)}",
            $"[DebugWalkKey]={GetKeyName(Keys.LeftControl)}",
            $"[LightKey]={GetKeyName(Keys.F4)}",
            $"[PerspectiveSwitch]={GetKeyName(Keys.F5)}",
            $"[DisableController]={GetKeyName(Keys.F6)}",
            $"[FullScreen]={GetKeyName(Keys.F11)}",
            $"[Enter1]={GetKeyName(Keys.Enter)}",
            $"[Enter2]={GetKeyName(Keys.Space)}",
            $"[Back1]={GetKeyName(Keys.E)}",
            $"[Back2]={GetKeyName(Keys.E)}",
            $"[Escape]={GetKeyName(Keys.Escape)}",
            $"[OnlineStatus]={GetKeyName(Keys.Tab)}"
        });

        File.WriteAllText(keysPath, s);
    }

    /// <summary>Saves the current keyboard configuration to Keyboard.dat.</summary>
    public static void SaveKeys()
    {
        String s = String.Join(Environment.NewLine, new[]
        {
            $"[MoveForward]={GetKeyName(ForwardMoveKey)}",
            $"[MoveLeft]={GetKeyName(LeftMoveKey)}",
            $"[MoveBackward]={GetKeyName(BackwardMoveKey)}",
            $"[MoveRight]={GetKeyName(RightMoveKey)}",
            $"[Run]={GetKeyName(RunKey)}",
            $"[Inventory]={GetKeyName(OpenInventoryKey)}",
            $"[Chat]={GetKeyName(ChatKey)}",
            $"[Special]={GetKeyName(SpecialKey)}",
            $"[MuteAudio]={GetKeyName(MuteAudioKey)}",
            $"[Up]={GetKeyName(UpKey)}",
            $"[Down]={GetKeyName(DownKey)}",
            $"[Left]={GetKeyName(LeftKey)}",
            $"[Right]={GetKeyName(RightKey)}",
            $"[CameraLock]={GetKeyName(CameraLockKey)}",
            $"[GUIControl]={GetKeyName(GUIControlKey)}",
            $"[ScreenShot]={GetKeyName(ScreenshotKey)}",
            $"[DebugControl]={GetKeyName(DebugKey)}",
            $"[LightKey]={GetKeyName(LightKey)}",
            $"[PerspectiveSwitch]={GetKeyName(PerspectiveSwitchKey)}",
            $"[DisableController]={GetKeyName(DisableControllerKey)}",
            $"[FullScreen]={GetKeyName(FullScreenKey)}",
            $"[Enter1]={GetKeyName(EnterKey1)}",
            $"[Enter2]={GetKeyName(EnterKey2)}",
            $"[Back1]={GetKeyName(BackKey1)}",
            $"[Back2]={GetKeyName(BackKey2)}",
            $"[Escape]={GetKeyName(EscapeKey)}",
            $"[OnlineStatus]={GetKeyName(OnlineStatusKey)}"
        });

        File.WriteAllText(Path.Combine(AppPaths.ConfigDir, "Keyboard.dat"), s);
        Logger.Debug("---Saved Keybindings---");
    }

    public static String GetInput(
        Keys[] whiteKeys, Keys[] blackKeys,
        ref String text, int maxLength = NO_MAX_LENGTH,
        bool triggerShift = true, bool triggerAlt = true)
    {
        Keys[] pressedKeys = KeyBoardHandler.GetPressedKeys();
        foreach (Keys key in pressedKeys)
        {
            bool isCtrl = KeyBoardHandler.KeyDown(Keys.LeftControl) == true ||
                          KeyBoardHandler.KeyDown(Keys.RightControl) == true;

            if (key == Keys.V && KeyBoardHandler.KeyPressed(Keys.V) == true && isCtrl == true)
            {
                String? clipboard = ClipboardService.GetText();
                if (clipboard != null)
                {
                    text += clipboard.Replace(Environment.NewLine, " ");
                }
            }
            else
            {
                if (key != Keys.Back)
                {
                    if (KeyBlocked(whiteKeys, blackKeys, key) == false)
                    {
                        char? cc = KeyCharConverter.GetCharFromKey(key);
                        if (cc.HasValue == true)
                        {
                            if (_holdDelay <= 0f && _holdKey == key)
                            {
                                text += cc.ToString();
                            }
                            else if (KeyBoardHandler.KeyPressed(key) == true)
                            {
                                text += cc.ToString();
                                _holdKey = key;
                                _holdDelay = HOLD_DELAY_INITIAL;
                            }
                        }
                    }
                }

                if (key == Keys.Back && KeyBlocked(whiteKeys, blackKeys, Keys.Back) == false)
                {
                    if (_holdDelay <= 0f && _holdKey == key)
                    {
                        if (text.Length > 0)
                        {
                            text = text[..^1];
                        }
                    }
                    else if (KeyBoardHandler.KeyPressed(key) == true)
                    {
                        if (text.Length > 0)
                        {
                            text = text[..^1];
                        }
                        _holdKey = key;
                        _holdDelay = HOLD_DELAY_INITIAL;
                    }
                }
            }
        }

        if (KeyBoardHandler.KeyUp(_holdKey) == true)
        {
            _holdDelay = HOLD_DELAY_INITIAL;
        }
        else
        {
            _holdDelay -= HOLD_DELAY_DECREMENT;
            if (_holdDelay < 0f)
            {
                _holdDelay = 0f;
            }
        }

        if (maxLength > NO_MAX_LENGTH)
        {
            while (text.Length > maxLength)
            {
                text = text[..^1];
            }
        }

        return text;
    }

    private static readonly Keys[] DEFAULT_BLOCKED_KEYS =
    {
        Keys.Enter, Keys.Up, Keys.Down, Keys.Left, Keys.Right,
        Keys.Tab, Keys.Delete, Keys.Home, Keys.End, Keys.Escape
    };

    public static String GetInput(ref String text, int maxLength = NO_MAX_LENGTH,
        bool triggerShift = true, bool triggerAlt = true)
    {
        return GetInput([], DEFAULT_BLOCKED_KEYS,
            ref text, maxLength, triggerShift, triggerAlt);
    }

    private static readonly Keys[] NAME_INPUT_WHITE_KEYS =
    {
        Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
        Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
        Keys.Space, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
        Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.Back,
        Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G,
        Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N,
        Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U,
        Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
    };

    public static String GetNameInput(ref String text, int maxLength = NO_MAX_LENGTH,
        bool triggerShift = true, bool triggerAlt = true)
    {
        return GetInput(NAME_INPUT_WHITE_KEYS, [],
            ref text, maxLength, triggerShift, triggerAlt);
    }

    private static bool KeyBlocked(Keys[] whiteKeys, Keys[] blackKeys, Keys key)
    {
        bool hasWhite = whiteKeys.Length > 0;
        bool hasBlack = blackKeys.Length > 0;

        if (hasWhite == false || whiteKeys.Contains(key) == true)
        {
            if (hasBlack == false || blackKeys.Contains(key) == false)
            {
                return false;
            }
        }
        return true;
    }
}
