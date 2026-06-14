using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class AddServerScreen : Screen
{
    private List<String> _serverNames = [];

    private String _identifyName = String.Empty;
    private String _address = String.Empty;

    private int _index = 0;
    private int _buttonIndex = 0;

    private bool _newServer = true;
    private Server? _editServer = null;

    public AddServerScreen(Screen currentScreen, List<Server> servers, bool newServer, Server editServer)
    {
        PreScreen = currentScreen;
        Identification = Identifications.AddServerScreen;

        foreach (Server s in servers)
        {
            _serverNames.Add(s.IdentifierName.ToLower());
        }

        CanBePaused = false;
        CanChat = false;
        CanMuteAudio = false;
        MouseVisible = true;

        _newServer = newServer;
        _editServer = editServer;

        if (newServer == false)
        {
            _address = editServer.GetAddressString();
            _identifyName = editServer.IdentifierName;
        }
    }

    public override void Draw()
    {
        int tx = (int)World.CurrentSeason;
        int ty = 0;
        if (tx > 1)
        {
            tx -= 2;
            ty += 1;
        }

        Texture2D pattern = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(160 + tx * 16, ty * 16, 16, 16), String.Empty);
        for (int dx = 0; dx <= Core.windowSize.Width; dx += 128)
        {
            for (int dy = 0; dy <= Core.windowSize.Height; dy += 128)
            {
                Core.SpriteBatch.Draw(pattern, new Rectangle(dx, dy, 128, 128), Color.White);
            }
        }

        Canvas.DrawRectangle(new Rectangle(0, 75, Core.windowSize.Width, 680 - 240), new Color(0, 0, 0, 128));

        String title = Localization.GetString("add_server_screen_title", "Add a server:");
        Core.SpriteBatch.DrawString(FontManager.MainFont, title,
            new Vector2((float)(Core.windowSize.Width / 2 - FontManager.MainFont.MeasureString(title).X), 14),
            Color.White, 0.0f, new Vector2(0), 2.0f, SpriteEffects.None, 0.0f);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("add_server_screen_name", "Name:"),
            new Vector2((float)(Core.windowSize.Width / 2 - 300), 140), Color.White);
        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 300), 170, 600, 40), new Color(40, 40, 40, 255));

        if (_index == 0)
        {
            Canvas.DrawBorder(3, new Rectangle((int)(Core.windowSize.Width / 2 - 300), 170, 600, 40), new Color(220, 220, 220, 255));
        }
        else
        {
            Canvas.DrawBorder(3, new Rectangle((int)(Core.windowSize.Width / 2 - 300), 170, 600, 40), new Color(100, 100, 100, 255));
        }

        String nameDisplay = _identifyName;
        if (nameDisplay.Length < 30 && _index == 0)
        {
            nameDisplay += "_";
            if (_identifyName == String.Empty && ControllerHandler.IsConnected() == true)
            {
                nameDisplay = Localization.GetString("add_server_screen_hint_controller_edit", "Press X to edit.");
            }
        }
        Core.SpriteBatch.DrawString(FontManager.MainFont, nameDisplay,
            new Vector2((float)(Core.windowSize.Width / 2 - 294), 175), Color.White);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("add_server_screen_address", "Address:"),
            new Vector2((float)(Core.windowSize.Width / 2 - 300), 270), Color.White);
        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 300), 300, 600, 40), new Color(40, 40, 40, 255));

        if (_index == 1)
        {
            Canvas.DrawBorder(3, new Rectangle((int)(Core.windowSize.Width / 2 - 300), 300, 600, 40), new Color(220, 220, 220, 255));
        }
        else
        {
            Canvas.DrawBorder(3, new Rectangle((int)(Core.windowSize.Width / 2 - 300), 300, 600, 40), new Color(100, 100, 100, 255));
        }

        String addrDisplay = _address;
        if (addrDisplay.Length < 30 && _index == 1)
        {
            addrDisplay += "_";
            if (_address == String.Empty && ControllerHandler.IsConnected() == true)
            {
                addrDisplay = Localization.GetString("add_server_screen_hint_controller_edit", "Press X to edit.");
            }
        }
        Core.SpriteBatch.DrawString(FontManager.MainFont, addrDisplay,
            new Vector2((float)(Core.windowSize.Width / 2 - 294), 305), Color.White);

        for (int i = 0; i <= 1; i++)
        {
            String text = i == 0
                ? Localization.GetString("global_done", "Done")
                : Localization.GetString("global_back", "Back");

            Texture2D canvasTexture = i == _buttonIndex
                ? TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty)
                : TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(Core.windowSize.Width / 2) - 180 + i * 192, 544, 128, 64));
            Core.SpriteBatch.DrawString(FontManager.InGameFont, text,
                new Vector2((int)(Core.windowSize.Width / 2) - 162 + i * 192, 578), Color.Black);
        }

        if (IsValid() != String.Empty)
        {
            Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 300), 430, 600, 40), new Color(40, 40, 40, 255));
            Canvas.DrawBorder(3, new Rectangle((int)(Core.windowSize.Width / 2 - 300), 430, 600, 40), new Color(200, 200, 200, 255));
            Core.SpriteBatch.DrawString(FontManager.MainFont, IsValid(),
                new Vector2((int)(Core.windowSize.Width / 2 - 294), 436), new Color(180, 0, 0, 255));
        }

        var d = new Dictionary<Buttons, String>
        {
            { Buttons.A, Localization.GetString("game_interaction_accept", "Accept") },
            { Buttons.B, Localization.GetString("game_interaction_back", "Back") },
            { Buttons.X, Localization.GetString("game_interaction_edit", "Edit") },
            { Buttons.Back, Localization.GetString("game_interaction_clear", "Clear") }
        };
        DrawGamePadControls(d);
    }

    public override void Update()
    {
        if (KeyBoardHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Tab) == true)
        {
            if (Controls.ShiftDown() == true)
            {
                _index = 0;
            }
            else
            {
                _index = 1;
            }
        }
        if (Controls.Up(true, true, false, false, true, true) == true)
        {
            _index = 0;
        }
        if (Controls.Down(true, true, false, false, true, true) == true)
        {
            _index = 1;
        }
        if (Controls.Left(true, true, true, false, true, true) == true)
        {
            _buttonIndex = 0;
        }
        if (Controls.Right(true, true, true, false, true, true) == true)
        {
            _buttonIndex = 1;
        }

        if (Controls.Accept(true, false, false) == true)
        {
            if (new Rectangle((int)(Core.windowSize.Width / 2 - 300), 170, 600, 40).Contains(MouseHandler.MousePosition))
            {
                _index = 0;
            }
            if (new Rectangle((int)(Core.windowSize.Width / 2 - 300), 300, 600, 40).Contains(MouseHandler.MousePosition))
            {
                _index = 1;
            }
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (new Rectangle((int)(Core.windowSize.Width / 2) - 180 + i * 192, 544, 128 + 32, 64 + 32).Contains(MouseHandler.MousePosition))
                {
                    if (_buttonIndex == i)
                    {
                        if (Controls.Accept(true, false, false) == true)
                        {
                            if (i == 0)
                            {
                                SoundManager.PlaySound("select");
                                ButtonDone();
                            }
                            else
                            {
                                SoundManager.PlaySound("select");
                                ButtonCancel();
                            }
                        }
                    }
                    else
                    {
                        _buttonIndex = i;
                    }
                }
            }
        }

        switch (_index)
        {
            case 0:
                KeyBindings.GetInput(ref _identifyName, 30, true, true);
                if (ControllerHandler.ButtonPressed(Buttons.X) == true)
                {
                    Core.SetScreen(new InputScreen(this, Localization.GetString("add_server_screen_input_name_default", "Pokémon 3D Server"), InputScreen.InputModes.Text, _identifyName, 30, [], AcceptName));
                }
                if (ControllerHandler.ButtonPressed(Buttons.Back) == true)
                {
                    _identifyName = String.Empty;
                }
                break;
            case 1:
                KeyBindings.GetInput(ref _address, 30, true, true);
                if (ControllerHandler.ButtonPressed(Buttons.X) == true)
                {
                    Core.SetScreen(new InputScreen(this, "127.0.0.1", InputScreen.InputModes.Text, _address, 30, [], AcceptAddress));
                }
                if (ControllerHandler.ButtonPressed(Buttons.Back) == true)
                {
                    _address = String.Empty;
                }
                break;
        }

        switch (_buttonIndex)
        {
            case 0:
                if (Controls.Accept(false, false, true) == true || KeyBoardHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter) == true)
                {
                    SoundManager.PlaySound("select");
                    ButtonDone();
                }
                break;
            case 1:
                if (Controls.Accept(false, false, true) == true || KeyBoardHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter) == true)
                {
                    SoundManager.PlaySound("select");
                    ButtonCancel();
                }
                break;
        }

        if (Controls.Dismiss(true, false, true) == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(PreScreen);
        }
    }

    private void AcceptName(String input)
    {
        _identifyName = input;
    }

    private void AcceptAddress(String input)
    {
        _address = input;
    }

    private String IsValid()
    {
        if (_identifyName == String.Empty)
        {
            return Localization.GetString("add_server_screen_validation_empty_name", "The server name cannot be empty.");
        }
        if (_address == String.Empty)
        {
            return Localization.GetString("add_server_screen_validation_empty_address", "The address cannot be empty.");
        }
        if (_serverNames.Contains(_identifyName.ToLower()) == true)
        {
            return Localization.GetString("add_server_screen_validation_duplicate_server", "This server name already exists on the list.");
        }
        return String.Empty;
    }

    private void ButtonDone()
    {
        if (IsValid() == String.Empty)
        {
            List<String> data = File.ReadAllLines(GameController.GamePath + @"\Save\server_list.dat").ToList();
            data.Add(_identifyName + "," + _address);
            File.WriteAllLines(GameController.GamePath + @"\Save\server_list.dat", data.ToArray());
            Core.SetScreen(PreScreen);
        }
    }

    private void ButtonCancel()
    {
        if (_newServer == false)
        {
            List<String> data = File.ReadAllLines(GameController.GamePath + @"\Save\server_list.dat").ToList();
            data.Add(_editServer!.ToString());
            File.WriteAllLines(GameController.GamePath + @"\Save\server_list.dat", data.ToArray());
        }
        Core.SetScreen(PreScreen);
    }
}
