using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NameObjectScreen : Screen
{
    private Pokemon? _pokemon;
    private String _currentText = String.Empty;
    private Texture2D _mainTexture;
    private int _maxLength = 20;

    private int _index = 0;
    private bool _askedRename = false;
    private bool _renamePokemon = false;

    private bool _canChooseNo = true;
    private String _defaultName = "Name";
    private float _delay = 0.0f;

    public delegate void DNameAcceptEventHandler(String name);
    private DNameAcceptEventHandler? _acceptName;

    public NameObjectScreen(Screen currentScreen, Pokemon pokemon)
    {
        Identification = Identifications.NameObjectScreen;
        PreScreen = currentScreen;
        MouseVisible = true;
        CanChat = false;
        CanMuteAudio = false;
        CanBePaused = false;
        _canChooseNo = true;
        _pokemon = pokemon;
        _defaultName = pokemon.GetDisplayName();
        _renamePokemon = true;
        _maxLength = 12;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        Screen.PokemonImageView.Show(pokemon, true);
    }

    public NameObjectScreen(Screen currentScreen, Texture2D texture, bool canExit, bool canChoose, String nameString, String defaultName, DNameAcceptEventHandler? acceptName)
    {
        Identification = Identifications.NameObjectScreen;
        PreScreen = currentScreen;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        _acceptName = acceptName;
        _canChooseNo = canExit;
        if (canChoose == false)
            _delay = 5.0f;
        _askedRename = canChoose == false;
        _defaultName = nameString;
        _currentText = defaultName;
        _renamePokemon = false;
        MouseVisible = true;
        CanBePaused = true;
        CanChat = false;
        CanMuteAudio = false;
        _maxLength = 20;
        Screen.PokemonImageView.Show(texture);
    }

    public override void Draw()
    {
        PreScreen?.Draw();
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));

        for (int i = 0; i <= 1; i++)
        {
            String text = _askedRename == true
                ? Localization.GetString("global_ok", "OK")
                : Localization.GetString("rename_screen_button_Rename", "Rename");

            if (i == 1)
                text = Localization.GetString("global_cancel", "Cancel");

            Texture2D canvasTexture = i == _index
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            if (i == 0 || (i == 1 && _canChooseNo == true))
            {
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(
                    (int)(Core.windowSize.Width / 2) - 182 + i * 192 + 22,
                    Core.windowSize.Height - 128, 128, 64));
                Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2(
                    (int)(Core.windowSize.Width / 2) - 164 + i * 192 + 22,
                    Core.windowSize.Height - 96), Color.Black);
            }
        }

        if (_askedRename == false)
        {
            String genderString = GetGenderString();
            String titleText = Localization.GetString("rename_screen_title_Question", "Rename [NAME]?")
                .Replace("[NAME]", _defaultName + genderString);
            int titleWidth = (int)FontManager.InGameFont.MeasureString(titleText).X;
            Core.SpriteBatch.DrawString(FontManager.InGameFont, titleText, new Vector2((int)(Core.windowSize.Width / 2) - titleWidth / 2 + 16 + 2, 96 + 2), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, titleText, new Vector2((int)(Core.windowSize.Width / 2) - titleWidth / 2 + 16, 96), Color.White);
            ChooseBox.Showing = false;
        }
        else
        {
            if (_delay == 0.0f)
            {
                String genderString = GetGenderString();
                String titleText = Localization.GetString("rename_screen_title_EnterName", "Enter name for [NAME]:")
                    .Replace("[NAME]", _defaultName + genderString);
                int titleWidth = (int)FontManager.InGameFont.MeasureString(titleText).X;
                Core.SpriteBatch.DrawString(FontManager.InGameFont, titleText, new Vector2((int)(Core.windowSize.Width / 2) - titleWidth / 2 + 16 + 2, 96 + 2), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.InGameFont, titleText, new Vector2((int)(Core.windowSize.Width / 2) - titleWidth / 2 + 16, 96), Color.White);

                Canvas.DrawRectangle(new Rectangle((int)TextboxPosition().X - 4, (int)TextboxPosition().Y - 4, 320 + 8, 32), new Color(101, 142, 255));
                DrawTextBox();
            }
        }

        PokemonImageView.Draw();
        ImageView.Draw();
    }

    private String GetGenderString()
    {
        if (_renamePokemon == true && _pokemon != null)
        {
            if (_pokemon.Gender == Pokemon.Genders.Male) return " ♂";
            if (_pokemon.Gender == Pokemon.Genders.Female) return " ♀";
        }
        return String.Empty;
    }

    private Vector2 TextboxPosition()
    {
        return new Vector2((int)(Core.windowSize.Width / 2) - 160 + 16, 140);
    }

    private void DrawTextBox()
    {
        Canvas.DrawRectangle(new Rectangle((int)TextboxPosition().X, (int)TextboxPosition().Y, 320, 24), Color.White);
        String t = _currentText;
        if (t.Length < _maxLength) t += "_";
        Core.SpriteBatch.DrawString(FontManager.InGameFont, t, TextboxPosition(), Color.Black);
    }

    public override void Update()
    {
        if (bool.Parse(GameModeManager.GetGameRuleValue("ForceRename", "0")) == true)
        {
            _askedRename = true;
            _canChooseNo = false;
        }

        for (int i = 0; i <= 1; i++)
        {
            if (new Rectangle((int)(Core.windowSize.Width / 2) - 182 + i * 192, Core.windowSize.Height - 128, 128 + 32, 64 + 32).Contains(MouseHandler.MousePosition) == true)
                _index = i;
        }

        if (Controls.Accept(true, false, true) == true || KeyBoardHandler.KeyPressed(KeyBindings.EnterKey1) == true)
        {
            switch (_index)
            {
                case 0:
                    SoundManager.PlaySound("select");
                    ClickYes();
                    break;
                case 1:
                    if (_canChooseNo == true)
                    {
                        SoundManager.PlaySound("select");
                        ClickNo();
                    }
                    break;
            }
        }

        if (_askedRename == true)
        {
            if (Controls.Right(true, true, true, false, true) == true) _index = 1;
            if (Controls.Left(true, true, true, false, true) == true) _index = 0;

            if (_delay > 0.0f)
            {
                _delay -= 0.1f;
                if (_delay <= 0.0f) _delay = 0.0f;
            }
            else
            {
                if (_pokemon != null)
                    KeyBindings.GetNameInput(ref _currentText, 12);
                else
                    KeyBindings.GetNameInput(ref _currentText, 20);

                _currentText = ReplaceInvalidChars(_currentText);

                if (Controls.Dismiss(true, false, true) == true && _canChooseNo == true)
                {
                    SoundManager.PlaySound("select");
                    ClickNo();
                }
            }
        }
        else
        {
            if (Controls.Right(true, true, true, true, true) == true) _index = 1;
            if (Controls.Left(true, true, true, true, true) == true) _index = 0;
        }
    }

    private String ReplaceInvalidChars(String text)
    {
        char[] chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ".ToCharArray();
        String newText = String.Empty;
        for (int i = 0; i <= text.Length - 1; i++)
        {
            if (chars.Contains(text[i]) == true)
                newText += text[i].ToString();
        }
        return newText;
    }

    private void ClickYes()
    {
        if (_askedRename == true)
        {
            while (_currentText.StartsWith(" ") == true) _currentText = _currentText.Remove(0, 1);
            while (_currentText.EndsWith(" ") == true) _currentText = _currentText.Remove(_currentText.Length - 1, 1);

            if (_currentText != String.Empty)
            {
                if (_renamePokemon == true)
                {
                    if (_pokemon!.GetName() != _currentText)
                        _pokemon.NickName = _currentText;
                }
                else
                {
                    _acceptName?.Invoke(_currentText);
                }
                PokemonImageView.Showing = false;
                Core.SetScreen(PreScreen!);
            }
        }
        else
        {
            _askedRename = true;
            if (ControllerHandler.IsConnected() == true)
            {
                if (_pokemon != null)
                    Core.SetScreen(new InputScreen(this, _defaultName, InputScreen.InputModes.Pokemon, _defaultName, 12, [], GetControllerInput));
                else
                    Core.SetScreen(new InputScreen(this, _defaultName, InputScreen.InputModes.Name, _defaultName, 20, [], GetControllerInput));
            }
        }
    }

    private void ClickNo()
    {
        if (_askedRename == true)
            _askedRename = false;
        else
        {
            PokemonImageView.Showing = false;
            Core.SetScreen(PreScreen!);
        }
    }

    public void GetControllerInput(String input)
    {
        _currentText = ReplaceInvalidChars(input);
    }
}
