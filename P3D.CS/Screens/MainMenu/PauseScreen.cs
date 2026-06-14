using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class PauseScreen : Screen
{
    private int _mainIndex = 0;
    private int _quitIndex = 0;
    private Texture2D _mainTexture;
    private bool _leftEscapeKey = false;
    private int _menuIndex = 0;

    private bool _canCreateAutosave = true;

    public PauseScreen(Screen currentScreen)
    {
        Identification = Identifications.PauseScreen;
        CanBePaused = false;
        MouseVisible = true;
        CanChat = false;

        if (currentScreen != null)
        {
            PreScreen = currentScreen;
        }

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        if (Core.Player.IsGameJoltSave == true)
        {
            _canCreateAutosave = false;
        }
        else
        {
            if (PreScreen != null)
            {
                if (Camera == null)
                {
                    Camera = new OverworldCamera();
                }
                else if (Camera.Name != null)
                {
                    if (Camera.Name != "Overworld")
                    {
                        _canCreateAutosave = false;
                    }
                    else
                    {
                        Screen s = PreScreen;
                        while (s.Identification != Identifications.OverworldScreen && s.PreScreen != null)
                        {
                            s = s.PreScreen;
                        }
                        if (s.Identification == Identifications.OverworldScreen)
                        {
                            if (((OverworldScreen)s).ActionScript.IsReady == false)
                            {
                                _canCreateAutosave = false;
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Draw()
    {
        if (PreScreen != null)
        {
            PreScreen.Draw();
        }

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));

        String titletext = Localization.GetString("pause_menu_title");
        int pX = (int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(titletext).X / 2);
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, titletext,
            new Vector2((int)(pX - 7), (int)(160 - FontManager.InGameFont.MeasureString(titletext).Y / 2 + 3)), Color.Black);
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, titletext,
            new Vector2((int)(pX - 10), (int)(160 - FontManager.InGameFont.MeasureString(titletext).Y / 2)), Color.White);

        if (_menuIndex == 0)
        {
            DrawMenu();
        }
        else
        {
            DrawQuit();
        }

        if (_canCreateAutosave == false)
        {
            String autosaveFailText = Localization.GetString("pause_menu_autosave_fail");
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, autosaveFailText,
                new Vector2(9, (int)(Core.ScreenSize.Height - FontManager.InGameFont.MeasureString(autosaveFailText).Y)), Color.Black);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, autosaveFailText,
                new Vector2(7, (int)(Core.ScreenSize.Height - FontManager.InGameFont.MeasureString(autosaveFailText).Y - 2)), Color.White);
        }

        var d = new Dictionary<Buttons, String>
        {
            { Buttons.A, Localization.GetString("game_interaction_accept", "Accept") },
            { Buttons.B, Localization.GetString("game_interaction_go_back", "Go Back") }
        };
        DrawGamePadControls(d);
    }

    public override void Update()
    {
        if (PreScreen.Identification == Identifications.OverworldScreen)
        {
            Level.Update();
        }
        else if (PreScreen.Identification == Identifications.BattleCatchScreen)
        {
            PreScreen.Update();
            ((BattleCatchScreen)PreScreen).UpdateAnimations();
        }
        else if (PreScreen.Identification == Identifications.BattleScreen)
        {
            PreScreen.Update();
        }
        else if (PreScreen.Identification == Identifications.VoltorbFlipScreen)
        {
            if (VoltorbFlip.VoltorbFlipScreen.GameOrigin.X != (int)(Core.windowSize.Width / 2 - VoltorbFlip.VoltorbFlipScreen.GameSize.Width / 2 - 32) ||
                VoltorbFlip.VoltorbFlipScreen.GameOrigin.Y != (int)(Core.windowSize.Height / 2 - ((VoltorbFlip.VoltorbFlipScreen)PreScreen)._screenTransitionY))
            {
                ((VoltorbFlip.VoltorbFlipScreen)PreScreen).Minimized();
            }
        }
        else if (PreScreen.Identification == Identifications.PartyScreen)
        {
            if (((PartyScreen)PreScreen)._cursorDest != ((PartyScreen)PreScreen).GetBoxPosition(((PartyScreen)PreScreen)._index))
            {
                ((PartyScreen)PreScreen).Minimized();
            }
        }
        else if (PreScreen.Identification == Identifications.MenuScreen)
        {
            NewMenuScreen menuscr = (NewMenuScreen)PreScreen;
            Vector2 buttonPos = menuscr.GetButtonPosition(menuscr._menuIndex);
            Vector2 cursorPos = new Vector2((int)(buttonPos.X + 180), (int)(buttonPos.Y - 42));
            if (menuscr._cursorDestPosition != cursorPos)
            {
                ((NewMenuScreen)PreScreen).Minimized();
            }
        }

        TextBox.reDelay = 0.0f;

        if (GameController.IsActiveWindow() == true)
        {
            if (_menuIndex == 0)
            {
                UpdateMain();
            }
            else
            {
                UpdateQuit();
            }
        }
    }

    private void DrawMenu()
    {
        Color fontShadow = new Color(0, 0, 0, 0);
        for (int i = 0; i <= 1; i++)
        {
            String text = i == 0
                ? Localization.GetString("pause_menu_back_to_game")
                : Localization.GetString("pause_menu_quit_to_menu");

            Color fontColor;
            if (i == _mainIndex)
            {
                fontColor = Color.White;
                fontShadow.A = 255;
                Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 48, 48, 48)), 2,
                    new Rectangle((int)(Core.ScreenSize.Width / 2) - 180, 220 + i * 128, 320, 64), true);
            }
            else
            {
                fontColor = Color.Black;
                fontShadow.A = 0;
                Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48)), 2,
                    new Rectangle((int)(Core.ScreenSize.Width / 2) - 180, 220 + i * 128, 320, 64), true);
            }

            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2 - FontManager.InGameFont.MeasureString(text).X / 2 - 10 + 2), (int)(256 + i * 128 + 2)), fontShadow);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2 - FontManager.InGameFont.MeasureString(text).X / 2 - 10), (int)(256 + i * 128)), fontColor);
        }
    }

    private void UpdateMain()
    {
        if (Controls.Up(true, true, true) == true)
        {
            _mainIndex -= 1;
        }
        if (Controls.Down(true, true, true) == true)
        {
            _mainIndex += 1;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 180, (int)(220 + i * 128), 320 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _mainIndex = i;

                    if (Controls.Accept(true, false) == true)
                    {
                        switch (_mainIndex)
                        {
                            case 0:
                                SoundManager.PlaySound("select");
                                ClickContinue();
                                break;
                            case 1:
                                SoundManager.PlaySound("select");
                                ClickQuit();
                                break;
                        }
                    }
                }
            }
        }

        _mainIndex = (int)MathHelper.Clamp(_mainIndex, 0, 1);

        if (Controls.Accept(false, true) == true)
        {
            switch (_mainIndex)
            {
                case 0:
                    SoundManager.PlaySound("select");
                    ClickContinue();
                    break;
                case 1:
                    SoundManager.PlaySound("select");
                    ClickQuit();
                    break;
            }
        }

        if (Controls.Dismiss() == true || (_leftEscapeKey == true && (KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) == true || ControllerHandler.ButtonPressed(Buttons.Start) == true)))
        {
            SoundManager.PlaySound("select");
            ClickContinue();
        }

        if (KeyBoardHandler.KeyDown(KeyBindings.EscapeKey) == false && ControllerHandler.ButtonPressed(Buttons.Start) == false)
        {
            _leftEscapeKey = true;
        }
    }

    private void ClickContinue()
    {
        Core.SetScreen(PreScreen);
    }

    private void ClickQuit()
    {
        if (_canCreateAutosave == true)
        {
            QuitGame();
        }
        else
        {
            _menuIndex = 1;
            _quitIndex = 0;
        }
    }

    private void DrawQuit()
    {
        String confirmText = Localization.GetString("pause_menu_confirmation");
        int pX = (int)(Core.ScreenSize.Width / 2) - (int)(FontManager.InGameFont.MeasureString(confirmText).X / 2);
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, confirmText,
            new Vector2((int)(pX - 10 + 2), (int)(192 - FontManager.InGameFont.MeasureString(confirmText).Y / 2) + 2), Color.Black);
        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, confirmText,
            new Vector2((int)(pX - 10), (int)(192 - FontManager.InGameFont.MeasureString(confirmText).Y / 2)), Color.White);

        Color fontShadow = new Color(0, 0, 0, 0);
        for (int i = 0; i <= 1; i++)
        {
            String text;
            int x;
            if (i == 0)
            {
                text = Localization.GetString("global_no");
                x = -200;
            }
            else
            {
                text = Localization.GetString("global_yes");
                x = 200;
            }

            Color fontColor;
            if (i == _quitIndex)
            {
                fontColor = Color.White;
                fontShadow.A = 255;
                Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 48, 48, 48)), 2,
                    new Rectangle((int)(Core.ScreenSize.Width / 2 - 180 + x), 320, 320, 64), true);
            }
            else
            {
                fontColor = Color.Black;
                fontShadow.A = 0;
                Canvas.DrawImageBorder(TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48)), 2,
                    new Rectangle((int)(Core.ScreenSize.Width / 2 - 180 + x), 320, 320, 64), true);
            }

            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2 - (FontManager.InGameFont.MeasureString(text).X / 2) - 10 + x + 2), 356 + 2), fontShadow);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2 - (FontManager.InGameFont.MeasureString(text).X / 2) - 10 + x), 356), fontColor);
        }
    }

    private void UpdateQuit()
    {
        if (Controls.Left(true, true, true) == true)
        {
            _quitIndex -= 1;
        }
        if (Controls.Right(true, true, true) == true)
        {
            _quitIndex += 1;
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 1; i++)
            {
                int x = i == 1 ? 200 : -200;
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2 - 180 + x), 320, 320 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _quitIndex = i;

                    if (Controls.Accept(true, false) == true)
                    {
                        switch (_quitIndex)
                        {
                            case 0:
                                SoundManager.PlaySound("select");
                                ClickBack();
                                break;
                            case 1:
                                SoundManager.PlaySound("select");
                                ClickConfirmationQuit();
                                break;
                        }
                    }
                }
            }
        }

        _quitIndex = (int)MathHelper.Clamp(_quitIndex, 0, 1);

        if (Controls.Accept(false, true) == true)
        {
            switch (_quitIndex)
            {
                case 0:
                    SoundManager.PlaySound("select");
                    ClickBack();
                    break;
                case 1:
                    SoundManager.PlaySound("select");
                    ClickConfirmationQuit();
                    break;
            }
        }

        if (Controls.Dismiss(false, true) == true)
        {
            SoundManager.PlaySound("select");
            ClickBack();
        }
    }

    private void ClickBack()
    {
        _menuIndex = 0;
    }

    private void ClickConfirmationQuit()
    {
        QuitGame();
    }

    private void QuitGame()
    {
        VoltorbFlip.VoltorbFlipScreen.CurrentLevel = 1;
        VoltorbFlip.VoltorbFlipScreen.PreviousLevel = 1;
        VoltorbFlip.VoltorbFlipScreen.ConsecutiveWins = 0;
        VoltorbFlip.VoltorbFlipScreen.TotalFlips = 0;
        VoltorbFlip.VoltorbFlipScreen.CurrentCoins = 0;
        VoltorbFlip.VoltorbFlipScreen.TotalCoins = -1;

        if (JoinServerScreen.Online == true)
        {
            Core.ServersManager.ServerConnection.Disconnect();
        }
        TextBox.TextColor = TextBox.DefaultColor;
        MusicManager.ForceMusic = String.Empty;
        World.setDaytime = -1;
        World.setSeason = -1;
        Chat.ClearChat();
        ScriptStorage.Clear();
        GameModeManager.SetGameModePointer("Kolben");
        Localization.LocalizationTokens.Clear();
        Localization.LoadTokenFile(GameMode.DefaultLocalizationsPath, false);
        Core.OffsetMaps.Clear();
        TextureManager.TextureList.Clear();
        TextureManager.TextureRectList.Clear();
        ContentPackManager.ScriptTextureReplacements.Clear();
        Whirlpool.LoadedWaterTemp = false;
        Core.Player.RunToggled = false;
        Core.Player.DoWalkAnimation = true;
        Core.SetScreen(new PressStartScreen());
        Core.Player.loadedSave = false;
    }

    public override void ChangeTo()
    {
        MusicManager.PauseVolume = 0.25f;
        MusicManager.UpdateVolume();
    }

    public override void ChangeFrom()
    {
        MusicManager.PauseVolume = 1.0f;
        MusicManager.UpdateVolume();
    }
}
