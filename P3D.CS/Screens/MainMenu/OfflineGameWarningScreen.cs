using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class OfflineGameWarningScreen : Screen
{
    public OfflineGameWarningScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.OfflineGameWarningScreen;

        CanBePaused = false;
        CanChat = false;
        CanDrawDebug = true;
        CanGoFullscreen = true;
        CanMuteAudio = true;
        CanTakeScreenshot = true;
        MouseVisible = true;
    }

    public override void Draw()
    {
        PreScreen.Draw();

        Canvas.DrawRectangle(Core.windowSize, new Color(0, 0, 0, 150));
        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 310), 90, 620, 420), new Color(16, 16, 16));
        Canvas.DrawRectangle(new Rectangle((int)(Core.windowSize.Width / 2 - 300), 100, 600, 400), new Color(39, 39, 39));

        Core.SpriteBatch.DrawString(FontManager.InGameFont, "Start a new offline game",
            new Vector2((float)(Core.windowSize.Width / 2 - 280), 130), Color.White);

        String t = "If you start a game in \"Offline Mode\" by pressing the \"New Game\" button, you cannot access the online features of Pokémon 3D such as trading and trainer customization. Click on the GameJolt button in the lower right corner in order to start a game in \"Online Mode\".";
        t = t.CropStringToWidth(FontManager.MainFont, 450);

        Core.SpriteBatch.DrawString(FontManager.MainFont, t,
            new Vector2((float)(Core.windowSize.Width / 2 - 310) + 50, 240), Color.White);

        var d = new Dictionary<Buttons, String>
        {
            { Buttons.A, Localization.GetString("game_interaction_accept", "Accept") },
            { Buttons.B, Localization.GetString("game_interaction_dismiss", "Dismiss") }
        };
        DrawGamePadControls(d, new Vector2((float)(Core.windowSize.Width / 2) - 140, 420));
    }

    public override void Update()
    {
        if (Controls.Accept(true, true, true) == true)
        {
            Core.SetScreen(PreScreen);
        }
        if (Controls.Dismiss(true, true, true) == true)
        {
            Core.SetScreen(PreScreen);
            Core.GameOptions.StartedOfflineGame = false;
            Core.GameOptions.SaveOptions();
        }
    }
}
