using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NewNewGameScreen : OverworldScreen
{
        public NewNewGameScreen(Screen currentScreen)
        {
            Identification = Identifications.NewGameScreen;
            CanChat = false;
            MouseVisible = false;
            IsOverlay = true;
            PreScreen = currentScreen;
            CanBePaused = false;
            UpdateFadeOut = true;

            foreach (String s in Core.GameOptions.ContentPackNames)
            {
                ContentPackManager.Load(GameController.GamePath + @"\ContentPacks\" + s + @"\exceptions.dat");
            }

            Core.Player.Unload();

            BattleSystem.GameModeElementLoader.Load();
            BattleSystem.GameModeAttackLoader.Load();
            GameModeItemLoader.Load();

            SmashRock.Load();
            Badge.Load();
            Pokedex.Load();
            BattleSystem.BattleScreen.ResetVars();
            LevelLoader.ClearTempStructures();
            PokemonForms.Initialize();

            Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
            Effect.FogEnabled = true;

            ScriptStorage.Clear();
            ActionScript.Scripts.Clear();

            GameMode? gm = GameModeManager.ActiveGameMode;
            if (gm != null)
            {
                Camera = new NewGameCamera(gm.StartPosition, gm.StartRotation, gm.StartPitch);
            }
            else
            {
                Camera = new NewGameCamera(Vector3.Zero, 0f, 0f);
            }

            SkyDome = new SkyDome();
            Level = new Level();
            if (gm != null)
            {
                Level.Load(gm.StartMap);
            }

            Camera.Update();

            MusicManager.PlayNoMusic();

            Level.World.Initialize(Level.EnvironmentType, Level.WeatherType);

            ActionScript.StartScript(@"newgame\intro", 0);
        }

        public override void Update()
        {
            BasicEffectWithAlphaTest? lightEffect = Effect;
            if (lightEffect != null)
            {
                Lighting.UpdateLighting(ref lightEffect);
                Effect = lightEffect;
            }

            ChooseBox.Update();
            if (ChooseBox.Showing == false)
            {
                TextBox.Update();
            }
            if (PokemonImageView.Showing == true)
            {
                PokemonImageView.Update();
            }
            if (ImageView.Showing == true)
            {
                ImageView.Update();
            }

            if (TextBox.Showing == false && ChooseBox.Showing == false && PokemonImageView.Showing == false && ImageView.Showing == false)
            {
                if (ActionScript.IsReady == true)
                {
                    Camera.Update();
                    Level.Update();
                }
                if (ActionScript.Scripts.Count == 0)
                {
                    if (Core.CurrentScreen.Identification == Identifications.NewGameScreen)
                    {
                        if (FadeValue > 0)
                        {
                            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new OverworldScreen(), Color.Black, false, RemoveFade));
                        }
                        else
                        {
                            Core.SetScreen(new OverworldScreen());
                        }
                    }
                }
                else
                {
                    ActionScript.Update();
                }
            }
            else
            {
                Camera.Update();
                Level.Update();
            }

            SkyDome.Update();

            Level.World.Initialize(Level.EnvironmentType, Level.WeatherType);
        }

        public void RemoveFade()
        {
            FadeValue = 0;
        }

        public override void Draw()
        {
            SkyDome.Draw(Camera.FOV);
            Level.Draw();

            if (FadeValue > 0)
            {
                Canvas.DrawRectangle(Core.windowSize, new Color(FadeColor.R, FadeColor.G, FadeColor.B, FadeValue));
            }

            PokemonImageView.Draw();
            ImageView.Draw();
            TextBox.Draw();

            if (IsCurrentScreen() == true)
            {
                ChooseBox.Draw();
            }
        }

        public static void EndNewGame(String map, float x, float y, float z, int rot)
        {
            String folderPath = Core.Player.Name
                .Replace("\\", "_").Replace("/", "_").Replace(":", "_")
                .Replace("*", "_").Replace("?", "_").Replace("\"", "_")
                .Replace("<", "_").Replace(">", "_").Replace("|", "_")
                .Replace(",", "_").Replace(".", "_");
            int folderPrefix = 0;

            if (folderPath.ToLower() == "autosave")
            {
                folderPath = "autosave0";
            }

            while (Directory.Exists(Path.Combine(AppPaths.SaveDir, folderPath)) == true)
            {
                if (folderPath != Core.Player.Name)
                {
                    folderPath = folderPath.Remove(folderPath.Length - folderPrefix.ToString().Length, folderPrefix.ToString().Length);
                }
                folderPath += folderPrefix;
                folderPrefix += 1;
            }

            Directory.CreateDirectory(AppPaths.SaveDir);

            Core.Player.FilePrefix = folderPath;
            Core.Player.GameStart = DateTime.Now;
            Core.Player.GameMode = GameModeManager.ActiveGameMode?.DirectoryName ?? String.Empty;
            Core.Player.StartFOV = 60;
            Core.Player.StartFreeCameraMode = true;
            Core.Player.StartPosition = new Vector3(x, y, z);
            Core.Player.StartMap = map;
            Core.Player.StartRotationSpeed = 12;
            Core.Player.StartSurfing = false;
            Core.Player.StartThirdPerson = false;
            Core.Player.StartRiding = false;
            Core.Player.StartRotation = (float)(MathHelper.Pi * (rot / 2.0));

            Core.Player.PokedexData = Pokedex.NewPokedex();
            Core.Player.BerryData = CreateBerryData();
            Core.Player.AddVisitedMap(map);
            Core.Player.SaveCreated = GameController.GAMEDEVELOPMENT_STAGE + " " + GameController.GAME_VERSION;

            String ot = Core.Random.Next(0, 999999).ToString();
            while (ot.Length < 6)
            {
                ot = "0" + ot;
            }
            Core.Player.OT = ot;

            String slotDir = Path.Combine(AppPaths.SaveDir, folderPath);
            Directory.CreateDirectory(slotDir);

            File.WriteAllText(Path.Combine(slotDir, "Player.dat"), Core.Player.GetPlayerData(false));
            File.WriteAllText(Path.Combine(slotDir, "Pokedex.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Items.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Register.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Berries.dat"), Core.Player.BerryData);
            File.WriteAllText(Path.Combine(slotDir, "Apricorns.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Daycare.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Party.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "ItemData.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Options.dat"), Core.Player.GetOptionsData());
            File.WriteAllText(Path.Combine(slotDir, "Box.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "NPC.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "HallOfFame.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "SecretBase.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "RoamingPokemon.dat"), String.Empty);
            File.WriteAllText(Path.Combine(slotDir, "Statistics.dat"), String.Empty);
        }

        private static String CreateBerryData()
        {
            String s = String.Empty;
            if (File.Exists(GameModeManager.GetContentFilePath("Data\\BerryData.dat")))
            {
                String[] berries = File.ReadAllLines(GameModeManager.GetContentFilePath("Data\\BerryData.dat"));
                for (int i = 0; i <= berries.Length - 1; i++)
                {
                    if (berries[i] != String.Empty)
                    {
                        s += berries[i];
                        if (i < berries.Length - 1)
                        {
                            s += Environment.NewLine;
                        }
                    }
                }
            }
            else
            {
                if (GameModeManager.ActiveGameMode?.IsDefaultGamemode == true)
                {
                    s = "{route29.dat|13,0,5|6|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route29.dat|14,0,5|6|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route29.dat|15,0,5|6|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{azalea.dat|9,0,3|0|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{azalea.dat|9,0,4|1|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{azalea.dat|9,0,5|0|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route30.dat|7,0,41|10|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route30.dat|14,0,5|2|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route30.dat|15,0,5|6|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route30.dat|16,0,5|2|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{routes\route35.dat|0,0,4|7|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{routes\route35.dat|1,0,4|8|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route36.dat|37,0,7|0|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route36.dat|38,0,7|4|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route36.dat|39,0,7|3|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route39.dat|8,0,2|9|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route39.dat|8,0,3|6|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route38.dat|13,0,12|16|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route38.dat|14,0,12|23|1|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        "{route38.dat|15,0,12|16|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{routes\route43.dat|13,0,45|23|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{routes\route43.dat|13,0,46|24|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{routes\route43.dat|13,0,47|25|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{safarizone\main.dat|3,0,11|5|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{safarizone\main.dat|4,0,11|0|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
                        @"{safarizone\main.dat|5,0,11|6|3|0|2012,9,21,4,0,0|1}";
                }
            }
            return s;
        }

        public class CharacterSelectionScreen : Screen
        {
            private List<String> _skins = [];
            private List<String> _names = [];
            private List<Color> _colors = [];
            private List<String> _genders = [];
            private List<Texture2D> _sprites = [];

            private float _offset = 0f;
            private int _index = 0;
            private float _fadeIn = 0f;

            public static String SelectedSkin { get; set; } = String.Empty;
            public static String SelectedName { get; set; } = String.Empty;
            public static String SelectedGender { get; set; } = String.Empty;

            public CharacterSelectionScreen(Screen currentScreen)
            {
                Identification = Identifications.CharacterSelectionScreen;
                PreScreen = currentScreen;
                CanBePaused = true;
                CanChat = false;
                CanDrawDebug = true;
                CanGoFullscreen = true;
                CanTakeScreenshot = true;
                MouseVisible = true;
                SelectedSkin = String.Empty;

                GameMode? gm = GameModeManager.ActiveGameMode;
                if (gm != null)
                {
                    foreach (String skin in gm.SkinFiles)
                    {
                        _sprites.Add(TextureManager.GetTexture(@"Textures\NPC\" + skin));
                    }
                    _skins = gm.SkinFiles;
                    _names = gm.SkinNames;
                    _genders = gm.SkinGenders;
                    _colors = gm.SkinColors;
                }
            }

            public override void Update()
            {
                if (_fadeIn < 1.0f)
                {
                    _fadeIn = MathHelper.Lerp(1.0f, _fadeIn, 0.95f);
                    if (_fadeIn + 0.01f >= 1.0f)
                    {
                        _fadeIn = 1.0f;
                    }
                }
                if (_fadeIn > 0.9f)
                {
                    if (Controls.Left(true) == true && _index > 0)
                    {
                        _index -= 1;
                        _offset -= 280;
                    }
                    if (Controls.Right(true) == true && _index < _skins.Count - 1)
                    {
                        _index += 1;
                        _offset += 280;
                    }

                    if (_offset > 0f)
                    {
                        _offset = MathHelper.Lerp(0f, _offset, 0.9f);
                        if (_offset - 0.01f <= 0f)
                        {
                            _offset = 0f;
                        }
                    }
                    else if (_offset < 0f)
                    {
                        _offset = MathHelper.Lerp(0f, _offset, 0.9f);
                        if (_offset + 0.01f >= 0f)
                        {
                            _offset = 0f;
                        }
                    }

                    if (Controls.Accept(false, true, true) == true)
                    {
                        SoundManager.PlaySound("select");
                        SelectedSkin = _skins[_index];
                        SelectedName = _names[_index];
                        SelectedGender = _genders[_index];
                        Core.SetScreen(PreScreen);
                    }
                    if (Controls.Accept(true, false, false) == true)
                    {
                        for (int i = 0; i <= _skins.Count - 1; i++)
                        {
                            if (new Rectangle(
                                (int)(Core.windowSize.Width / 2 - 128 + i * 280 - _index * 280 + _offset),
                                (int)(Core.windowSize.Height / 2 - 128),
                                256, 256).Contains(MouseHandler.MousePosition))
                            {
                                if (i == _index)
                                {
                                    SoundManager.PlaySound("select");
                                    SelectedSkin = _skins[_index];
                                    SelectedName = _names[_index];
                                    SelectedGender = _genders[_index];
                                    Core.SetScreen(PreScreen);
                                }
                                else
                                {
                                    _offset += (i - _index) * 280;
                                    _index = i;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            public override void Draw()
            {
                PreScreen.Draw();
                Color backcolor = new Color(_colors[_index], (int)(128 * _fadeIn));
                Canvas.DrawRectangle(Core.windowSize, backcolor);

                String selectSkin = Localization.GetString("new_game_select_skin");
                Core.SpriteBatch.DrawString(FontManager.MainFont, selectSkin,
                    new Vector2(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(selectSkin).X + 2, 100 + 2),
                    new Color(0, 0, 0, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
                Core.SpriteBatch.DrawString(FontManager.MainFont, selectSkin,
                    new Vector2(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(selectSkin).X, 100),
                    new Color(255, 255, 255, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

                for (int i = 0; i <= _sprites.Count - 1; i++)
                {
                    Texture2D sprite = _sprites[i];
                    Size frameSize;
                    if (sprite.Width == sprite.Height / 2)
                    {
                        frameSize = new Size((int)(sprite.Width / 2), (int)(sprite.Height / 4));
                    }
                    else if (sprite.Width == sprite.Height)
                    {
                        frameSize = new Size((int)(sprite.Width / 4), (int)(sprite.Height / 4));
                    }
                    else
                    {
                        frameSize = new Size((int)(sprite.Width / 3), (int)(sprite.Height / 4));
                    }

                    int outSize = 256 - Math.Abs(_index - i) * 30;
                    Core.SpriteBatch.Draw(sprite,
                        new Rectangle(
                            (int)(Core.windowSize.Width / 2 - (int)(outSize / 2) + i * 280 - _index * 280 + _offset),
                            (int)(Core.windowSize.Height / 2 - 128),
                            outSize, outSize),
                        new Rectangle(0, frameSize.Height * 2, frameSize.Width, frameSize.Height),
                        new Color(255, 255, 255, (int)(255 * _fadeIn)));
                }

                String name = _names[_index];
                Core.SpriteBatch.DrawString(FontManager.MainFont, name,
                    new Vector2((int)(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(name).X) + 2, (int)(Core.windowSize.Height / 2.0f + 200) + 2),
                    new Color(0, 0, 0, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
                Core.SpriteBatch.DrawString(FontManager.MainFont, name,
                    new Vector2((int)(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(name).X), (int)(Core.windowSize.Height / 2.0f + 200)),
                    new Color(255, 255, 255, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

                String gender = Localization.GetString("global_" + _genders[_index].ToLower(), _genders[_index]);
                Core.SpriteBatch.DrawString(FontManager.MainFont, gender,
                    new Vector2((int)(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(gender).X / 2.0f) + 1, (int)(Core.windowSize.Height / 2.0f + 300) + 1),
                    new Color(0, 0, 0, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                Core.SpriteBatch.DrawString(FontManager.MainFont, gender,
                    new Vector2((int)(Core.windowSize.Width / 2.0f - FontManager.MainFont.MeasureString(gender).X / 2.0f), (int)(Core.windowSize.Height / 2.0f + 300)),
                    new Color(255, 255, 255, (int)(255 * _fadeIn)), 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            }
        }
    }

