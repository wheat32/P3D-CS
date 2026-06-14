using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class SaveScreen : Screen
{
    private bool _ready = false;
    private float _delay = 15.0f;
    private Texture2D? _mainTexture;
    private Texture2D? _menuTexture;

    private bool _savingStarted = false;
    private bool _saveSessionFailed = false;

    private int _ySlide = 0;
    private int _ySlideMax = 0;

    private bool _closing = false;
    private bool _opening = true;

    private RenderTarget2D? _target;
    private RenderTarget2D? _target2;
    private SpriteBatch? _saveBookBatch;
    private SpriteBatch? _renderBatch;

    public SaveScreen(Screen currentScreen)
    {
        _ySlideMax = (int)(Core.windowSize.Height / 2) + 270;
        _ySlide = _ySlideMax;

        Identification = Identifications.SaveScreen;
        PreScreen = currentScreen;

        _target = new RenderTarget2D(Core.GraphicsDevice, 700, 440, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _target2 = new RenderTarget2D(Core.GraphicsDevice, Math.Max(1, Core.windowSize.Width), Math.Max(1, Core.windowSize.Height), false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

        _saveBookBatch = new SpriteBatch(Core.GraphicsDevice);
        _renderBatch = new SpriteBatch(Core.GraphicsDevice);

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
        _menuTexture = TextureManager.GetTexture(@"GUI\Menus\SaveBook");

        ChooseBox.Show([Localization.GetString("global_yes"), Localization.GetString("global_no")], 0, []);
        SaveGameHelpers.ResetSaveCounter();
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        if (Core.Player.IsGameJoltSave == true)
        {
            GameJolt.Emblem.Draw(GameJolt.API.username, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem,
                new Vector2((float)(Core.windowSize.Width / 2 - 256), 30), 4, Core.GameJoltSave.DownloadedSprite);
        }

        if (_target == null || _target2 == null || _saveBookBatch == null || _renderBatch == null || _menuTexture == null)
            return;

        int halfWidth = (int)(Core.windowSize.Width / 2);
        int halfHeight = (int)(Core.windowSize.Height / 2);
        int renderX = halfWidth - 350;
        int renderY = halfHeight - 220 + _ySlide;
        int deltaX = 0;
        int deltaY = 0;

        Core.GraphicsDevice.SetRenderTarget(_target);
        Core.GraphicsDevice.Clear(Color.Transparent);

        _saveBookBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

        _saveBookBatch.Draw(_menuTexture, new Rectangle(deltaX, deltaY, 700, 440), Color.White);

        if (_saveSessionFailed == true)
        {
            _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_fail_title", "Saving failed!"), new Vector2(deltaX + 90, deltaY + 50), Color.Red);

            if (Core.GameOptions.Extras.Contains("Backup Save Feature") == true)
            {
                _saveBookBatch.DrawString(FontManager.MiniFont,
                    ("Press [<system.button(back1)>] to close this" + Environment.NewLine +
                    "screen and try to save again" + Environment.NewLine +
                    "in order to prevent data" + Environment.NewLine +
                    "corruption." + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                    "Your save has been backed" + Environment.NewLine +
                    "up in the event of the" + Environment.NewLine +
                    "Gamejolt API being down.").Replace("<system.button(back1)>", KeyBindings.BackKey1.ToString()),
                    new Vector2(deltaX + 90, deltaY + 100), Color.Black);
                _saveBookBatch.DrawString(FontManager.MiniFont,
                    "You may safely quit the" + Environment.NewLine +
                    "game now or try to save" + Environment.NewLine +
                    "again later." + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                    "The backup save can be" + Environment.NewLine +
                    "found in the Backup Save" + Environment.NewLine +
                    "folder",
                    new Vector2(deltaX + 390, deltaY + 100), Color.Black);
            }
            else
            {
                _saveBookBatch.DrawString(FontManager.MiniFont,
                    Localization.GetString("save_screen_fail_message1",
                    "Press [<system.button(back1)>] to close this~screen and try to save again~in order to prevent data~corruption.~~~If the problem persists, the~GameJolt servers could be~down for maintenance right~now.")
                        .Replace("~", Environment.NewLine)
                        .Replace("<system.button(back1)>", KeyBindings.BackKey1.ToString()),
                    new Vector2(deltaX + 90, deltaY + 100), Color.Black);
                _saveBookBatch.DrawString(FontManager.MiniFont,
                    Localization.GetString("save_screen_fail_message2",
                    "Please try again later,~or contact us here:~~Discord Server:~www.discord.me/p3d~~Official News:~pokemon3d.net/blog")
                        .Replace("~", Environment.NewLine),
                    new Vector2(deltaX + 390, deltaY + 100), Color.Black);
            }

            String text = String.Empty;
            Vector2 textSizeUntilButton = new Vector2(0);
            if (ControllerHandler.IsConnected() == true)
            {
                text = Localization.GetString("save_screen_press", "Press") + "<button>" + Localization.GetString("save_screen_to_continue", "to continue.");
                textSizeUntilButton = FontManager.InGameFont.MeasureString(text.GetSplit(0, "<button>"));
                text = text.Replace("<button>", "     ");
            }
            else
            {
                text = Localization.GetString("save_screen_press", "Press") + " [" + KeyBindings.BackKey1.ToString() + "] " + Localization.GetString("save_screen_to_continue", "to continue.");
            }

            Vector2 textSize = FontManager.InGameFont.MeasureString(text);
            GetFontRenderer().DrawString(FontManager.InGameFont, text,
                new Vector2(deltaX + 610 - textSize.X / 2.0f, deltaY + 350 - textSize.Y / 2.0f),
                Color.DarkBlue);

            if (ControllerHandler.IsConnected() == true)
            {
                Core.SpriteBatch.Draw(
                    TextureManager.GetTexture(@"GUI\GamePad\xboxControllerButtonB"),
                    new Rectangle(
                        (int)(deltaX + 610 - textSize.X / 2 + textSizeUntilButton.X + FontManager.InGameFont.MeasureString(" ").X + 2),
                        (int)(deltaY + 350 - textSize.Y / 2), 20, 20),
                    Color.White);
            }
        }
        else
        {
            if (_ready == true)
            {
                _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_success", "Saved the game."), new Vector2(deltaX + 90, deltaY + 50), Color.DarkBlue);
            }
            else
            {
                if (SaveGameHelpers.GameJoltSaveDone() == false && _savingStarted == true)
                {
                    if (SaveGameHelpers.StartedDownloadCheck == true)
                    {
                        _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_progress_validating", "Validating data") + LoadingDots.Dots, new Vector2(deltaX + 90, deltaY + 50), Color.Black);
                    }
                    else
                    {
                        _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_progress_saving", "Saving, please wait") + LoadingDots.Dots, new Vector2(deltaX + 77, deltaY + 50), Color.Black);
                    }
                }
                else
                {
                    _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_question1", "Would you like to"), new Vector2(deltaX + 90, deltaY + 50), Color.Black);
                    _saveBookBatch.DrawString(FontManager.InGameFont, Localization.GetString("save_screen_question2", "save the game?"), new Vector2(deltaX + 90, deltaY + 80), Color.Black);
                }
            }

            for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
            {
                Vector2 pos = new Vector2(deltaX + 390 + (i % 3) * 80, deltaY + 50 + (int)Math.Floor(i / 3.0) * 80);
                Texture2D pokeTexture = Core.Player.Pokemons[i].GetMenuTexture();
                _saveBookBatch.Draw(pokeTexture, new Rectangle((int)pos.X, (int)pos.Y, 64, 64), Color.White);

                if (Core.Player.Pokemons[i].Item != null && Core.Player.Pokemons[i].IsEgg == false)
                {
                    _saveBookBatch.Draw(Core.Player.Pokemons[i].Item!.Texture, new Rectangle((int)pos.X + 36, (int)pos.Y + 36, 32, 32), Color.White);
                }
            }

            _saveBookBatch.DrawString(FontManager.MainFont,
                Localization.GetString("global_name") + ": " + Core.Player.Name + Environment.NewLine + Environment.NewLine +
                Localization.GetString("global_badges") + ": " + Core.Player.Badges.Count.ToString() + Environment.NewLine + Environment.NewLine +
                Localization.GetString("global_money") + ": $" + Core.Player.Money + Environment.NewLine + Environment.NewLine +
                Localization.GetString("global_time") + ": " + TimeHelpers.GetDisplayTime(TimeHelpers.GetCurrentPlayTime(), true),
                new Vector2(deltaX + 400, deltaY + 215), Color.DarkBlue);
        }

        _saveBookBatch.End();

        Core.GraphicsDevice.SetRenderTarget(_target2);
        Core.GraphicsDevice.Clear(Color.Transparent);

        _renderBatch.Begin();
        _renderBatch.Draw(_target,
            new Rectangle(
                (int)(Core.windowSize.Width / 2 - 350 * Core.SpriteBatch.InterfaceScale()),
                (int)(Core.windowSize.Height / 2 - 220 * Core.SpriteBatch.InterfaceScale() + _ySlide),
                (int)(_target.Width * Core.SpriteBatch.InterfaceScale()),
                (int)(_target.Height * Core.SpriteBatch.InterfaceScale())),
            null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0f);
        _renderBatch.End();

        Core.GraphicsDevice.SetRenderTarget(null);

        Core.SpriteBatch.Draw(_target2, new Vector2(0, 0), Color.White);

        Vector2 chooseBoxPositionOffset = new Vector2(0, 0);
        double interfaceScale = Core.SpriteBatch.InterfaceScale();
        if (interfaceScale == 0.5)
            chooseBoxPositionOffset = new Vector2(-96, 16);
        else if (interfaceScale == 2.0)
            chooseBoxPositionOffset = new Vector2(-256, -128);

        double scaleFloor = Math.Floor(interfaceScale - 1.0);
        ChooseBox.Draw(
            new Vector2(
                (int)(renderX + 115 + scaleFloor * chooseBoxPositionOffset.X),
                (int)(renderY + 155 + scaleFloor * chooseBoxPositionOffset.Y)),
            false, 1.5f);
    }

    public override void Update()
    {
        ChooseBox.Update();

        if (_opening == true)
        {
            if (_ySlide < 5)
            {
                _ySlide = 0;
                _opening = false;
            }
            else
            {
                _ySlide = (int)MathHelper.Lerp(_ySlide, 0, 0.1f);
            }
            return;
        }
        else if (_closing == true)
        {
            if (_ySlide < (int)(_ySlideMax * 0.91))
            {
                _ySlide = (int)MathHelper.Lerp(_ySlide, _ySlideMax, 0.1f);
            }
            else
            {
                _closing = false;
                Core.SetScreen(PreScreen!);
            }
            return;
        }

        if (Core.Player.IsGameJoltSave == true)
        {
            if (SaveGameHelpers.GameJoltSaveDone() == true)
            {
                _ready = true;

                if (SaveGameHelpers.EncounteredErrors == true)
                {
                    _saveSessionFailed = true;
                }
                else
                {
                    String backupPath = System.IO.Path.Combine(GameController.GamePath, "Backup Save", Core.GameJoltSave.GameJoltID, "Encrypted", "Encrypted.dat");
                    if (System.IO.File.Exists(backupPath) == true)
                    {
                        System.IO.File.Delete(backupPath);
                    }
                    SoundManager.PlaySound("save");
                }

                SaveGameHelpers.ResetSaveCounter();
            }
        }

        if (_saveSessionFailed == false)
        {
            if (ChooseBox.Showing == false)
            {
                if (_ready == true)
                {
                    if (_delay <= 0.0f)
                    {
                        _closing = true;
                    }
                    else
                    {
                        _delay -= 0.2f;
                        if (_delay <= 0.0f)
                            _delay = 0.0f;
                    }
                }
            }

            if (ChooseBox.readyForResult == true && _savingStarted == false)
            {
                if (ChooseBox.result == 0)
                {
                    if (_ready == false)
                    {
                        Core.Player.SaveGame(false);
                        _savingStarted = true;

                        if (Core.Player.IsGameJoltSave == false)
                        {
                            _ready = true;
                            SoundManager.PlaySound("save");
                        }
                    }
                }
                else
                {
                    _delay = 0.0f;
                    _ready = true;
                }
            }

            if (Controls.Dismiss() == true && _ready == false)
            {
                SoundManager.PlaySound("select");
                ChooseBox.Showing = false;
                _closing = true;
            }
        }
        else
        {
            if (Controls.Dismiss() == true)
            {
                SoundManager.PlaySound("select");
                ChooseBox.Showing = false;
                _closing = true;
            }
        }
    }
}
