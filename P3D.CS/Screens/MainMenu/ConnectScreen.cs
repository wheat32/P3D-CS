using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class ConnectScreen : Screen
{
    public enum Modes
    {
        Connect,
        Disconnect
    }

    public Modes MyMode = Modes.Connect;

    private String _message = String.Empty;
    private String _header = String.Empty;
    private bool _quitToMenu = true;

    public static bool Connected = false;

    public ConnectScreen(Modes myMode, String header, String message, Screen currentScreen)
    {
        PreScreen = Core.CurrentScreen;
        MyMode = myMode;
        _message = message;
        _header = header;
        PreScreen = currentScreen;
        MouseVisible = true;

        if (currentScreen != null)
        {
            Screen s = PreScreen;
            while (s.PreScreen != null && s.Identification != Identifications.OverworldScreen)
            {
                s = s.PreScreen;
            }

            if (s.Identification == Identifications.OverworldScreen)
            {
                _quitToMenu = false;
            }
            else
            {
                if (s.Identification == Identifications.BattleScreen)
                {
                    _quitToMenu = false;
                }
            }
        }

        Identification = Identifications.ConnectScreen;
        CanBePaused = false;
        CanChat = false;
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
                Color c = Color.White;
                if (dy == 128)
                {
                    c = Color.Gray;
                }
                Core.SpriteBatch.Draw(pattern, new Rectangle(dx, dy, 128, 128), c);
            }
        }

        String t = _message.CropStringToWidth(FontManager.MainFont, 500);

        Core.SpriteBatch.DrawString(FontManager.MainFont, _header,
            new Vector2((float)(Core.windowSize.Width / 2 - FontManager.MainFont.MeasureString(_header).X), 168),
            Color.White, 0.0f, new Vector2(0), 2.0f, SpriteEffects.None, 0.0f);

        Core.SpriteBatch.DrawString(FontManager.MainFont, t,
            new Vector2((float)(Core.windowSize.Width / 2 - (FontManager.MainFont.MeasureString(t).X * 1.4f) / 2), 320),
            Color.White, 0.0f, new Vector2(0), 1.4f, SpriteEffects.None, 0.0f);
    }

    public override void Update()
    {
        if (MyMode == Modes.Disconnect)
        {
            if (Controls.Accept(true, true, true) == true || Controls.Dismiss(true, true, true) == true)
            {
                if (_quitToMenu == true)
                {
                    Core.SetScreen(new PressStartScreen());
                }
                else
                {
                    Core.SetScreen(PreScreen);
                }
            }
        }
        else
        {
            if (Core.ServersManager.PlayerManager.ReceivedIniData() == true)
            {
                Connected = true;
                Core.SetScreen(new OverworldScreen());
            }
            if (Controls.Dismiss() == true)
            {
                Connected = false;
                Core.ServersManager.ServerConnection.Disconnect();
                Core.SetScreen(new PressStartScreen());
            }
        }
    }

    public override void ChangeTo()
    {
        if (MyMode == Modes.Connect)
        {
            Thread t = new Thread(() => Core.ServersManager.Connect(JoinServerScreen.SelectedServer));
            t.IsBackground = true;
            t.Start();
        }
    }

    private static ConnectScreen? _tempConnectScreen;
    private static bool _needToSwitch = false;

    public static void Setup(ConnectScreen connectScreen)
    {
        _tempConnectScreen = connectScreen;
        _needToSwitch = true;
    }

    public static void UpdateConnectSet()
    {
        if (_needToSwitch == true)
        {
            _needToSwitch = false;
            Core.SetScreen(_tempConnectScreen!);
        }
    }
}
