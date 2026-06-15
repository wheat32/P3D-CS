using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NewGameScreen : Screen
{
    private String[] _skinFiles = [];
    private String[] _skinNames = [];
    private String[] _skinGenders = [];
    private List<Color> _skinColors = [];

    public int Index = 0;
    private int _pokeIndex = 0;

    private int _profAlpha = 0;
    private int _otherAlpha = 0;

    private String _name = String.Empty;
    private int _skinIndex = 0;
    private Texture2D? _skinTexture;
    private bool _enterCorrectName = false;
    private String _nameMessage = "This name is too short.";

    private Texture2D? _mainTexture;
    private Texture2D? _pokeTexture;

    private Vector2 _ballPosition;
    private Vector2 _ballIndex = Vector2.Zero;
    private float _ballAnimationDelay = 0.2f;
    private float _ballVelocity = -7.0f;
    private Vector2 _pokePosition;
    private int _pokeID = 0;

    private String _currentText = String.Empty;

    private Color _currentBackColor = new Color(59, 123, 165);
    private Color _normalColor = new Color(59, 123, 165);

    private int[] _pokemonRange = [1, 252];
    private String _introMusic = "welcome";
    private String _startMap = "yourroom.dat";
    private Vector3 _startPosition = new Vector3(1, 0.1f, 3);
    private String _startLocation = "Your Room";
    private float _startYaw = MathHelper.PiOver2;

    private List<String> _dialogues = [];

    public NewGameScreen()
    {
        foreach (String s in Core.GameOptions.ContentPackNames)
            ContentPackManager.Load(GameController.GamePath + @"\ContentPacks\" + s + @"\exceptions.dat");

        BattleSystem.GameModeElementLoader.Load();
        BattleSystem.GameModeAttackLoader.Load();

        SmashRock.Load();
        Badge.Load();
        Pokedex.Load();
        BattleSystem.BattleScreen.ResetVars();
        LevelLoader.ClearTempStructures();
        PokemonForms.Initialize();

        Identification = Identifications.NewGameScreen;
        CanChat = false;
        _mainTexture = TextureManager.GetTexture(@"GUI\Intro");

        LoadIntroValues();

        Pokemon p = Pokemon.GetPokemonByID(Core.Random.Next(_pokemonRange[0], _pokemonRange[1]));
        p.Generate(1, true);
        _pokeID = p.Number;
        _pokeTexture = p.GetTexture(true);

        _currentText = String.Empty;

        TextBox.Showing = false;
        Index = 0;
        TextBox.reDelay = 0;
        LoadSkinTexture();

        MusicManager.Play("nomusic");
    }

    private void LoadSkinTexture()
    {
        if (_skinFiles.Length == 0) return;
        Texture2D skinTex2D = TextureManager.GetTexture(@"Textures\NPC\" + _skinFiles[_skinIndex]);
        Size skinFrameSize;
        if (skinTex2D.Width == skinTex2D.Height / 2)
            skinFrameSize = new Size(skinTex2D.Width / 2, skinTex2D.Height / 4);
        else if (skinTex2D.Width == skinTex2D.Height)
            skinFrameSize = new Size(skinTex2D.Width / 4, skinTex2D.Height / 4);
        else
            skinFrameSize = new Size(skinTex2D.Width / 3, skinTex2D.Height / 4);
        Rectangle skinRec = new Rectangle(0, skinFrameSize.Height * 2, skinFrameSize.Width, skinFrameSize.Height);
        _skinTexture = TextureManager.GetTexture(skinTex2D, skinRec);
    }

    private void LoadIntroValues()
    {
        GameMode gameMode = GameModeManager.ActiveGameMode!;

        _skinNames = gameMode.SkinNames.ToArray();
        _skinFiles = gameMode.SkinFiles.ToArray();
        _skinGenders = gameMode.SkinGenders.ToArray();
        _skinColors = gameMode.SkinColors;

        _pokemonRange = gameMode.PokemonRange;
        _introMusic = gameMode.IntroMusic;
        _startMap = gameMode.StartMap;
        _startPosition = gameMode.StartPosition;
        _startLocation = gameMode.StartLocationName;
        _startYaw = gameMode.StartRotation;
        _normalColor = gameMode.StartColor;
        _currentBackColor = gameMode.StartColor;

        if (gameMode.StartDialogue != String.Empty)
        {
            if (gameMode.StartDialogue.CountSplits("|") >= 3)
            {
                String[] splits = gameMode.StartDialogue.Split('|');
                _dialogues.AddRange(splits);
            }
        }
        else
        {
            if (_dialogues.Count < 3)
            {
                _dialogues.Clear();
                _dialogues.AddRange([
                    Localization.GetString("new_game_intro_1"),
                    Localization.GetString("new_game_intro_2"),
                    Localization.GetString("new_game_intro_3")
                ]);
            }
        }
    }

    public override void Update()
    {
        if (Index == 5)
            Core.GameInstance.IsMouseVisible = true;
        else
            Core.GameInstance.IsMouseVisible = false;

        if (_profAlpha < 255 && Index == 0)
        {
            _profAlpha += 2;
            if (_profAlpha >= 255)
                MusicManager.Play(_introMusic, true, 0.0f);
        }
        else if (_profAlpha >= 255 || Index > 0)
        {
            TextBox.Update();

            if (TextBox.Showing == false)
            {
                switch (Index)
                {
                    case 1: UpdatePokemon(); break;
                    case 3: UpdateTransition(false); break;
                    case 4:
                        if (NewNewGameScreen.CharacterSelectionScreen.SelectedSkin != String.Empty)
                        {
                            _skinTexture = TextureManager.GetTexture(NewNewGameScreen.CharacterSelectionScreen.SelectedSkin);
                            Index += 1;
                        }
                        else
                        {
                            Core.SetScreen(new NewNewGameScreen.CharacterSelectionScreen(Core.CurrentScreen!));
                        }
                        break;
                    case 5:
                        UpdateTextbox();
                        Index += 1;
                        break;
                    case 7: UpdateTransition(true); break;
                    case 9:
                        if (_profAlpha > 0) _profAlpha -= 2;
                        if (_profAlpha <= 0) CreateGame();
                        break;
                    default: ShowText(); break;
                }
            }
        }

        Color aimColor = _normalColor;
        if (Index == 4 && _skinColors.Count > _skinIndex)
            aimColor = _skinColors[_skinIndex];

        byte diffR = (Math.Abs(_currentBackColor.R - aimColor.R) < 5) ? (byte)1 : (byte)5;
        byte diffG = (Math.Abs(_currentBackColor.G - aimColor.G) < 5) ? (byte)1 : (byte)5;
        byte diffB = (Math.Abs(_currentBackColor.B - aimColor.B) < 5) ? (byte)1 : (byte)5;

        if (_currentBackColor.R < aimColor.R) _currentBackColor.R += diffR;
        else if (_currentBackColor.R > aimColor.R) _currentBackColor.R -= diffR;
        if (_currentBackColor.G < aimColor.G) _currentBackColor.G += diffG;
        else if (_currentBackColor.G > aimColor.G) _currentBackColor.G -= diffG;
        if (_currentBackColor.B < aimColor.B) _currentBackColor.B += diffB;
        else if (_currentBackColor.B > aimColor.B) _currentBackColor.B -= diffB;
    }

    private void ShowText()
    {
        String text = String.Empty;
        switch (Index)
        {
            case 0: text = _dialogues.Count > 0 ? _dialogues[0] : String.Empty; break;
            case 2: text = _dialogues.Count > 1 ? _dialogues[1] : String.Empty; break;
            case 6: text = NameReaction(_name); break;
            case 8: text = _name + (_dialogues.Count > 2 ? _dialogues[2] : String.Empty); break;
        }
        TextBox.reDelay = 0;
        TextBox.Show(text, []);
        Index += 1;
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), _currentBackColor);

        TextBox.Draw();

        if (_mainTexture != null)
        {
            Core.SpriteBatch.Draw(_mainTexture,
                new Rectangle(Core.windowSize.Width / 2 - 62, Core.windowSize.Height / 2 - 218, 130, 256),
                new Rectangle(0, 0, 62, 128),
                new Color(255, 255, 255, _profAlpha));
        }

        if (_pokeTexture != null && _mainTexture != null)
        {
            switch (_pokeIndex)
            {
                case 1:
                    Core.SpriteBatch.Draw(_mainTexture,
                        new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, 22, 22),
                        new Rectangle(62 + (int)(_ballIndex.X * 22), 48 + (int)(_ballIndex.Y * 22), 22, 22),
                        Color.White);
                    break;
                case 2:
                    Core.SpriteBatch.Draw(_pokeTexture,
                        new Rectangle(
                            (int)_pokePosition.X - Math.Min(_pokeTexture.Width, 128),
                            (int)_pokePosition.Y - Math.Min(_pokeTexture.Height, 128),
                            Math.Min(_pokeTexture.Width * 2, 256),
                            Math.Min(_pokeTexture.Height * 2, 256)),
                        Color.White);
                    break;
                case 3:
                    if (Index < 6)
                    {
                        Core.SpriteBatch.Draw(_pokeTexture,
                            new Rectangle(
                                (int)_pokePosition.X - Math.Min(_pokeTexture.Width, 128),
                                Core.windowSize.Height / 2 - Math.Min(_pokeTexture.Height, 128),
                                Math.Min(_pokeTexture.Width * 2, 256),
                                Math.Min(_pokeTexture.Height * 2, 256)),
                            new Color(255, 255, 255, _profAlpha));
                    }
                    break;
            }
        }
    }

    private void UpdatePokemon()
    {
        if (_pokeTexture == null) return;
        switch (_pokeIndex)
        {
            case 0:
                _ballPosition = new Vector2(Core.windowSize.Width / 2 - 40, Core.windowSize.Height / 2 - 110);
                _pokePosition = new Vector2(Core.windowSize.Width / 2 - Math.Min(_pokeTexture.Width * 2, 256), Core.windowSize.Height / 2 - Math.Min(_pokeTexture.Height * 2, 256));
                _pokeIndex = 1;
                AnimateBall();
                break;
            case 1:
                if (_ballPosition.X > Core.windowSize.Width / 2 - Math.Min(_pokeTexture.Width * 2, 256))
                {
                    _ballPosition.X -= 3;
                    _ballPosition.Y += _ballVelocity;
                    _ballVelocity += 0.2f;
                }
                else
                {
                    _pokeIndex = 2;
                }
                AnimateBall();
                break;
            case 2:
                if (_pokePosition.Y < Core.windowSize.Height / 2)
                {
                    _pokePosition.Y += 5;
                }
                else
                {
                    Pokemon p = Pokemon.GetPokemonByID(_pokeID);
                    p.PlayCry();
                    _pokeIndex = 3;
                    Index = 2;
                }
                break;
        }
    }

    private void AnimateBall()
    {
        if (_ballAnimationDelay <= 0.0f)
        {
            _ballIndex.X += 1;
            if (_ballIndex.X == 2 && _ballIndex.Y == 2) _ballIndex = Vector2.Zero;
            if (_ballIndex.X == 2) { _ballIndex.X = 0; _ballIndex.Y += 1; }
            _ballAnimationDelay = 0.2f;
        }
        else
        {
            _ballAnimationDelay -= 0.1f;
        }
    }

    private void UpdateTransition(bool toProf)
    {
        if (toProf == true)
        {
            if (_otherAlpha > 0) _otherAlpha -= 5;
            if (_otherAlpha <= 0)
            {
                if (_profAlpha < 255)
                {
                    _profAlpha += 5;
                    if (_profAlpha >= 255) Index += 1;
                }
            }
        }
        else
        {
            if (_profAlpha > 0) _profAlpha -= 5;
            if (_profAlpha <= 0)
            {
                if (_otherAlpha < 255)
                {
                    _otherAlpha += 5;
                    if (_otherAlpha >= 255) Index += 1;
                }
            }
        }
    }

    private void UpdateTextbox()
    {
        LoadSkinTexture();
        Core.SetScreen(new InputScreen(Core.CurrentScreen!, _skinNames.Length > _skinIndex ? _skinNames[_skinIndex] : "Player",
            InputScreen.InputModes.Name,
            _skinNames.Length > _skinIndex ? _skinNames[_skinIndex] : "Player",
            14, [_skinTexture!], ConfirmInput));
    }

    private void ConfirmInput(String input)
    {
        _name = input;
    }

    private void CreateGame()
    {
        String folderPath = _name
            .Replace("\\", "_").Replace("/", "_").Replace(":", "_")
            .Replace("*", "_").Replace("?", "_").Replace("\"", "_")
            .Replace("<", "_").Replace(">", "_").Replace("|", "_")
            .Replace(",", "_").Replace(".", "_");
        int folderPrefix = 0;

        if (folderPath.ToLower() == "autosave") folderPath = "autosave0";

        while (Directory.Exists(Path.Combine(AppPaths.SaveDir, folderPath)) == true)
        {
            if (folderPath != _name)
                folderPath = folderPath.Remove(folderPath.Length - folderPrefix.ToString().Length, folderPrefix.ToString().Length);
            folderPath += folderPrefix;
            folderPrefix += 1;
        }

        Directory.CreateDirectory(AppPaths.SaveDir);

        Core.Player.FilePrefix = folderPath;
        Core.Player.GameStart = DateTime.Now;
        Core.Player.GameMode = GameModeManager.ActiveGameMode!.DirectoryName;
        Core.Player.StartFOV = 60;
        Core.Player.StartFreeCameraMode = true;
        Core.Player.StartPosition = _startPosition;
        Core.Player.StartMap = _startMap;
        Core.Player.StartRotationSpeed = 12;
        Core.Player.StartSurfing = false;
        Core.Player.StartThirdPerson = false;
        Core.Player.StartRiding = false;
        Core.Player.StartRotation = _startYaw;

        Core.Player.PokedexData = Pokedex.NewPokedex();
        Core.Player.BerryData = CreateBerryData();
        Core.Player.AddVisitedMap(_startMap);
        Core.Player.SaveCreated = GameController.GAMEDEVELOPMENT_STAGE + " " + GameController.GAME_VERSION;

        String slotDir = Path.Combine(AppPaths.SaveDir, folderPath);
        Directory.CreateDirectory(slotDir);

        File.WriteAllText(Path.Combine(slotDir, "Player.dat"), GetPlayerData());
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

        Core.Player.IsGameJoltSave = false;
        Core.Player.LoadGame(folderPath);
        Core.SetScreen(new TransitionScreen(this, new OverworldScreen(), Color.Black, false, 5));
    }

    private String GetPlayerData()
    {
        String ot = Core.Random.Next(0, 65256).ToString();
        while (ot.Length < 5) ot = "0" + ot;

        String pos = _startPosition.X.ToString().Replace(GameController.DecSeparator, ".") + "," +
                     _startPosition.Y.ToString().Replace(GameController.DecSeparator, ".") + "," +
                     _startPosition.Z.ToString().Replace(GameController.DecSeparator, ".");

        return "Name|" + _name + Environment.NewLine +
            "Position|" + pos + Environment.NewLine +
            "MapFile|" + _startMap + Environment.NewLine +
            "Rotation|" + _startYaw + Environment.NewLine +
            "RivalName|???" + Environment.NewLine +
            "RivalSkin|4" + Environment.NewLine +
            "Money|3000" + Environment.NewLine +
            "Badges|0" + Environment.NewLine +
            "Gender|" + (_skinGenders.Length > _skinIndex ? _skinGenders[_skinIndex] : "Male") + Environment.NewLine +
            "PlayTime|0,0,0,0" + Environment.NewLine +
            "OT|" + ot + Environment.NewLine +
            "Points|0" + Environment.NewLine +
            "hasPokedex|0" + Environment.NewLine +
            "hasPokegear|0" + Environment.NewLine +
            "FreeCamera|1" + Environment.NewLine +
            "ThirdPerson|0" + Environment.NewLine +
            "Skin|" + (_skinFiles.Length > _skinIndex ? _skinFiles[_skinIndex] : "Hilbert") + Environment.NewLine +
            "Location|" + _startLocation + Environment.NewLine +
            "BattleAnimations|1" + Environment.NewLine +
            "RunMode|1" + Environment.NewLine +
            "RunToggled|1" + Environment.NewLine +
            "BoxAmount|10" + Environment.NewLine +
            "LastRestPlace|" + _startMap + Environment.NewLine +
            "LastRestPlacePosition|" + pos + Environment.NewLine +
            "DiagonalMovement|0" + Environment.NewLine +
            "RepelSteps|0" + Environment.NewLine +
            "ScriptDelayItems|" + Environment.NewLine +
            "ScriptDelaySteps|0" + Environment.NewLine +
            "ScriptDelayDisplaySteps|0" + Environment.NewLine +
            "LastSavePlace|" + _startMap + Environment.NewLine +
            "LastSavePlacePosition|" + pos + Environment.NewLine +
            "Difficulty|" + GameModeManager.GetGameRuleValue("Difficulty", "0") + Environment.NewLine +
            "BattleStyle|1" + Environment.NewLine +
            "SaveCreated|" + GameController.GAMEDEVELOPMENT_STAGE + " " + GameController.GAME_VERSION + Environment.NewLine +
            "LastPokemonPosition|999,999,999" + Environment.NewLine +
            "DaycareSteps|0" + Environment.NewLine +
            "GameMode|" + GameModeManager.ActiveGameMode!.DirectoryName + Environment.NewLine +
            "PokeFiles|" + Environment.NewLine +
            "VisitedMaps|" + _startMap + Environment.NewLine +
            "TempSurfSkin|" + (_skinFiles.Length > _skinIndex ? _skinFiles[_skinIndex] : "Hilbert") + Environment.NewLine +
            "Surfing|0" + Environment.NewLine +
            "BP|0" + Environment.NewLine +
            "Coins|0" + Environment.NewLine +
            "ExpAll|False" + Environment.NewLine +
            "ShowModels|1" + Environment.NewLine +
            "GTSStars|8" + Environment.NewLine +
            "SandBoxMode|0" + Environment.NewLine +
            "EarnedAchievements|";
    }

    public static String GetOptionsData()
    {
        return "FOV|60" + Environment.NewLine +
               "TextSpeed|2" + Environment.NewLine +
               "MouseSpeed|12";
    }

    private static String CreateBerryData()
    {
        if (File.Exists(GameModeManager.GetContentFilePath(@"Data\BerryData.dat")))
        {
            String[] berries = File.ReadAllLines(GameModeManager.GetContentFilePath(@"Data\BerryData.dat"));
            List<String> lines = [];
            foreach (String line in berries)
                if (line != String.Empty) lines.Add(line);
            return String.Join(Environment.NewLine, lines);
        }
        else if (GameModeManager.ActiveGameMode?.IsDefaultGamemode == true)
        {
            return "{route29.dat|13,0,5|6|2|0|2012,9,21,4,0,0|1}" + Environment.NewLine +
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
        return String.Empty;
    }

    private static String NameReaction(String name)
    {
        String[] weirdNames = ["derp", "karp"];
        String[] knownNames = ["ash", "gary", "misty", "brock", "tracey", "may", "max", "dawn", "iris", "cilan", "red", "blue", "green", "gold", "silver"];
        String[] ownNames = ["oak", "samuel", "prof. oak", "prof oak"];

        if (Array.Exists(weirdNames, n => n == name.ToLower()))
            return Localization.GetString("new_game_intro_weird_name_1") + name + Localization.GetString("new_game_intro_weird_name_2");
        if (Array.Exists(knownNames, n => n == name.ToLower()))
            return Localization.GetString("new_game_intro_known_name_1") + name + Localization.GetString("new_game_intro_known_name_2");
        if (Array.Exists(ownNames, n => n == name.ToLower()))
            return Localization.GetString("new_game_intro_same_name_1") + name + Localization.GetString("new_game_intro_same_name_2");

        return Localization.GetString("new_game_intro_name_1") + name + Localization.GetString("new_game_intro_name_2");
    }
}
