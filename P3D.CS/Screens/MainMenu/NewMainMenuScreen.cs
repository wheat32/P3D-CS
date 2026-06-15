using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class NewMainMenuScreen : Screen
{
    public Vector2 _screenOffset = Vector2.Zero;
    public Vector2 _screenOffsetTarget = Vector2.Zero;
    public Vector2 _screenOrigin;
    private Vector2 _mainOffset = Vector2.Zero;

    public Vector2 _optionsOffset = Vector2.Zero;
    public Vector2 _optionsOffsetTarget = Vector2.Zero;

    private Vector2 _gameJoltOffset = Vector2.Zero;
    private Vector2 _gameJoltOffsetTarget = Vector2.Zero;

    private bool _loading = true;
    public float _fadeInMain = 0f;
    public float _fadeInOptions = 0f;
    private float _fadeInGameJolt = 0f;
    private float _expandDisplay = 0f;
    private bool _closingDisplay = false;

    private float _sliderPosition = 0f;
    private int _sliderTarget = 0;

    private Texture2D? _menuTexture;
    private Texture2D? _oldMenuTexture;

    private List<GameProfile> _mainProfiles = [];
    private List<GameProfile> _gameJoltProfiles = [];
    private List<GameProfile> _optionsProfiles = [];
    public static int _selectedProfile = 2;
    public static int _selectedProfileTemp = _selectedProfile;
    private int _gameJoltButtonIndex = 0;
    public static int _menuIndex = 0;

    public Texture2D? GameModeSplash;

    private MessageBox? _messageBox;

    public NewMainMenuScreen(Screen currentScreen)
    {
        Identification = Identifications.MainMenuScreen;
        PreScreen = currentScreen;

        CanBePaused = false;
        MouseVisible = true;
        CanChat = false;

        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
        _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        _screenOrigin = new Vector2((int)(Core.windowSize.Width / 2 - 80 - 180), (int)(Core.windowSize.Height / 4));
        _screenOffset = new Vector2(-180, 0);
        _screenOffsetTarget = _screenOffset;
        _mainOffset = _screenOffset;
        _optionsOffset = Vector2.Zero;
        _optionsOffsetTarget = _optionsOffset;
        _gameJoltOffset = Vector2.Zero;
        _gameJoltOffsetTarget = _gameJoltOffset;

        _selectedProfile = 2;
        _sliderTarget = GetSliderTarget(_selectedProfile);
        _sliderPosition = _sliderTarget;

        _messageBox = new MessageBox(this);
    }

    public override void Update()
    {
        PreScreen?.Update();

        if (_loading == false)
        {
            _mainOffset.X = _screenOffset.X;

            switch (_menuIndex)
            {
                case 0:
                    _screenOffsetTarget.Y = 0;
                    _optionsOffsetTarget.X = _screenOffsetTarget.X;
                    break;
                case 1:
                    _screenOffsetTarget.Y = -180 - 32;
                    _optionsOffsetTarget.X = _gameJoltOffsetTarget.X;
                    break;
                case 2:
                    _screenOffsetTarget.Y = -180 - 32;
                    break;
                case 3:
                    _screenOffsetTarget.Y = -320 - 32;
                    break;
            }

            if (_menuIndex == 2 || _menuIndex == 3)
            {
                if (_fadeInOptions < 1.0f)
                {
                    _fadeInOptions = MathHelper.Lerp(1.0f, _fadeInOptions, 0.93f);
                    if (_fadeInOptions + 0.01f >= 1.0f) _fadeInOptions = 1.0f;
                }
            }
            else
            {
                if (_fadeInOptions > 0.0f)
                {
                    _fadeInOptions = MathHelper.Lerp(0.0f, _fadeInOptions, 0.93f);
                    if (_fadeInOptions - 0.01f <= 0.0f) _fadeInOptions = 0.0f;
                }
            }

            if (_fadeInMain < 1.0f)
            {
                _fadeInMain = MathHelper.Lerp(1.0f, _fadeInMain, 0.93f);
                if (_fadeInMain + 0.01f >= 1.0f)
                {
                    _fadeInMain = 1.0f;
                    _sliderPosition = _sliderTarget;
                }
            }
            else
            {
                if (Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                {
                    if (Controls.Accept(true, false, false) == true)
                    {
                        switch (_menuIndex)
                        {
                            case 0:
                                for (int x = 0; x <= _mainProfiles.Count - 1; x++)
                                {
                                    float xOffset = _screenOrigin.X + _screenOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                    if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _screenOffset.Y), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                    {
                                        if (_selectedProfile == x)
                                        {
                                            ClickedProfile();
                                            SoundManager.PlaySound("select");
                                        }
                                        else
                                        {
                                            GameModeSplash = null;
                                            int diff = x - _selectedProfile;
                                            _screenOffsetTarget.X -= diff * 180;
                                            _selectedProfile = x;
                                            if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                                GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                            else
                                                GameModeManager.SetGameModePointer("Kolben");
                                            Localization.ReloadGameModeTokens();
                                            _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                            _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                            break;
                                        }
                                    }
                                }
                                if (_mainProfiles[_selectedProfile].IsGameJolt == true && _mainProfiles[_selectedProfile].Loaded == true)
                                {
                                    for (int x = 0; x <= _mainProfiles.Count - 1; x++)
                                    {
                                        float xOffset = _screenOrigin.X + _screenOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                        Vector2 r = new Vector2(xOffset + 400, _screenOrigin.Y + _screenOffset.Y + 200);
                                        if (new Rectangle((int)r.X, (int)r.Y, 32, 32).Contains(MouseHandler.MousePosition) == true)
                                            ButtonChangeMale();
                                        else if (new Rectangle((int)r.X, (int)r.Y + 48, 32, 32).Contains(MouseHandler.MousePosition) == true)
                                            ButtonChangeFemale();
                                        else if (new Rectangle((int)r.X, (int)r.Y + 96, 32, 32).Contains(MouseHandler.MousePosition) == true)
                                            ButtonChangeGenderless();
                                        else if (new Rectangle((int)r.X, (int)r.Y + 144, 32, 32).Contains(MouseHandler.MousePosition) == true)
                                            ButtonResetSave();
                                    }
                                }
                                if (Controls.Dismiss(true, false, false) == true)
                                {
                                    for (int x = 0; x <= _mainProfiles.Count - 1; x++)
                                    {
                                        float xOffset = _screenOrigin.X + _screenOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                        if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _screenOffset.Y), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                        {
                                            if (_selectedProfile == x)
                                            {
                                                SoundManager.PlaySound("select");
                                                DismissProfile();
                                            }
                                            break;
                                        }
                                    }
                                }
                                break;

                            case 2:
                                GameModeSplash = null;
                                for (int x = 0; x <= _mainProfiles.Count - 1; x++)
                                {
                                    float xOffset = _screenOrigin.X + _mainOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                    if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _screenOffsetTarget.Y), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                    {
                                        _menuIndex = 0;
                                        float diff = _screenOffset.X + x * 180 - (_optionsOffset.X + _selectedProfile * 180);
                                        _screenOffsetTarget.X -= (int)diff;
                                        _selectedProfile = x;
                                        _sliderTarget = GetSliderTarget(x);
                                        SoundManager.PlaySound("select");
                                        if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                            GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                        else
                                            GameModeManager.SetGameModePointer("Kolben");
                                        Localization.ReloadGameModeTokens();
                                        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                        _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                    }
                                }
                                for (int x = 0; x <= _optionsProfiles.Count - 1; x++)
                                {
                                    float xOffset = _screenOrigin.X + _optionsOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                    if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _optionsOffset.Y), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                    {
                                        if (_selectedProfile == x)
                                        {
                                            ClickedProfile();
                                            SoundManager.PlaySound("select");
                                        }
                                        else
                                        {
                                            int diff = x - _selectedProfile;
                                            _optionsOffsetTarget.X -= diff * 180;
                                            _selectedProfile = x;
                                            break;
                                        }
                                    }
                                }
                                break;

                            case 3:
                                for (int x = 0; x <= _gameJoltProfiles.Count - 1; x++)
                                {
                                    float xOffset = _screenOrigin.X + _gameJoltOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                    if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _screenOffsetTarget.Y + -180 + 32), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                    {
                                        int diff = x - _selectedProfile;
                                        _gameJoltOffsetTarget.X -= diff * 180;
                                        _menuIndex = 1;
                                        _selectedProfile = x;
                                        _sliderTarget = GetSliderTarget(x);
                                        SoundManager.PlaySound("select");
                                        if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                            GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                        else
                                            GameModeManager.SetGameModePointer("Kolben");
                                        Localization.ReloadGameModeTokens();
                                        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                        _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                    }
                                }
                                for (int x = 0; x <= _optionsProfiles.Count - 1; x++)
                                {
                                    float xOffset = _screenOrigin.X + _optionsOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));
                                    if (new Rectangle((int)xOffset, (int)(_screenOrigin.Y + _optionsOffset.Y), 160, 160).Contains(MouseHandler.MousePosition) == true)
                                    {
                                        if (_selectedProfile == x)
                                        {
                                            ClickedProfile();
                                            SoundManager.PlaySound("select");
                                        }
                                        else
                                        {
                                            GameModeSplash = null;
                                            int diff = x - _selectedProfile;
                                            _optionsOffsetTarget.X -= diff * 180;
                                            _selectedProfile = x;
                                            break;
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    if (Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                    {
                        switch (_menuIndex)
                        {
                            case 0:
                                if (Controls.Right(true) == true && _selectedProfile < _mainProfiles.Count - 1)
                                {
                                    _selectedProfile += 1;
                                    _screenOffsetTarget.X -= 180;
                                    _gameJoltButtonIndex = 0;
                                    GameModeSplash = null;
                                    if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                        GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                    Localization.ReloadGameModeTokens();
                                    _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                    _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                }
                                if (Controls.Left(true) == true && _selectedProfile > 0)
                                {
                                    _selectedProfile -= 1;
                                    _screenOffsetTarget.X += 180;
                                    _gameJoltButtonIndex = 0;
                                    GameModeSplash = null;
                                    if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                        GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                    Localization.ReloadGameModeTokens();
                                    _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                    _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                }
                                break;
                            case 1:
                                if (Controls.Right(true) == true && _selectedProfile < _gameJoltProfiles.Count - 1)
                                {
                                    _selectedProfile += 1;
                                    _gameJoltOffsetTarget.X -= 180;
                                    _gameJoltButtonIndex = 0;
                                    GameModeSplash = null;
                                    if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                        GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                    Localization.ReloadGameModeTokens();
                                    _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                    _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                }
                                if (Controls.Left(true) == true && _selectedProfile > 0)
                                {
                                    _selectedProfile -= 1;
                                    _gameJoltOffsetTarget.X += 180;
                                    _gameJoltButtonIndex = 0;
                                    GameModeSplash = null;
                                    if (_mainProfiles[_selectedProfile]._gameModeExists == true)
                                        GameModeManager.SetGameModePointer(_mainProfiles[_selectedProfile]._gameMode);
                                    Localization.ReloadGameModeTokens();
                                    _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
                                    _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
                                }
                                break;
                            case 2:
                            case 3:
                                if (Controls.Right(true) == true && _selectedProfile < _optionsProfiles.Count - 1)
                                {
                                    _selectedProfile += 1;
                                    _optionsOffsetTarget.X -= 180;
                                }
                                if (Controls.Left(true) == true && _selectedProfile > 0)
                                {
                                    _selectedProfile -= 1;
                                    _optionsOffsetTarget.X += 180;
                                }
                                break;
                        }
                    }

                    if (_menuIndex == 0)
                    {
                        if (_mainProfiles[_selectedProfile].IsGameJolt == true && _mainProfiles[_selectedProfile].Loaded == true)
                        {
                            if (Controls.Down(true, true, false) == true) _gameJoltButtonIndex += 1;
                            if (Controls.Up(true, true, false) == true) _gameJoltButtonIndex -= 1;
                            _gameJoltButtonIndex = _gameJoltButtonIndex.Clamp(0, 4);
                        }
                    }

                    switch (_menuIndex)
                    {
                        case 0: _selectedProfile = _selectedProfile.Clamp(0, _mainProfiles.Count - 1); break;
                        case 1: _selectedProfile = _selectedProfile.Clamp(0, _gameJoltProfiles.Count - 1); break;
                        case 2:
                        case 3: _selectedProfile = _selectedProfile.Clamp(0, _optionsProfiles.Count - 1); break;
                    }

                    if (_fadeInMain == 1.0f)
                    {
                        if (Controls.Accept(false, true, true) == true)
                        {
                            switch (_gameJoltButtonIndex)
                            {
                                case 0: SoundManager.PlaySound("select"); ClickedProfile(); break;
                                case 1: SoundManager.PlaySound("select"); ButtonChangeMale(); break;
                                case 2: SoundManager.PlaySound("select"); ButtonChangeFemale(); break;
                                case 3: SoundManager.PlaySound("select"); ButtonChangeGenderless(); break;
                                case 4: SoundManager.PlaySound("select"); ButtonResetSave(); break;
                            }
                        }
                        if (Controls.Dismiss(false, true, false) == true)
                        {
                            SoundManager.PlaySound("select");
                            DismissProfile();
                            _gameJoltButtonIndex = 0;
                        }
                        if (Controls.Dismiss(false, false, true) == true)
                        {
                            DismissProfile();
                            _gameJoltButtonIndex = 0;
                        }
                        _mainProfiles[1].LoadGameJolt();
                    }

                    if (_menuIndex == 0)
                    {
                        _closingDisplay = _mainProfiles[_selectedProfile].Loaded == false;
                    }

                    _sliderTarget = GetSliderTarget(_selectedProfile);
                    if (_sliderPosition < _sliderTarget || _sliderPosition > _sliderTarget)
                    {
                        _sliderPosition = MathHelper.Lerp(_sliderTarget, _sliderPosition, 0.8f);
                    }

                    bool backPressed =
                        KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) == true ||
                        KeyBoardHandler.KeyPressed(KeyBindings.BackKey1) == true ||
                        KeyBoardHandler.KeyPressed(KeyBindings.BackKey2) == true ||
                        MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) == true ||
                        ControllerHandler.ButtonPressed(Buttons.B) == true;

                    if (backPressed == true)
                    {
                        switch (_menuIndex)
                        {
                            case 0:
                                if (Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                                {
                                    Core.SetScreen(new PressStartScreen());
                                }
                                SoundManager.PlaySound("select");
                                break;
                            case 1:
                            case 2:
                                _menuIndex = 0;
                                _selectedProfile = _selectedProfileTemp;
                                _sliderTarget = GetSliderTarget(_selectedProfile);
                                SoundManager.PlaySound("select");
                                break;
                            case 3:
                                _menuIndex = 1;
                                _selectedProfile = _selectedProfileTemp;
                                _sliderTarget = GetSliderTarget(_selectedProfile);
                                SoundManager.PlaySound("select");
                                break;
                        }
                    }

                    if (KeyBoardHandler.KeyPressed(KeyBindings.ForwardMoveKey) == true || KeyBoardHandler.KeyPressed(KeyBindings.UpKey) == true)
                    {
                        switch (_menuIndex)
                        {
                            case 1:
                            case 2:
                                _menuIndex = 0;
                                _selectedProfile = _selectedProfileTemp;
                                _sliderTarget = GetSliderTarget(_selectedProfile);
                                SoundManager.PlaySound("select");
                                break;
                            case 3:
                                _menuIndex = 1;
                                _selectedProfile = _selectedProfileTemp;
                                _sliderTarget = GetSliderTarget(_selectedProfile);
                                SoundManager.PlaySound("select");
                                break;
                        }
                    }

                    if (_fadeInMain == 1.0f)
                    {
                        if (_closingDisplay == true)
                        {
                            if (_expandDisplay > 0.0f)
                            {
                                _expandDisplay = MathHelper.Lerp(0.0f, _expandDisplay, 0.9f);
                                if (_expandDisplay - 0.01f <= 0.0f) _expandDisplay = 0.0f;
                            }
                        }
                        else
                        {
                            if (_expandDisplay < 1.0f)
                            {
                                _expandDisplay = MathHelper.Lerp(1.0f, _expandDisplay, 0.9f);
                                if (_expandDisplay + 0.01f >= 1.0f) _expandDisplay = 1.0f;
                            }
                        }
                    }
                }

                UpdateScreenOffset();
                UpdateOptionsOffset();

                if ((_menuIndex == 0 || _menuIndex == 1) && _mainProfiles[_selectedProfile].GameMode != "Kolben")
                {
                    if (GameModeSplash == null)
                    {
                        try
                        {
                            String fileName = GameController.GamePath + @"\GameModes\" + _mainProfiles[_selectedProfile].GameMode + @"\MainMenu.png";
                            if (File.Exists(fileName) == true)
                            {
                                using Stream stream = File.Open(fileName, FileMode.OpenOrCreate);
                                GameModeSplash = Texture2D.FromStream(Core.GraphicsDevice, stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(Logger.LogTypes.ErrorMessage, "NewMainMenuScreen.cs/Update: " + ex.Message);
                        }
                    }
                }
            }
        }
    }

    private void ButtonChangeMale()
    {
        if (Core.GameJoltSave.Gender == "Male") return;
        Core.GameJoltSave.Gender = "Male";
        Core.Player.Skin = GameJolt.Emblem.GetPlayerSpriteFile(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        _mainProfiles[_selectedProfile].Sprite = GameJolt.Emblem.GetPlayerSprite(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
    }

    private void ButtonChangeFemale()
    {
        if (Core.GameJoltSave.Gender == "Female") return;
        Core.GameJoltSave.Gender = "Female";
        Core.Player.Skin = GameJolt.Emblem.GetPlayerSpriteFile(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        _mainProfiles[_selectedProfile].Sprite = GameJolt.Emblem.GetPlayerSprite(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
    }

    private void ButtonChangeGenderless()
    {
        if (Core.GameJoltSave.Gender == "Other") return;
        Core.GameJoltSave.Gender = "Other";
        Core.Player.Skin = GameJolt.Emblem.GetPlayerSpriteFile(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        _mainProfiles[_selectedProfile].Sprite = GameJolt.Emblem.GetPlayerSprite(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
    }

    private void ButtonResetSave()
    {
        Core.GameJoltSave.ResetSave();
        _mainProfiles[_selectedProfile].Sprite = GameJolt.Emblem.GetPlayerSprite(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        _mainProfiles[_selectedProfile].SetToDefault();
    }

    private void ClickedProfile()
    {
        switch (_menuIndex)
        {
            case 0:
                if (_mainProfiles[_selectedProfile].IsGameJolt && Security.FileValidation.IsValid(false) == false)
                {
                    _messageBox?.Show(Localization.GetString("main_menu_error_filevalidation", "File validation failed!~Redownload the game's files to solve this problem.")
                        .Replace("~", Environment.NewLine).Replace("*", Environment.NewLine + Environment.NewLine));
                }
                else
                {
                    _mainProfiles[_selectedProfile].SelectProfile(this);
                }
                break;
            case 2:
            case 3:
                _optionsProfiles[_selectedProfile].SelectProfile(this);
                break;
        }
    }

    private void DismissProfile()
    {
        if (_menuIndex == 0)
        {
            _mainProfiles[_selectedProfile].UnSelectProfile();
        }
    }

    private void UpdateScreenOffset()
    {
        _screenOrigin = new Vector2((int)(Core.windowSize.Width / 2 - 80 - 180), (int)(Core.windowSize.Height / 4));

        if (_screenOffset.X > _screenOffsetTarget.X)
        {
            _screenOffset.X = MathHelper.Lerp(_screenOffsetTarget.X, _screenOffset.X, 0.93f);
            if (_screenOffset.X - 0.01f <= _screenOffsetTarget.X) _screenOffset.X = _screenOffsetTarget.X;
        }
        if (_screenOffset.X < _screenOffsetTarget.X)
        {
            _screenOffset.X = MathHelper.Lerp(_screenOffsetTarget.X, _screenOffset.X, 0.93f);
            if (_screenOffset.X + 0.01f >= _screenOffsetTarget.X) _screenOffset.X = _screenOffsetTarget.X;
        }
        if (_screenOffset.Y > _screenOffsetTarget.Y)
        {
            _screenOffset.Y = MathHelper.Lerp(_screenOffsetTarget.Y, _screenOffset.Y, 0.93f);
            if (_screenOffset.Y - 0.01f <= _screenOffsetTarget.Y) _screenOffset.Y = _screenOffsetTarget.Y;
        }
        if (_screenOffset.Y < _screenOffsetTarget.Y)
        {
            _screenOffset.Y = MathHelper.Lerp(_screenOffsetTarget.Y, _screenOffset.Y, 0.93f);
            if (_screenOffset.Y + 0.01f >= _screenOffsetTarget.Y) _screenOffset.Y = _screenOffsetTarget.Y;
        }
    }

    private void UpdateOptionsOffset()
    {
        if (_optionsOffset.X > _optionsOffsetTarget.X)
        {
            _optionsOffset.X = MathHelper.Lerp(_optionsOffsetTarget.X, _optionsOffset.X, 0.93f);
            if (_optionsOffset.X - 0.01f <= _optionsOffsetTarget.X) _optionsOffset.X = _optionsOffsetTarget.X;
        }
        if (_optionsOffset.X < _optionsOffsetTarget.X)
        {
            _optionsOffset.X = MathHelper.Lerp(_optionsOffsetTarget.X, _optionsOffset.X, 0.93f);
            if (_optionsOffset.X + 0.01f >= _optionsOffsetTarget.X) _optionsOffset.X = _optionsOffsetTarget.X;
        }
        if (_optionsOffset.Y > _optionsOffsetTarget.Y)
        {
            _optionsOffset.Y = MathHelper.Lerp(_optionsOffsetTarget.Y, _optionsOffset.Y, 0.93f);
            if (_optionsOffset.Y - 0.01f <= _optionsOffsetTarget.Y) _optionsOffset.Y = _optionsOffsetTarget.Y;
        }
        if (_optionsOffset.Y < _optionsOffsetTarget.Y)
        {
            _optionsOffset.Y = MathHelper.Lerp(_optionsOffsetTarget.Y, _optionsOffset.Y, 0.93f);
            if (_optionsOffset.Y + 0.01f >= _optionsOffsetTarget.Y) _optionsOffset.Y = _optionsOffsetTarget.Y;
        }
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        if (_loading == false)
        {
            bool isGJ = _menuIndex == 0
                ? _mainProfiles[_selectedProfile].IsGameJolt
                : (_menuIndex == 1
                    ? (_gameJoltProfiles.Count > _selectedProfile && _gameJoltProfiles[_selectedProfile].IsOptionsMenuButton == false)
                    : false);
            DrawGradients((int)(255 * _fadeInMain), isGJ);
        }

        if (IsCurrentScreen() == true)
        {
            if (_loading == true)
            {
                String waitText = Localization.GetString("global_please_wait", "Please wait") + LoadingDots.Dots;
                Vector2 textSize = FontManager.InGameFont.MeasureString(waitText);
                GetFontRenderer().DrawString(FontManager.InGameFont, waitText,
                    new Vector2(Core.windowSize.Width / 2.0f - textSize.X / 2.0f, Core.windowSize.Height / 2.0f - textSize.Y / 2.0f + 100),
                    Color.White);
            }
            else
            {
                if (GameModeSplash != null)
                {
                    DrawGameModeSplash();
                }
                switch (_menuIndex)
                {
                    case 1:
                    case 3:
                        DrawOptionsProfiles(true);
                        break;
                    default:
                        DrawOptionsProfiles(false);
                        break;
                }
                DrawMainProfiles();
            }
        }
    }

    public void DrawGameModeSplash()
    {
        if (GameModeSplash == null) return;
        int origW = GameModeSplash.Width;
        int origH = GameModeSplash.Height;
        float aspectRatio = (float)origW / origH;

        int bw = Core.windowSize.Width;
        int bh = (int)(bw / aspectRatio);
        if (bh < Core.windowSize.Height)
        {
            bh = Core.windowSize.Height;
            bw = (int)(bh * aspectRatio);
        }

        int xOffset = 0;
        if (Core.windowSize.Width < bw)
        {
            float xAspectRatio = (float)origW / bw;
            xOffset = (int)(Math.Floor((bw - Core.windowSize.Width) * xAspectRatio) / 2);
        }

        Core.SpriteBatch.Draw(GameModeSplash, new Rectangle(0, 0, bw, bh), new Rectangle(xOffset, 0, origW, origH), Color.White);
    }

    public void DrawGameJoltButtons(Vector2 offset)
    {
        if (_oldMenuTexture == null) return;
        Rectangle r = new Rectangle((int)(offset.X + 400), (int)(offset.Y + 200), 512, 128);
        int y = 0;
        Color fontColor = Color.White;
        World.DayTimes dayTime = World.GetTime();
        if (dayTime == World.DayTimes.Day || dayTime == World.DayTimes.Morning) fontColor = Color.Black;

        y = Core.ScaleScreenRec(new Rectangle(r.X, r.Y, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible ||
            Core.GameInstance.IsMouseVisible == false && _gameJoltButtonIndex == 1 ? 16 : 0;
        if (y == 16) Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, Localization.GetString("main_menu_gamejolt_ChangeToMale", "Change to male"), new Vector2(r.X + 64 + 4, r.Y + 4), fontColor);
        Core.SpriteBatch.DrawInterface(_oldMenuTexture, new Rectangle(r.X, r.Y, 32, 32), new Rectangle(144, 32 + y, 16, 16), Color.White);

        y = Core.ScaleScreenRec(new Rectangle(r.X, r.Y + 48, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible ||
            Core.GameInstance.IsMouseVisible == false && _gameJoltButtonIndex == 2 ? 16 : 0;
        if (y == 16) Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, Localization.GetString("main_menu_gamejolt_ChangeToFemale", "Change to female"), new Vector2(r.X + 64 + 4, r.Y + 4 + 48), fontColor);
        Core.SpriteBatch.DrawInterface(_oldMenuTexture, new Rectangle(r.X, r.Y + 48, 32, 32), new Rectangle(160, 32 + y, 16, 16), Color.White);

        y = Core.ScaleScreenRec(new Rectangle(r.X, r.Y + 96, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible ||
            Core.GameInstance.IsMouseVisible == false && _gameJoltButtonIndex == 3 ? 16 : 0;
        if (y == 16) Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, Localization.GetString("main_menu_gamejolt_ChangeToGenderless", "Change to genderless"), new Vector2(r.X + 64 + 4, r.Y + 4 + 96), fontColor);
        Core.SpriteBatch.DrawInterface(_oldMenuTexture, new Rectangle(r.X, r.Y + 96, 32, 32), new Rectangle(208, 32 + y, 16, 16), Color.White);

        y = Core.ScaleScreenRec(new Rectangle(r.X, r.Y + 144, 32, 32)).Contains(MouseHandler.MousePosition) == true && Core.GameInstance.IsMouseVisible ||
            Core.GameInstance.IsMouseVisible == false && _gameJoltButtonIndex == 4 ? 16 : 0;
        if (y == 16) Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, Localization.GetString("main_menu_gamejolt_ResetSave", "Reset save"), new Vector2(r.X + 64 + 4, r.Y + 4 + 144), fontColor);
        Core.SpriteBatch.DrawInterface(_oldMenuTexture, new Rectangle(r.X, r.Y + 144, 32, 32), new Rectangle(176, 32 + y, 16, 16), Color.White);
    }

    private void DrawMainProfiles()
    {
        if (_menuTexture == null || _oldMenuTexture == null) return;

        for (int x = 0; x <= _mainProfiles.Count - 1; x++)
        {
            bool isSelected = x == _selectedProfile && _menuIndex == 0;
            float xOffset = _screenOrigin.X + _mainOffset.X + x * 180 + ((x + 1) * 100 * (1 - _fadeInMain));

            _mainProfiles[x].Draw(new Vector2((int)xOffset, (int)(_screenOrigin.Y + _screenOffset.Y)), (int)(_fadeInMain * 255), isSelected, _menuTexture, this);
            if (_mainProfiles[x].IsGameJolt == true && _mainProfiles[x].Loaded == true && isSelected == true)
            {
                DrawGameJoltButtons(new Vector2((int)xOffset, (int)(_screenOrigin.Y + _screenOffset.Y)));
            }
        }

        if (_fadeInMain == 1.0f && _menuIndex == 0)
        {
            Rectangle arrowSrc = _mainProfiles[_selectedProfile].IsGameJolt == false
                ? new Rectangle(0, 16, 32, 16)
                : new Rectangle(32, 16, 32, 16);
            Color arrowColor = _mainProfiles[_selectedProfile].IsGameJolt == false
                ? new Color(255, 255, 255, (int)(_fadeInMain * 255))
                : Color.White;
            Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(_screenOrigin.X + _sliderPosition - 16), (int)(_screenOrigin.Y + 170), 32, 16), arrowSrc, arrowColor);

            Rectangle displayRect = new Rectangle(
                (int)((_screenOrigin.X + _sliderPosition - 300).Clamp(20, Core.windowSize.Width - 620)),
                (int)(_screenOrigin.Y + 170 + 16),
                600, (int)(240 * _expandDisplay));

            if (_expandDisplay > 0f)
            {
                Color mainC = Screens.UI.ColorProvider.MainColor(_mainProfiles[_selectedProfile].IsGameJolt);
                Color accentC = Screens.UI.ColorProvider.AccentColor(_mainProfiles[_selectedProfile].IsGameJolt, (int)(255 * _expandDisplay));
                Canvas.DrawRectangle(displayRect, mainC);
                Canvas.DrawRectangle(new Rectangle(displayRect.X, displayRect.Y + displayRect.Height - 3, displayRect.Width, 3), accentC);
            }

            GameProfile tmpProfile = _mainProfiles[_selectedProfile];
            if (_expandDisplay == 1.0f)
            {
                if (tmpProfile.GameModeExists == true)
                {
                    for (int i = 0; i <= tmpProfile.PokemonTextures.Count - 1; i++)
                    {
                        Core.SpriteBatch.Draw(tmpProfile.PokemonTextures[i], new Rectangle(displayRect.X + 30 + i * 70, displayRect.Y + 70, 64, 64), Color.White);
                    }

                    String gameModeNameStr = GameModeManager.GetGameMode(tmpProfile.GameMode)?.Name ?? tmpProfile.GameMode;
                    if (Localization.TokenExists("gamemode_name_" + GameModeManager.GetGameMode(tmpProfile.GameMode)?.DirectoryName) == true)
                        gameModeNameStr = Localization.GetString("gamemode_name_" + GameModeManager.GetGameMode(tmpProfile.GameMode)?.DirectoryName);
                    else if (Localization.TokenExists(("gamemode_name_" + GameModeManager.GetGameMode(tmpProfile.GameMode)?.DirectoryName)?.ToLower() ?? String.Empty) == true)
                        gameModeNameStr = Localization.GetString(("gamemode_name_" + GameModeManager.GetGameMode(tmpProfile.GameMode)?.DirectoryName)?.ToLower() ?? String.Empty);

                    GetFontRenderer().DrawString(FontManager.InGameFont,
                        Localization.GetString("global_player_name", "Player Name") + ": " + tmpProfile.Name + Environment.NewLine +
                        Localization.GetString("global_gamemode", "GameMode") + ": " + gameModeNameStr,
                        new Vector2(displayRect.X + 30, displayRect.Y + 20), Color.White);
                    GetFontRenderer().DrawString(FontManager.InGameFont,
                        Localization.GetString("global_badges", "Badges") + ": " + tmpProfile.Badges + Environment.NewLine +
                        Localization.GetString("global_play_time", "Play Time") + ": " + tmpProfile.TimePlayed + Environment.NewLine +
                        Localization.GetString("global_location", "Location") + ": " + Localization.GetString("Places_" + tmpProfile.Location, tmpProfile.Location),
                        new Vector2(displayRect.X + 30, displayRect.Y + 150), Color.White);
                }
                else
                {
                    String gameModeNameStr = tmpProfile.GameMode;
                    if (Localization.TokenExists("gamemode_name_" + tmpProfile.GameMode) == true)
                        gameModeNameStr = Localization.GetString("gamemode_name_" + tmpProfile.GameMode);
                    else if (Localization.TokenExists(("gamemode_name_" + tmpProfile.GameMode).ToLower()) == true)
                        gameModeNameStr = Localization.GetString(("gamemode_name_" + tmpProfile.GameMode).ToLower());
                    if (gameModeNameStr == null) gameModeNameStr = String.Empty;

                    GetFontRenderer().DrawString(FontManager.InGameFont,
                        Localization.GetString("global_player_name", "Player Name") + ": " + tmpProfile.Name + Environment.NewLine +
                        Localization.GetString("global_gamemode", "GameMode") + ": " + gameModeNameStr,
                        new Vector2(displayRect.X + 30, displayRect.Y + 20), Color.White);

                    Core.SpriteBatch.Draw(_menuTexture, new Rectangle(displayRect.X + 30, displayRect.Y + 70, 32, 32), new Rectangle(0, 32, 32, 32), Color.White);

                    String errorText;
                    if (tmpProfile.IsGameJolt == true)
                    {
                        errorText = Localization.GetString("main_menu_error_gamejolt_1", "Download failed. Press Accept to try again.") + Environment.NewLine + Environment.NewLine +
                            Localization.GetString("main_menu_error_gamejolt_2", "If the problem persists, please try again later") + Environment.NewLine +
                            Localization.GetString("main_menu_error_gamejolt_3", "or contact us in our Discord server:") + Environment.NewLine + Environment.NewLine +
                            Localization.GetString("main_menu_error_gamejolt_4", "http://www.discord.me/p3d");
                    }
                    else
                    {
                        errorText = Localization.GetString("main_menu_error_gamemode_profile", "The required GameMode does not exist!");
                    }
                    GetFontRenderer().DrawString(FontManager.InGameFont, errorText, new Vector2(displayRect.X + 70, displayRect.Y + 78), Color.White);
                }
            }
        }
    }

    private void DrawOptionsProfiles(bool isGameJoltOptions)
    {
        if (_menuTexture == null) return;

        for (int x = 0; x <= _optionsProfiles.Count - 1; x++)
        {
            float xOffset = _screenOrigin.X + _optionsOffset.X + x * 180;
            _optionsProfiles[x].Draw(new Vector2((int)xOffset, (int)_screenOrigin.Y), (int)(_fadeInOptions * 255), x == _selectedProfile, _menuTexture, this);
        }

        switch (_menuIndex)
        {
            case 2:
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(_screenOrigin.X + _sliderPosition - 16), (int)(_screenOrigin.Y + 170), 32, 16), new Rectangle(0, 16, 32, 16), Color.White);
                break;
            case 3:
                Core.SpriteBatch.Draw(_menuTexture, new Rectangle((int)(_screenOrigin.X + _sliderPosition - 16), (int)(_screenOrigin.Y + 170), 32, 16), new Rectangle(32, 16, 32, 16), Color.White);
                break;
        }
    }

    private int GetSliderTarget(int index)
    {
        switch (_menuIndex)
        {
            case 2:
            case 3:
                return (int)(_optionsOffset.X + index * 180 + 80);
            case 1:
                return (int)(_gameJoltOffset.X + index * 180 + 80);
            default:
                return (int)(_screenOffset.X + index * 180 + 80);
        }
    }

    public override void ChangeTo()
    {
        if (_mainProfiles.Count == 0)
        {
            LoadMainProfiles();
        }
        if (_optionsProfiles.Count == 0)
        {
            LoadOptionProfiles();
        }
    }

    private void LoadMainProfiles()
    {
        _mainProfiles.Add(GameProfile.CreateQuitButton());
        _mainProfiles.Add(new GameProfile(String.Empty, false, true));
        _mainProfiles.Add(new GameProfile(String.Empty, false, false));

        String[] files = ["Apricorns.dat", "Berries.dat", "Box.dat", "Daycare.dat", "HallOfFame.dat",
            "ItemData.dat", "Items.dat", "NPC.dat", "Options.dat", "Party.dat", "Player.dat",
            "Pokedex.dat", "Register.dat", "RoamingPokemon.dat", "SecretBase.dat", "Statistics.dat"];

        foreach (String path in Directory.GetDirectories(AppPaths.SaveDir))
        {
            bool exists = true;
            foreach (String file in files)
            {
                if (File.Exists(path + @"\" + file) == false)
                {
                    exists = false;
                    break;
                }
            }
            if (exists == true)
            {
                _mainProfiles.Add(new GameProfile(path, false, false));
            }
        }

        GameModeManager.SetGameModePointer("Kolben");
        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\MainMenu");
        _oldMenuTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        _mainProfiles.Add(new GameProfile(String.Empty, true, false));
        _loading = false;
    }

    private void LoadOptionProfiles()
    {
        _optionsProfiles.Add(new GameProfile(String.Empty, false, false, 0));
        _optionsProfiles.Add(new GameProfile(String.Empty, false, false, 1));
        _optionsProfiles.Add(new GameProfile(String.Empty, false, false, 2));
        _optionsProfiles.Add(new GameProfile(String.Empty, false, false, 3));
    }

    // ---- Inner class: GameProfile ----

    public class GameProfile
    {
        // TODO Phase 9: set to false when online/GameJolt support is implemented
        private const bool OnlineDisabled = true;

        private bool _isQuitButton = false;
        private bool _isGameJolt = false;
        private bool _loaded = false;
        private bool _isLoading = false;
        private bool _failedGameJoltLoading = false;
        public int _OptionsMenuIndex = -1;
        private String _path = String.Empty;
        private bool _isNewGameButton = false;
        private bool _isOptionsMenuButton = false;

        private String _name = String.Empty;
        public String _gameMode = String.Empty;
        private int _badges;
        private String _timePlayed = String.Empty;
        private String _location = String.Empty;
        private List<Texture2D> _pokemonTextures = [];
        private Texture2D? _sprite;
        public bool _gameModeExists;
        private String _skin = String.Empty;
        private bool _surfing = false;
        private String _tempSurfSkin = String.Empty;

        private float _fontSize = 1.0f;
        private int _spriteIndex = 0;
        private float _spriteDelay = 1.5f;
        private float _logoBounce = 0f;
        private int[] _spriteOrder = [0, 1, 0, 2];

        public Texture2D? Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public bool IsGameJolt => _isGameJolt;
        public bool IsNewGameButton => _isNewGameButton;
        public bool IsOptionsMenuButton => _isOptionsMenuButton;
        public String Path => _path;
        public String Name => _name;
        public String GameMode => _gameMode;
        public int Badges => _badges;
        public String TimePlayed => _timePlayed;
        public String Location => _location;
        public List<Texture2D> PokemonTextures => _pokemonTextures;
        public bool Loaded => _loaded;
        public bool IsLoading => _isLoading;
        public bool GameModeExists => _gameModeExists;

        public void SetToDefault()
        {
            _timePlayed = "00:00:00";
            _location = "Your Room";
            _pokemonTextures.Clear();
            _badges = 0;
        }

        public static GameProfile CreateQuitButton() => new GameProfile(isQuit: true);

        private GameProfile(bool isQuit)
        {
            _isQuitButton = isQuit;
        }

        public GameProfile(String path, bool isNewGameButton, bool isOptionsMenuButton, int optionsMenuIndex = -1)
        {
            if (isNewGameButton == true)
            {
                _isNewGameButton = true;
            }
            else if (isOptionsMenuButton == true)
            {
                _isOptionsMenuButton = true;
                _sprite = TextureManager.GetTexture(@"Textures\UI\OptionsMenu");
            }
            else
            {
                if (optionsMenuIndex != -1)
                {
                    _OptionsMenuIndex = optionsMenuIndex;
                    switch (_OptionsMenuIndex)
                    {
                        case 0: _sprite = TextureManager.GetTexture(@"Textures\UI\Options\Language"); break;
                        case 1: _sprite = TextureManager.GetTexture(@"Textures\UI\Options\Audio"); break;
                        case 2: _sprite = TextureManager.GetTexture(@"Textures\UI\Options\Controls"); break;
                        case 3: _sprite = TextureManager.GetTexture(@"Textures\UI\Options\ContentPacks"); break;
                    }
                }
                else
                {
                    if (path == String.Empty)
                    {
                        _isGameJolt = true;
                        _loaded = false;
                        _sprite = TextureManager.GetTexture(@"Textures\UI\GameJolt\gameJoltIcon");
                        LoadGameJolt();
                    }
                    else
                    {
                        _path = path;
                        LoadFromPlayerData(File.ReadAllText(path + @"\Player.dat"));
                        LoadContent(File.ReadAllText(path + @"\Party.dat"));
                        _loaded = true;
                    }
                }
            }
        }

        private void LoadContent(String pokedata)
        {
            if (GameModeManager.GameModeExists(_gameMode) == true)
            {
                _gameModeExists = true;
                GameModeManager.SetGameModePointer(_gameMode);
                PokemonForms.Initialize();
                String[] pokemonData = pokedata.SplitAtNewline();
                foreach (String line in pokemonData)
                {
                    if (line.StartsWith("{") == true)
                    {
                        _pokemonTextures.Add(Pokemon.GetPokemonByData(line).GetMenuTexture(true));
                    }
                }

                if (_isGameJolt == false)
                {
                    _sprite = _surfing == true
                        ? TextureManager.GetTexture(@"Textures\NPC\" + _tempSurfSkin)
                        : TextureManager.GetTexture(@"Textures\NPC\" + _skin);
                }
            }
            else
            {
                _gameModeExists = false;
                _sprite = TextureManager.GetTexture(@"GUI\unknownSprite");
            }
        }

        private void LoadFromPlayerData(String data)
        {
            String[] playerData = data.SplitAtNewline();
            foreach (String line in playerData)
            {
                if (line.Contains("|") == true)
                {
                    String id = line.Split('|')[0];
                    String content = line.Split('|')[1];

                    switch (id.ToLower())
                    {
                        case "name":
                            _name = content;
                            while (FontManager.InGameFont.MeasureString(_name).X * _fontSize > 140) _fontSize -= 0.01f;
                            break;
                        case "badges":
                            _badges = content.Length > 0 && content != "0"
                                ? content.Split(',').Length
                                : 0;
                            break;
                        case "playtime":
                            String[] timedata = content.Split(',');
                            int hours = int.Parse(timedata[0]) + int.Parse(timedata[3]) * 24;
                            int minutes = int.Parse(timedata[1]);
                            int seconds = int.Parse(timedata[2]);
                            _timePlayed = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
                            break;
                        case "location":
                            _location = content;
                            break;
                        case "gamemode":
                            _gameMode = content;
                            break;
                        case "skin":
                            _skin = content;
                            break;
                        case "surfing":
                            _surfing = content == "1";
                            break;
                        case "tempsurfskin":
                            _tempSurfSkin = content;
                            break;
                    }
                }
            }
        }

        public void Draw(Vector2 offset, int alpha, bool isSelected, Texture2D t, NewMainMenuScreen parent)
        {
            if (_isGameJolt == true)
            {
                bool isDisabledSlot = OnlineDisabled && _loaded == false && _isLoading == false;
                Color tileColor = isDisabledSlot ? new Color(160, 160, 160, alpha) : new Color(255, 255, 255, alpha);
                for (int x = 0; x <= 9; x++)
                    for (int y = 0; y <= 9; y++)
                        Core.SpriteBatch.Draw(t, new Rectangle((int)(x * 16 + offset.X), (int)(y * 16 + offset.Y), 16, 16), new Rectangle(32, 0, 16, 16), tileColor);
                Color accentColor = isDisabledSlot ? new Color(100, 100, 100, alpha) : Screens.UI.ColorProvider.AccentColor(true, alpha);
                Canvas.DrawRectangle(new Rectangle((int)offset.X, (int)offset.Y, 160, 3), accentColor);

                if (_isLoading == true && Core.GameJoltSave.DownloadProgress > 0)
                {
                    int width = (int)((Core.GameJoltSave.DownloadProgress / (float)(GameJolt.GamejoltSave.SAVEFILECOUNT + GameJolt.GamejoltSave.EXTRADATADOWNLOADCOUNT)) * 160);
                    Canvas.DrawRectangle(new Rectangle((int)offset.X, (int)(offset.Y + 3), width, 157), new Color(100, 100, 100, 128));
                }
            }
            else if (_menuIndex == 3)
            {
                for (int x = 0; x <= 9; x++)
                    for (int y = 0; y <= 9; y++)
                        Core.SpriteBatch.Draw(t, new Rectangle((int)(x * 16 + offset.X), (int)(y * 16 + offset.Y), 16, 16), new Rectangle(32, 0, 16, 16), new Color(255, 255, 255, alpha));
                Canvas.DrawRectangle(new Rectangle((int)offset.X, (int)offset.Y, 160, 3), Screens.UI.ColorProvider.AccentColor(true, alpha));
            }
            else
            {
                for (int x = 0; x <= 9; x++)
                    for (int y = 0; y <= 9; y++)
                        Core.SpriteBatch.Draw(t, new Rectangle((int)(x * 16 + offset.X), (int)(y * 16 + offset.Y), 16, 16), new Rectangle(0, 0, 16, 16), new Color(255, 255, 255, alpha));
                Canvas.DrawRectangle(new Rectangle((int)offset.X, (int)offset.Y, 160, 3), Screens.UI.ColorProvider.AccentColor(false, alpha));
            }

            if (_isQuitButton == true)
            {
                String textA = Localization.GetString("main_menu_quit_line1", "Quit");
                String textB = Localization.GetString("main_menu_quit_line2", "Game");

                if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                {
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                }
            }
            else if (_isNewGameButton == true)
            {
                String textA = Localization.GetString("main_menu_newgame_line1", "New");
                String textB = Localization.GetString("main_menu_newgame_line2", "Game");

                if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                {
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + 72 - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 72 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                }
            }
            else if (_isOptionsMenuButton == true)
            {
                String text = Localization.GetString("main_menu_options", "Options");
                if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                {
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, text, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(text).X / 2 + 2), (int)(offset.Y + 132 - FontManager.InGameFont.MeasureString(text).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, text, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(text).X / 2), (int)(offset.Y + 132 - FontManager.InGameFont.MeasureString(text).Y / 2)), new Color(255, 255, 255, alpha));
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(text).X / 2 + 2), (int)(offset.Y + 132 - FontManager.InGameFont.MeasureString(text).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(text).X / 2), (int)(offset.Y + 132 - FontManager.InGameFont.MeasureString(text).Y / 2)), new Color(255, 255, 255, alpha));
                }
                if (_menuIndex == 0) _logoBounce = isSelected == true ? _logoBounce + 0.2f : 0f;
                if (_sprite != null) Core.SpriteBatch.Draw(_sprite, new Rectangle((int)(offset.X + 40), (int)(offset.Y + 36 + Math.Sin(_logoBounce) * 8.0f), 80, 80), new Color(255, 255, 255, alpha));
            }
            else if (_OptionsMenuIndex != -1)
            {
                String textA = String.Empty;
                String textB = String.Empty;
                switch (_OptionsMenuIndex)
                {
                    case 0: textA = Localization.GetString("main_menu_options_language", "Language"); break;
                    case 1: textA = Localization.GetString("main_menu_options_audio", "Audio"); break;
                    case 2: textA = Localization.GetString("main_menu_options_controls", "Controls"); break;
                    case 3:
                        textA = Localization.GetString("main_menu_options_contentpacks_line1", "Content");
                        textB = Localization.GetString("main_menu_options_contentpacks_line2", "Packs");
                        break;
                }

                if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                {
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + (textB == String.Empty ? 132 : 116) - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + (textB == String.Empty ? 132 : 116) - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    if (textB != String.Empty)
                    {
                        parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 116 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                        parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 116 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                    }
                }
                else
                {
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2 + 2), (int)(offset.Y + (textB == String.Empty ? 132 : 116) - FontManager.InGameFont.MeasureString(textA).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, textA, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X / 2), (int)(offset.Y + (textB == String.Empty ? 132 : 116) - FontManager.InGameFont.MeasureString(textA).Y / 2)), new Color(255, 255, 255, alpha));
                    if (textB != String.Empty)
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2 + 2), (int)(offset.Y + 116 + FontManager.InGameFont.MeasureString(textB).Y / 2 + 2)), new Color(0, 0, 0, alpha));
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, textB, new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X / 2), (int)(offset.Y + 116 + FontManager.InGameFont.MeasureString(textB).Y / 2)), new Color(255, 255, 255, alpha));
                    }
                }
                if (_menuIndex == 2 || _menuIndex == 3) _logoBounce = isSelected == true ? _logoBounce + 0.2f : 0f;
                if (_sprite != null) Core.SpriteBatch.Draw(_sprite, new Rectangle((int)(offset.X + 40), (int)(offset.Y + 24 + Math.Sin(_logoBounce) * 8.0f), 80, 80), new Color(255, 255, 255, alpha));
            }
            else
            {
                if (_loaded == true && _sprite != null)
                {
                    Size frameSize;
                    if (_sprite.Width == _sprite.Height / 2)
                        frameSize = new Size(_sprite.Width / 2, _sprite.Height / 4);
                    else if (_sprite.Width == _sprite.Height)
                        frameSize = new Size(_sprite.Width / 4, _sprite.Height / 4);
                    else
                        frameSize = new Size(_sprite.Width / 3, _sprite.Height / 4);

                    if (isSelected == true)
                    {
                        _spriteDelay -= 0.1f;
                        if (_spriteDelay <= 0f)
                        {
                            _spriteDelay = 1.5f;
                            _spriteIndex += 1;
                            if (_spriteIndex == _spriteOrder.Length) _spriteIndex = 0;
                        }
                    }
                    else
                    {
                        _spriteIndex = 0;
                    }

                    Core.SpriteBatch.Draw(_sprite, new Rectangle((int)(offset.X + 17), (int)(offset.Y - 10), 128, 128),
                        new Rectangle(frameSize.Width * _spriteOrder[_spriteIndex], frameSize.Height * 2, frameSize.Width, frameSize.Height),
                        new Color(255, 255, 255, alpha));

                    if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                    {
                        parent.GetFontRenderer().DrawString(FontManager.InGameFont, _name,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(_name).X * _fontSize / 2 + 2), (int)(offset.Y + 120 + 2)),
                            new Color(0, 0, 0, alpha), 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                        parent.GetFontRenderer().DrawString(FontManager.InGameFont, _name,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(_name).X * _fontSize / 2), (int)(offset.Y + 120)),
                            new Color(255, 255, 255, alpha), 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, _name,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(_name).X * _fontSize / 2 + 2), (int)(offset.Y + 120 + 2)),
                            new Color(0, 0, 0, alpha), 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, _name,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(_name).X * _fontSize / 2), (int)(offset.Y + 120)),
                            new Color(255, 255, 255, alpha), 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    if (_menuIndex == 0) _logoBounce = isSelected == true ? _logoBounce + 0.2f : 0f;

                    bool isDisabledSlot = OnlineDisabled && _isLoading == false;
                    String textA = _isLoading == true
                        ? Localization.GetString("global_loading", "Loading") + "..."
                        : Localization.GetString("global_login", "Log in");
                    String textB = isDisabledSlot
                        ? Localization.GetString("main_menu_online_disabled", "(Disabled)")
                        : String.Empty;
                    Color textColor = isDisabledSlot ? new Color(120, 120, 120, alpha) : new Color(255, 255, 255, alpha);
                    Color iconColor = isDisabledSlot ? new Color(100, 100, 100, alpha) : new Color(255, 255, 255, alpha);

                    if (_sprite != null) Core.SpriteBatch.Draw(_sprite, new Rectangle((int)(offset.X + 46), (int)(offset.Y + 36 + Math.Sin(_logoBounce) * 8.0f), 68, 72), iconColor);

                    if (alpha >= 250 && Core.CurrentScreen?.Identification == Identifications.MainMenuScreen)
                    {
                        int yA = textB.Length > 0 ? (int)(offset.Y + 110) : (int)(offset.Y + 120);
                        parent.GetFontRenderer().DrawString(FontManager.InGameFont, textA,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X * _fontSize / 2), yA),
                            textColor, 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                        if (textB.Length > 0)
                            parent.GetFontRenderer().DrawString(FontManager.InGameFont, textB,
                                new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X * _fontSize / 2), (int)(offset.Y + 130)),
                                textColor, 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                    }
                    else
                    {
                        int yA = textB.Length > 0 ? (int)(offset.Y + 110) : (int)(offset.Y + 120);
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, textA,
                            new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textA).X * _fontSize / 2), yA),
                            textColor, 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                        if (textB.Length > 0)
                            Core.SpriteBatch.DrawString(FontManager.InGameFont, textB,
                                new Vector2((int)(offset.X + 80 - FontManager.InGameFont.MeasureString(textB).X * _fontSize / 2), (int)(offset.Y + 130)),
                                textColor, 0f, Vector2.Zero, new Vector2(_fontSize), SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public void LoadGameJolt()
        {
            if (GameJolt.API.LoggedIn == true)
            {
                if (_isGameJolt == true && _loaded == false && _isLoading == false)
                {
                    _isLoading = true;
                    Core.GameJoltSave.DownloadSave(GameJolt.LogInScreen.LoadedGameJoltID, true);
                }
                else if (_isGameJolt == true && _loaded == false && _isLoading == true)
                {
                    if (Core.GameJoltSave.DownloadFinished == true)
                    {
                        _loaded = true;
                        _isLoading = false;
                        _sprite = Core.GameJoltSave.DownloadedSprite as Texture2D;
                        if (_sprite == null)
                        {
                            _sprite = GameJolt.Emblem.GetPlayerSprite(GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points), Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
                        }
                        LoadFromPlayerData(Core.GameJoltSave.Player);
                        LoadContent(Core.GameJoltSave.Party);
                    }
                    else if (Core.GameJoltSave.DownloadFailed == true)
                    {
                        _loaded = true;
                        _isLoading = false;
                        _failedGameJoltLoading = true;
                        _sprite = TextureManager.GetTexture(@"GUI\unknownSprite");
                    }
                }
            }
        }

        public void UnSelectProfile()
        {
            if (_isGameJolt == true && _loaded == true)
            {
                _loaded = false;
                _isLoading = false;
                _pokemonTextures.Clear();
                _sprite = TextureManager.GetTexture(@"Textures\UI\GameJolt\gameJoltIcon");
                _fontSize = 1.0f;
                GameJolt.API.LoggedIn = false;
            }
        }

        public void SelectProfile(NewMainMenuScreen parent)
        {
            if (_isQuitButton == true)
            {
                Core.GameInstance.Exit();
                return;
            }

            switch (_menuIndex)
            {
                case 0:
                    if (_isGameJolt == true && _loaded == false && GameJolt.API.LoggedIn == false)
                    {
                        // TODO Phase 9: remove the OnlineDisabled guard when GameJolt is implemented
                        if (!OnlineDisabled)
                            Core.SetScreen(new GameJolt.LogInScreen(Core.CurrentScreen!));
                    }
                    else if (_isNewGameButton == true)
                    {
                        World.IsMainMenu = false;
                        Core.SetScreen(new GameModeSelectionScreen(Core.CurrentScreen!));
                    }
                    else
                    {
                        if (_gameModeExists == true)
                        {
                            if (GameModeManager.ActiveGameMode?.DirectoryName != _gameMode)
                            {
                                GameModeManager.SetGameModePointer(_gameMode);
                                Localization.ReloadGameModeTokens();
                            }
                            MusicManager.Clear();
                            SoundManager.Clear();
                            FontManager.LoadFonts();
                            Water.ClearAnimationResources();
                            Waterfall.ClearAnimationResources();
                            Water.AddDefaultWaterAnimationResources();
                            Waterfall.AddDefaultWaterAnimationResources();
                            AnimatedBlock.ClearAnimationResources();

                            World.IsMainMenu = false;
                            if (_isGameJolt == true)
                            {
                                Core.Player.IsGameJoltSave = true;
                                Core.Player.LoadGame("GAMEJOLTSAVE");
                                Core.SetScreen(new JoinServerScreen(Core.CurrentScreen!));
                            }
                            else
                            {
                                Core.Player.IsGameJoltSave = false;
                                Core.Player.LoadGame(System.IO.Path.GetFileName(_path));
                                Core.SetScreen(new JoinServerScreen(Core.CurrentScreen!));
                            }
                        }
                        else
                        {
                            if (_isGameJolt == true)
                            {
                                _loaded = false;
                                _sprite = TextureManager.GetTexture(@"Textures\UI\GameJolt\gameJoltIcon");
                                LoadGameJolt();
                            }
                            else if (_isOptionsMenuButton == false)
                            {
                                MessageBox messageBox = new MessageBox(Core.CurrentScreen!);
                                messageBox.Show(Localization.GetString("main_menu_error_gamemode_message", "The required GameMode does not exist.~Reaquire the GameMode to play on this profile.")
                                    .Replace("~", Environment.NewLine));
                            }
                            else
                            {
                                _menuIndex = 2;
                                _selectedProfileTemp = _selectedProfile;
                                _selectedProfile = 0;
                            }
                        }
                    }
                    break;
                case 2:
                case 3:
                    Core.SetScreen(new NewOptionScreen(Core.CurrentScreen!, _OptionsMenuIndex + 1));
                    break;
            }
        }
    }
}
