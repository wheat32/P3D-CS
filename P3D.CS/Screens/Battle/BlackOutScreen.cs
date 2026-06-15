using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class BlackOutScreen : Screen
{
    private BattleSystem.BattleScreen _battleScreen;
    private int _index = 0;
    private int _textIndex = 0;
    private String[] _text =
    [
        Localization.GetString("black_out_screen_line1"),
        Environment.NewLine,
        "        ",
        Localization.GetString("black_out_screen_line2"),
        Localization.GetString("black_out_screen_line3"),
        Localization.GetString("black_out_screen_line4"),
        Localization.GetString("black_out_screen_line5"),
        Environment.NewLine,
        Localization.GetString("black_out_continue"),
    ];
    private bool _ready = false;
    private float _delay = 0.2f;

    private bool _isGameOver = false;
    private bool _fromBattle = true;

    public BlackOutScreen(BattleSystem.BattleScreen battleScreen)
    {
        _battleScreen = battleScreen;
        Identification = Identifications.BlackOutScreen;

        _isGameOver = bool.Parse(GameModeManager.GetGameRuleValue("GameOverAt0Pokemon", "0"));

        _fromBattle = true;
    }

    public BlackOutScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        _battleScreen = null!;
        Identification = Identifications.BlackOutScreen;

        _isGameOver = bool.Parse(GameModeManager.GetGameRuleValue("GameOverAt0Pokemon", "0"));

        _fromBattle = false;
    }

    public override void Update()
    {
        if (_isGameOver == true)
        {
            if (Controls.Accept(true, true) == true)
            {
                SoundManager.PlaySound("select");
                if (Core.Player.IsGameJoltSave == false)
                {
                    String saveSlotDir = Path.Combine(AppPaths.SaveDir, Core.Player.FilePrefix);
                    if (Directory.Exists(saveSlotDir) == true)
                    {
                        Directory.Delete(saveSlotDir, true);
                    }
                }
                else
                {
                    Core.GameJoltSave.ResetSave();
                    Core.Player.SaveGame(false);
                }
                Core.SetScreen(new PressStartScreen());
                Core.Player.loadedSave = false;
            }
        }
        else
        {
            if (_ready == false)
            {
                if (_delay > 0.0f)
                {
                    _delay -= 0.1f;
                    if (_delay <= 0.0f)
                    {
                        _delay = 0.2f;
                        ProceedText();
                    }
                }
            }
            else
            {
                if (Controls.Accept(true, true) == true)
                {
                    SoundManager.PlaySound("Heal_Party");

                    if (_fromBattle == true)
                    {
                        Core.Player.HealParty();
                        ChangeScreen();

                        ChangeFromSurfRideTexture();

                        Screen.Level.Load(Core.Player.LastRestPlace);
                        Screen.Level.OverworldPokemon.Visible = false;
                        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
                        String[] positionString = Core.Player.LastRestPlacePosition.Split(',');
                        ((OverworldCamera)_battleScreen.SavedOverworld.Camera).YawLocked = false;
                        Screen.Camera.Yaw = MathHelper.Pi;
                        ((OverworldCamera)_battleScreen.SavedOverworld.Camera).CameraFocusType = OverworldCamera.CameraFocusTypes.Player;
                        ((OverworldCamera)_battleScreen.SavedOverworld.Camera).CameraFocusID = -1;
                        Screen.Camera.Position = new Vector3(
                            float.Parse(positionString[0].Replace(".", GameController.DecSeparator)),
                            float.Parse(positionString[1].Replace(".", GameController.DecSeparator)),
                            float.Parse(positionString[2].Replace(".", GameController.DecSeparator)));

                        Screen.Level.Update();
                        Screen.Camera.Update();

                        ((OverworldScreen)_battleScreen.SavedOverworld.OverworldScreen).ActionScript.Scripts.Clear();

                        Core.SetScreen(new TransitionScreen(this, _battleScreen.SavedOverworld.OverworldScreen, Color.Black, false));
                    }
                    else
                    {
                        Core.Player.HealParty();

                        ChangeFromSurfRideTexture();

                        Screen.Level.Load(Core.Player.LastRestPlace);
                        Screen.Level.OverworldPokemon.Visible = false;
                        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);

                        String[] positionString = Core.Player.LastRestPlacePosition.Split(',');
                        if (PreScreen.Identification == Identifications.BattleScreen)
                        {
                            ((OverworldCamera)_battleScreen.SavedOverworld.Camera).YawLocked = false;
                            ((OverworldCamera)_battleScreen.SavedOverworld.Camera).CameraFocusType = OverworldCamera.CameraFocusTypes.Player;
                            ((OverworldCamera)_battleScreen.SavedOverworld.Camera).CameraFocusID = -1;
                        }
                        else
                        {
                            ((OverworldCamera)Screen.Camera).YawLocked = false;
                        }
                        Screen.Camera.Yaw = MathHelper.Pi;
                        Screen.Camera.Position = new Vector3(
                            float.Parse(positionString[0].Replace(".", GameController.DecSeparator)),
                            float.Parse(positionString[1].Replace(".", GameController.DecSeparator)),
                            float.Parse(positionString[2].Replace(".", GameController.DecSeparator)));

                        Screen.Level.Update();
                        Screen.Camera.Update();

                        if (PreScreen.Identification == Identifications.BattleScreen)
                        {
                            ((OverworldScreen)_battleScreen.SavedOverworld.OverworldScreen).ActionScript.Scripts.Clear();
                        }
                        else
                        {
                            ((OverworldScreen)PreScreen).ActionScript.Scripts.Clear();
                        }

                        while (Core.CurrentScreen.Identification != Identifications.OverworldScreen)
                        {
                            Core.SetScreen(Core.CurrentScreen.PreScreen);
                        }
                    }
                }
            }
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), Color.Black);

        if (_isGameOver == true)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, "GAME OVER",
                new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.InGameFont.MeasureString("GAME OVER").X / 2), 100), Color.White);
        }
        else
        {
            Microsoft.Xna.Framework.Graphics.SpriteFont f = FontManager.MiniFont;
            String aText = String.Empty;
            for (int i = 0; i <= _textIndex; i++)
            {
                if (i != _textIndex)
                {
                    aText += _text[i] + Environment.NewLine;
                }
                else
                {
                    if (_textIndex < _text.Length)
                    {
                        aText += _text[i].Remove(_index);
                    }
                }
            }

            Vector2 p = new Vector2(
                (float)(Core.windowSize.Width / 2 - f.MeasureString(aText).X / 2),
                (float)(Core.windowSize.Height / 2 - f.MeasureString(aText).Y / 2));

            Core.SpriteBatch.DrawString(FontManager.MainFont, aText, p, Color.White);
        }

        if (_isGameOver == true || _ready == true)
        {
            Dictionary<Buttons, String> d = [];
            d.Add(Buttons.A, Localization.GetString("game_interaction_accept", "Accept"));
            DrawGamePadControls(d);
        }
    }

    public override void ChangeTo()
    {
        MusicManager.PlayNoMusic();
        PlayerStatistics.Track("Blackouts", 1);
    }

    private void ChangeScreen()
    {
        Screen.Level = _battleScreen.SavedOverworld.Level;
        Screen.Camera = _battleScreen.SavedOverworld.Camera;
        Screen.Effect = _battleScreen.SavedOverworld.Effect;
        Screen.SkyDome = _battleScreen.SavedOverworld.SkyDome;
        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    private void ProceedText()
    {
        if (_index >= _text[_textIndex].Length - 1)
        {
            _index = 0;
            _textIndex += 1;
            if (_textIndex > _text.Length - 1)
            {
                _ready = true;
            }
        }
        else
        {
            _index += 1;
        }
    }

    private void ChangeFromSurfRideTexture()
    {
        if (Screen.Level.Riding == true && Core.Player.TempRideSkin != String.Empty)
        {
            Screen.Level.Riding = false;
            Screen.Level.OwnPlayer.SetTexture(Core.Player.TempRideSkin, true);
            Core.Player.Skin = Core.Player.TempRideSkin;
        }
        if (Screen.Level.Surfing == true && Core.Player.TempSurfSkin != String.Empty)
        {
            Screen.Level.Surfing = false;
            Screen.Level.OwnPlayer.SetTexture(Core.Player.TempSurfSkin, true);
            Core.Player.Skin = Core.Player.TempSurfSkin;
        }
        Screen.Level.OverworldPokemon.warped = true;
        Screen.Level.OverworldPokemon.Visible = false;
    }
}
