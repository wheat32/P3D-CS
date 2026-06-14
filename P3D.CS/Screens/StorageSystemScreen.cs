using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class StorageSystemScreen : Screen
{
    public enum FeatureTypes { Deposit, Withdraw, Organize }

    public enum SelectionModes { SingleMove, EasyMove, ItemMove, Withdraw, Deposit }

    private enum CursorModes { Selection, Box }

    public enum FilterTypes
    {
        Pokémon,
        Type1,
        Type2,
        Move,
        Ability,
        Nature,
        Gender,
        HeldItem
    }

    public FeatureTypes featureType = FeatureTypes.Organize;
    public SelectionModes selectionMode = SelectionModes.SingleMove;

    public struct Filter
    {
        public FilterTypes FilterType;
        public String FilterValue;
    }

    public List<Filter> filters = [];

    public static int tileOffset = 0;

    private Microsoft.Xna.Framework.Graphics.RenderTarget2D _renderTarget = null!;
    private CursorModes _cursorMode = CursorModes.Selection;
    private Vector2 _cursorPosition = Vector2.Zero;
    private Vector2 _cursorMovePosition = Vector2.Zero;
    private Vector2 _cursorAimPosition = Vector2.Zero;
    private bool _cursorMoving = false;
    private int _cursorSpeed = 0;

    private Pokemon? _movingPokemon = null;
    private Vector2 _pickupPlace = new Vector2(1);
    private int _pickupBox = 0;

    private Texture2D _texture = null!;
    private Texture2D _menuTexture = null!;

    private List<MenuEntry> _menuEntries = [];
    private bool _menuVisible = false;
    private int _menuCursor = 0;
    private String _menuHeader = String.Empty;

    private bool _boxChooseMode = false;

    private List<Box> _boxes = [];
    private int _currentBox = 0;

    private float _modelRoll = 0.0F;
    private float _modelPan = 0.0F;
    private bool _clickedObject = false;
    private int _yOffset = 0;

    public StorageSystemScreen(Screen currentScreen)
    {
        _renderTarget = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(Core.GraphicsDevice, 1200, 680, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        PreScreen = currentScreen;
        Identification = Identifications.StorageSystemScreen;
        MouseVisible = true;
        CanBePaused = true;
        CanChat = true;
        CanMuteAudio = true;

        _texture = TextureManager.GetTexture("GUI\\Box\\storage");
        _menuTexture = TextureManager.GetTexture("GUI\\Menus\\General");

        LoadScreen();
    }

    public static List<Box> LoadBoxes()
    {
        Dictionary<int, Box> boxes = [];

        for (int i = 0; i <= Core.Player.BoxAmount - 1; i++)
            boxes.Add(i, new Box(i));

        foreach (String line in Core.Player.BoxData.SplitAtNewline())
        {
            if (line.StartsWith("BOX") == false && line != String.Empty)
            {
                String[] data = line.Split(',');
                int boxIndex = int.Parse(data[0]);
                int pokemonIndex = int.Parse(data[1]);
                String pokemonData = line.Remove(0, line.IndexOf('{'));

                if (boxes.ContainsKey(boxIndex) == false)
                    boxes.Add(boxIndex, new Box(boxIndex));

                if (boxes[boxIndex].pokemon.ContainsKey(pokemonIndex) == false)
                    boxes[boxIndex].pokemon.Add(pokemonIndex, new PokemonWrapper(pokemonData));
            }
            else if (line.StartsWith("BOX") == true)
            {
                String[] boxData = line.Split('|');
                int boxIndex = int.Parse(boxData[1]);

                if (boxIndex > boxes.Count - 1)
                {
                    int boxCount = boxes.Count + 5;
                    if (boxCount > 30) boxCount = 30;
                    for (int b = boxes.Count - 1; b <= boxCount - 1; b++)
                    {
                        if (boxes.ContainsKey(b) == false)
                            boxes.Add(b, new Box(b));
                    }
                }

                if (boxes.ContainsKey(boxIndex) == false)
                    boxes.Add(boxIndex, new Box(boxIndex));

                boxes[boxIndex].background = int.Parse(boxData[3]);
                boxes[boxIndex].name = boxData[2];
            }
        }

        (int min, int max) bounds = (boxes.Min(x => x.Value.index), boxes.Max(x => x.Value.index));

        for (int i = bounds.min; i <= bounds.max; i++)
        {
            if (boxes.ContainsKey(i) == false)
                boxes.Add(i, new Box(i));
        }
        boxes[bounds.max].IsBattleBox = true;

        return boxes.Values.ToList();
    }

    private void LoadScreen()
    {
        selectionMode = Player.Temp.PCSelectionType;
        _cursorMode = CursorModes.Selection;
        _cursorPosition = Player.Temp.StorageSystemCursorPosition;
        _boxes = LoadBoxes();
        _currentBox = Player.Temp.PCBoxIndex;
        _boxChooseMode = Player.Temp.PCBoxChooseMode;
    }

    public override void Update()
    {
        TextBox.Update();

        if (ControllerHandler.ButtonPressed(Buttons.Back) == true || KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true)
            Core.SetScreen(new StorageSystemFilterScreen(this));

        if (TextBox.Showing == false)
        {
            if (_menuVisible == true)
            {
                for (int i = 0; i <= _menuEntries.Count - 1; i++)
                {
                    if (i <= _menuEntries.Count - 1)
                        _menuEntries[i].Update(this);
                }

                if (Controls.Up(true, true) == true) _menuCursor -= 1;
                if (Controls.Down(true, true) == true) _menuCursor += 1;

                int maxIndex = _menuEntries.Max(x => x.index);
                int minIndex = _menuEntries.Min(x => x.index);

                if (_menuCursor > maxIndex) _menuCursor = minIndex;
                if (_menuCursor < minIndex) _menuCursor = maxIndex;
            }
            else
            {
                TurnModel();
                if (_cursorMoving == true)
                {
                    MoveCursor();
                }
                else
                {
                    bool rightInput = ControllerHandler.ButtonPressed(Buttons.RightTrigger) == true || Controls.Right(true, false, true, false, false, false) == true;
                    bool leftInput = ControllerHandler.ButtonPressed(Buttons.LeftTrigger) == true || Controls.Left(true, false, true, false, false, false) == true;
                    if (leftInput == true) _currentBox -= 1;
                    if (rightInput == true) _currentBox += 1;
                    if (_currentBox < 0) _currentBox = _boxes.Count - 1;
                    if (_currentBox > _boxes.Count - 1) _currentBox = 0;

                    PressNumberButtons();

                    if (GetRelativeMousePosition() != new Vector2(-1) && GetRelativeMousePosition() == _cursorPosition && Controls.Accept(true, false, false) == true)
                    {
                        SoundManager.PlaySound("select");
                        ChooseObject();
                    }

                    ControlCursor();

                    if (Controls.Accept(false, true, true) == true)
                    {
                        SoundManager.PlaySound("select");
                        ChooseObject();
                    }

                    if (Controls.Dismiss(true, true, true) == true)
                    {
                        SoundManager.PlaySound("select");
                        CloseScreen();
                    }
                }
            }

            StorageSystemScreen.tileOffset = (StorageSystemScreen.tileOffset + 1) % 64;
        }
    }

    private void TurnModel()
    {
        if (Controls.ShiftDown("L", false) == true) _modelRoll -= 0.1F;
        if (Controls.ShiftDown("R", false) == true) _modelRoll += 0.1F;
        if (ControllerHandler.ButtonDown(Buttons.RightThumbstickLeft | Buttons.RightThumbstickRight) == false) return;
        GamePadState gPadState = GamePad.GetState(PlayerIndex.One);
        _modelRoll -= gPadState.ThumbSticks.Right.X * 0.1F;
    }

    private void PressNumberButtons()
    {
        int switchTo = KeyBoardHandler.KeyPressed(Keys.D0) == true ? 9 : -1;
        if (switchTo < 0)
        {
            IEnumerable<Keys> keysPressed = KeyBoardHandler.GetPressedKeys().Where(key => key >= Keys.D1 && key <= Keys.D9);
            switchTo = keysPressed.Count() < 1 ? switchTo : (int)(keysPressed.Max() - Keys.D1);
        }

        if (switchTo < 0) return;
        if (_boxes.Count - 1 >= switchTo) _currentBox = switchTo;
    }

    private void ChooseObject()
    {
        if (_cursorPosition.Y == 0)
        {
            if (_cursorPosition.X == 0) _currentBox -= 1;
            if (_cursorPosition.X == 5) _currentBox += 1;
            if (_currentBox < 0) _currentBox = _boxes.Count - 1;
            if (_currentBox > _boxes.Count - 1) _currentBox = 0;
            if (_cursorPosition.X == 6)
            {
                SelectPokemon();
            }
            else if (_cursorPosition.X > 0 && _cursorPosition.X < 5)
            {
                if (_boxChooseMode == true)
                {
                    _boxChooseMode = false;
                    return;
                }
                List<MenuEntry> entries = [];
                entries.Add(new MenuEntry(3, Localization.GetString("storage_screen_menu_box_ChooseBox", "Choose Box"), false, () => _boxChooseMode = !_boxChooseMode));
                entries.Add(new MenuEntry(4, Localization.GetString("storage_screen_menu_box_ChangeMode", "Change Mode"), false, ChangeModeMenu));
                if (GetBox(_currentBox).IsBattleBox == false)
                {
                    entries.Add(new MenuEntry(5, Localization.GetString("storage_screen_menu_box_Wallpaper", "Wallpaper"), false, WallpaperMain));
                    entries.Add(new MenuEntry(6, Localization.GetString("global_name", "Name"), false, SelectNameBox));
                }
                entries.Add(new MenuEntry(entries.Max(x => x.index) + 1, Localization.GetString("global_cancel", "Cancel"), true, null));
                SetupMenu(entries.ToArray(), Localization.GetString("storage_screen_menu_box_WhatDoYouWant", "What do you want to do?"));
            }
        }
        else if (_cursorPosition.Y < 6)
        {
            if (_boxChooseMode == false || _cursorPosition.X > 5 || _cursorPosition.Y < 1)
            {
                SelectPokemon();
                return;
            }
            int id = (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
            if (GetBox(id) == null) return;
            _currentBox = id;
            _boxChooseMode = false;
        }
    }

    private void ChangeModeMenu()
    {
        MenuEntry e = new MenuEntry(3, Localization.GetString("storage_screen_menu_mode_Withdraw", "Withdraw"), false, () => selectionMode = SelectionModes.Withdraw);
        MenuEntry e1 = new MenuEntry(4, Localization.GetString("storage_screen_menu_mode_Deposit", "Deposit"), false, () => selectionMode = SelectionModes.Deposit);
        MenuEntry e2 = new MenuEntry(5, Localization.GetString("storage_screen_menu_mode_SingleMove", "Single Move"), false, () => selectionMode = SelectionModes.SingleMove);
        MenuEntry e3 = new MenuEntry(6, Localization.GetString("storage_screen_menu_mode_EasyMove", "Easy Move"), false, () => selectionMode = SelectionModes.EasyMove);
        MenuEntry e4 = new MenuEntry(7, Localization.GetString("global_cancel", "Cancel"), true, ChooseObject);
        SetupMenu([e, e1, e2, e3, e4], Localization.GetString("storage_screen_menu_mode_ChooseModeToUse", "Choose a mode to use."));
    }

    private void SelectNameBox()
    {
        Box box = GetBox(_currentBox);
        String defaultName = Localization.GetString("storage_screen_box_DefaultName", "BOX [NUMBER]").Replace("[NUMBER]", $"{box.index + 1}");
        InputScreen.InputModes inputMode = InputScreen.InputModes.Text;
        InputScreen.ConfirmInput rename = name => box.name = name;
        InputScreen screen = new InputScreen(Core.CurrentScreen, defaultName, inputMode, box.name, 11, [], rename);
        Core.SetScreen(screen);
    }

    private void WallpaperMain()
    {
        int badges = Core.Player.Badges.Count;
        Dictionary<String, int> package1 = [];
        Dictionary<String, int> package2 = [];
        Dictionary<String, int> package3 = [];
        Dictionary<String, int> package4 = [];
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_Forest", "Forest"), 0);
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_City", "City"), 1);
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_Desert", "Desert"), 2);
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_Savanna", "Savanna"), 3);
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_Cave", "Cave"), 8);
        package1.Add(Localization.GetString("storage_screen_menu_wallpaper_River", "River"), 11);

        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Volcano", "Volcano"), 5);
        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Snow", "Snow"), 6);
        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Beach", "Beach"), 9);
        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Seafloor", "Seafloor"), 10);
        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Crag", "Crag"), 4);
        package2.Add(Localization.GetString("storage_screen_menu_wallpaper_Steel", "Steel"), 7);

        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_Volcano2", "Volcano 2"), 14);
        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_City2", "City 2"), 15);
        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_Snow2", "Snow 2"), 16);
        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_Desert2", "Desert 2"), 17);
        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_Savanna2", "Savanna 2"), 18);
        package3.Add(Localization.GetString("storage_screen_menu_wallpaper_Steel2", "Steel 2"), 19);

        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_System", "System"), 22);
        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_Simple ", "Simple "), 13);
        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_Checks ", "Checks "), 12);
        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_Seasons", "Seasons"), 23);
        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_Retro1", "Retro 1"), 20);
        package4.Add(Localization.GetString("storage_screen_menu_wallpaper_Retro2", "Retro 2"), 21);

        if (Core.Player.SandBoxMode == true || GameController.IS_DEBUG_ACTIVE == true)
            badges = 16;

        List<MenuEntry> entries = [];
        entries.Add(new MenuEntry(3, Localization.GetString("storage_screen_menu_theme_Package1", "Package 1"), false, () => WallpaperList(package1)));
        if (badges > 1) entries.Add(new MenuEntry(4, Localization.GetString("storage_screen_menu_theme_Package2", "Package 2"), false, () => WallpaperList(package2)));
        if (badges > 4) entries.Add(new MenuEntry(5, Localization.GetString("storage_screen_menu_theme_Package3", "Package 3"), false, () => WallpaperList(package3)));
        if (badges > 7) entries.Add(new MenuEntry(6, Localization.GetString("storage_screen_menu_theme_Package4", "Package 4"), false, () => WallpaperList(package4)));
        entries.Add(new MenuEntry(entries.Max(x => x.index) + 1, Localization.GetString("global_cancel", "Cancel"), true, ChooseObject));
        SetupMenu(entries.ToArray(), Localization.GetString("storage_screen_menu_theme_PickTheme", "Please pick a theme."));
    }

    private void WallpaperList(Dictionary<String, int> package)
    {
        List<MenuEntry> itemList = new List<MenuEntry>(package.Count + 1);
        int index = 3;
        foreach (KeyValuePair<String, int> wallpaper in package)
        {
            int wValue = wallpaper.Value;
            itemList.Add(new MenuEntry(index, wallpaper.Key, false, () => GetBox(_currentBox).background = wValue));
            index += 1;
        }
        itemList.Add(new MenuEntry(index, Localization.GetString("global_cancel", "Cancel"), true, WallpaperMain));
        SetupMenu(itemList.ToArray(), Localization.GetString("storage_screen_menu_wallpaper_PickWallpaper", "Pick the wallpaper."));
    }

    private void GetYOffset(Pokemon p)
    {
        Texture2D t = p.GetTexture(true);
        _yOffset = -1;

        Color[] cArr = new Color[t.Width * t.Height];
        t.GetData(cArr);

        bool found = false;
        for (int y = 0; y <= t.Height - 1 && found == false; y++)
        {
            for (int x = 0; x <= t.Width - 1; x++)
            {
                if (cArr[x + y * t.Height] == Color.Transparent) continue;
                _yOffset = y;
                found = true;
                break;
            }
        }
    }

    private void MoveCursor()
    {
        bool changedPosition = _cursorMovePosition != _cursorAimPosition;

        Vector2 difference = _cursorMovePosition - _cursorAimPosition;
        Vector2 speed = new Vector2(Math.Sign(difference.X), Math.Sign(difference.Y)) * _cursorSpeed;
        _cursorMovePosition -= speed;

        _cursorMovePosition.X = speed.X > 0 ? Math.Max(_cursorMovePosition.X, _cursorAimPosition.X) : Math.Min(_cursorMovePosition.X, _cursorAimPosition.X);
        _cursorMovePosition.Y = speed.Y > 0 ? Math.Max(_cursorMovePosition.Y, _cursorAimPosition.Y) : Math.Min(_cursorMovePosition.Y, _cursorAimPosition.Y);

        if (_cursorAimPosition != _cursorMovePosition) return;
        _cursorMoving = false;

        if (selectionMode == SelectionModes.EasyMove && changedPosition == true && _clickedObject == true)
            ChooseObject();
    }

    private void ControlCursor()
    {
        Vector2 preCursor = _cursorPosition;
        Box box = GetBox(_currentBox);
        Vector2 direction = Vector2.Zero;
        bool cancel = ControllerHandler.ButtonPressed(Buttons.X) == true;
        bool confirm = Controls.Accept(true, false, false) == true && GetRelativeMousePosition() != new Vector2(-1);
        _cursorMovePosition = GetAbsoluteCursorPosition(_cursorPosition);

        if (cancel == true)
        {
            _cursorPosition = new Vector2(1, 0);
        }
        else if (confirm == true)
        {
            _cursorPosition = GetRelativeMousePosition();
        }
        else
        {
            if (Controls.Right(true, true, false) == true) direction.X += 1;
            if (Controls.Left(true, true, false) == true) direction.X -= 1;
            if (Controls.Up(true, true, false) == true) direction.Y -= 1;
            if (Controls.Down(true, true, false) == true) direction.Y += 1;
            _cursorPosition += direction;

            if (direction.X > 0 && _cursorPosition.Y == 0 && _cursorPosition.X > 1 && _cursorPosition.X < 5)
                _cursorPosition.X = 5;
            if (direction.X < 0 && _cursorPosition.Y == 0 && _cursorPosition.X > 0 && _cursorPosition.X < 4)
                _cursorPosition.X = 0;
            if (direction.Y > 0 && _cursorPosition.Y == 1 && box.IsBattleBox == true && _cursorPosition.X < 6 && _boxChooseMode == false)
                _cursorPosition.X = 2;
        }

        _cursorMoving = cancel || confirm || direction != Vector2.Zero;
        _clickedObject = confirm;

        int[] xRange = [0, 6];

        if (_boxChooseMode == false)
        {
            if (selectionMode == SelectionModes.Withdraw && _cursorPosition.Y > 0)
                xRange = box.IsBattleBox == true ? [2, 3] : [0, 5];
            else if (selectionMode == SelectionModes.Deposit && _cursorPosition.Y > 0)
                xRange = [6, 6];
            else if (box.IsBattleBox == true)
                xRange = _cursorPosition.Y == 0 ? [0, 6] : [2, 6];
        }

        if (_cursorPosition.X < xRange[0]) _cursorPosition.X = xRange[1];
        if (_cursorPosition.X > xRange[1]) _cursorPosition.X = xRange[0];

        if (box.IsBattleBox == true && _boxChooseMode == false)
        {
            if (_cursorPosition.Y > 0 && _cursorPosition.X > 3 && _cursorPosition.X < 6)
                _cursorPosition.X = preCursor.X > _cursorPosition.X ? 3 : 6;
        }

        int[] yRange = [0, 5];

        if (_boxChooseMode == false)
        {
            if (box.IsBattleBox == true && _cursorPosition.X < 6)
                yRange = [0, 3];
        }

        if (_cursorPosition.Y < yRange[0]) _cursorPosition.Y = yRange[1];
        if (_cursorPosition.Y > yRange[1]) _cursorPosition.Y = yRange[0];

        _cursorAimPosition = GetAbsoluteCursorPosition(_cursorPosition);
        _cursorSpeed = (int)(Vector2.Distance(_cursorMovePosition, _cursorAimPosition) * 0.3F);
    }

    private void CloseScreen()
    {
        if (_boxChooseMode == true)
        {
            _boxChooseMode = false;
            return;
        }
        if (_movingPokemon != null)
        {
            if (_pickupPlace.X == 6)
            {
                Core.Player.Pokemons.Add(_movingPokemon);
            }
            else
            {
                int id = (int)_pickupPlace.X + (int)((_pickupPlace.Y - 1) * 6);
                Box box = GetBox(_pickupBox);
                int index = box.IsBattleBox == true ? box.pokemon.Count : id;
                box.pokemon.Add(index, new PokemonWrapper(_movingPokemon));
                _currentBox = _pickupBox;
            }
            _movingPokemon = null;
        }
        else
        {
            if (GetBox(_currentBox).IsBattleBox == true)
            {
                for (int newBoxIndex = 0; newBoxIndex <= _boxes.Count - 1; newBoxIndex++)
                {
                    if (_boxes[newBoxIndex].IsFull == false)
                    {
                        Player.Temp.PCBoxIndex = newBoxIndex;
                        break;
                    }
                }
            }
            else
            {
                Player.Temp.PCBoxIndex = _currentBox;
            }

            Player.Temp.StorageSystemCursorPosition = _cursorPosition;
            Player.Temp.PCBoxChooseMode = _boxChooseMode;
            Player.Temp.PCSelectionType = selectionMode;

            Core.Player.BoxData = GetBoxSaveData(_boxes);
            Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.Black, false));
        }
    }

    private static String GetBoxSaveData(List<Box> boxes)
    {
        bool boxesFull = true;
        List<String> newData = [];

        foreach (Box b in boxes)
        {
            if (b.IsBattleBox == true) continue;
            newData.Add($"BOX|{b.index}|{b.name}|{b.background}");

            bool hasPokemon = false;
            for (int i = 0; i <= 29; i++)
            {
                if (b.pokemon.ContainsKey(i) == false) continue;
                hasPokemon = true;
                newData.Add($"{b.index},{i},{b.pokemon[i].PokemonData}");
            }
            if (hasPokemon == false) boxesFull = false;
        }

        int addedBoxes = 0;
        if (boxesFull == true && boxes.Count < 30)
        {
            int newBoxes = 5;
            if (boxes.Count + 5 > 30)
                newBoxes = 30 - boxes.Count;

            addedBoxes = newBoxes;
            Core.Player.BoxAmount = boxes.Count + newBoxes;

            for (int i = 0; i <= newBoxes - 1; i++)
            {
                int newBoxID = boxes.Count - 1 + i;
                newData.Add($"BOX|{newBoxID}|BOX {newBoxID + 1}|{Core.Random.Next(0, 19)}");
            }
        }

        Box battleBox = boxes.Last();
        newData.Add($"BOX|{boxes.Count - 1 + addedBoxes}|{battleBox.name}|{battleBox.background}");

        for (int i = 0; i <= 29; i++)
        {
            if (battleBox.pokemon.ContainsKey(i) == false) continue;
            newData.Add($"{boxes.Count - 1 + addedBoxes},{i},{battleBox.pokemon[i].PokemonData}");
        }

        return String.Join(Environment.NewLine, newData);
    }

    private Vector2 GetRelativeMousePosition()
    {
        for (int x = 0; x <= 5; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                Rectangle boxTile = new Rectangle(50 + x * 100, 200 + y * 84, 64, 64);
                if (boxTile.Contains(MouseHandler.MousePosition) == true) return new Vector2(x, y + 1);
            }
        }

        for (int y = 0; y <= 5; y++)
        {
            Rectangle partyTile = new Rectangle(Core.windowSize.Width - 260, y * 100 + 50, 128, 80);
            if (partyTile.Contains(MouseHandler.MousePosition) == true) return new Vector2(6, y);
        }

        Rectangle labelArea = new Rectangle(80, 50, 600, 100);
        Rectangle leftArrowArea = new Rectangle(10, 52, 96, 96);
        Rectangle rightArrowArea = new Rectangle(655, 52, 96, 96);
        if (labelArea.Contains(MouseHandler.MousePosition) == true) return new Vector2(1, 0);
        if (leftArrowArea.Contains(MouseHandler.MousePosition) == true) return new Vector2(0, 0);
        if (rightArrowArea.Contains(MouseHandler.MousePosition) == true) return new Vector2(5, 0);

        return new Vector2(-1);
    }

    private Vector2 GetAbsoluteCursorPosition(Vector2 relPos)
    {
        if (relPos.Y == 0)
        {
            if (relPos.X < 0 || relPos.X > 6) return new Vector2();
            Vector2 leftArrow = new Vector2(60, 20);
            Vector2 rightArrow = new Vector2(705, 20);
            Vector2 label = new Vector2(380, 30);
            Vector2 party = new Vector2(Core.windowSize.Width - 200, 20);
            Vector2[] positions = [leftArrow, label, label, label, label, rightArrow, party];
            return positions[(int)relPos.X];
        }
        if (relPos.Y > 0 && relPos.Y < 6)
        {
            Vector2 boxTile = new Vector2(50 + relPos.X * 100 + 42, 200 + (relPos.Y - 1) * 84 - 42);
            Vector2 partyTile = new Vector2(Core.windowSize.Width - 200, 20 + 100 * relPos.Y);
            if (relPos.X >= 0 && relPos.X < 6) return boxTile;
            if (relPos.X == 6) return partyTile;
        }
        return new Vector2();
    }

    private int GetBattleBoxID()
    {
        if (_cursorPosition.Y < 1 || _cursorPosition.Y > 3) return -1;
        if (_cursorPosition.X == 2) return (int)(_cursorPosition.Y * 2 - 2);
        if (_cursorPosition.X == 3) return (int)(_cursorPosition.Y * 2 - 1);
        return -1;
    }

    private void SelectPokemon()
    {
        if (selectionMode == SelectionModes.EasyMove)
        {
            PickupPokemon();
            return;
        }
        if (selectionMode != SelectionModes.SingleMove && selectionMode != SelectionModes.Withdraw && selectionMode != SelectionModes.Deposit)
            return;
        if (_movingPokemon != null)
        {
            PickupPokemon();
            return;
        }

        Box box = GetBox(_currentBox);
        int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);

        bool hasPokemonInBox = box.pokemon.ContainsKey(id) && _cursorPosition.X < 6;
        bool hasPokemonInParty = _cursorPosition.X == 6 && Core.Player.Pokemons.Count - 1 >= (int)_cursorPosition.Y;

        if (hasPokemonInBox == true || hasPokemonInParty == true)
        {
            Pokemon p = _cursorPosition.X == 6 ? Core.Player.Pokemons[(int)_cursorPosition.Y] : box.pokemon[id].pokemon;
            List<MenuEntry> entries = [];

            if (selectionMode == SelectionModes.Withdraw)
                entries.Add(new MenuEntry(3, Localization.GetString("storage_screen_menu_pokemon_Withdraw", "Withdraw"), false, WithdrawPokemon));
            else if (selectionMode == SelectionModes.Deposit)
                entries.Add(new MenuEntry(3, Localization.GetString("storage_screen_menu_pokemon_Deposit", "Deposit"), false, DepositPokemonFromParty));
            else
                entries.Add(new MenuEntry(3, Localization.GetString("storage_screen_menu_pokemon_Move", "Move"), false, PickupPokemon));

            entries.Add(new MenuEntry(4, Localization.GetString("global_summary", "Summary"), false, SummaryPokemon));

            int itemOffset = p.Item != null ? 1 : 0;

            if (p.Item != null) entries.Add(new MenuEntry(5, Localization.GetString("storage_screen_menu_pokemon_TakeItem", "Take Item"), false, () => TakeItemPokemon()));
            entries.Add(new MenuEntry(5 + itemOffset, Localization.GetString("storage_screen_menu_pokemon_Release", "Release"), false, ReleasePokemon));
            entries.Add(new MenuEntry(6 + itemOffset, Localization.GetString("global_cancel", "Cancel"), true, null));
            SetupMenu(entries.ToArray(), Localization.GetString("storage_screen_menu_pokemon_IsSelected", "[POKEMONNAME] is selected.").Replace("[POKEMONNAME]", p.GetDisplayName()));
        }
    }

    private void PickupPokemon()
    {
        if (_cursorPosition.X == 6)
        {
            if (Core.Player.Pokemons.Count - 1 >= _cursorPosition.Y)
            {
                List<Pokemon> l = new List<Pokemon>(Core.Player.Pokemons.ToArray());
                l.RemoveAt((int)_cursorPosition.Y);
                if (_movingPokemon != null) l.Add(_movingPokemon);
                bool hasPokemon = l.Any(p => p.IsEgg == false && p.Status != Pokemon.StatusProblems.Fainted && p.HP > 0);

                if (hasPokemon == false)
                {
                    SetupMenu([new MenuEntry(3, Localization.GetString("global_ok", "OK"), true, null)], Localization.GetString("storage_screen_pokemon_CannotRemoveLastPokemon", "Can't remove last Pokémon from party."));
                }
                else
                {
                    if (_movingPokemon != null)
                    {
                        Pokemon sPokemon = Core.Player.Pokemons[(int)_cursorPosition.Y];
                        _movingPokemon.FullRestore();
                        Core.Player.Pokemons.Insert((int)_cursorPosition.Y, _movingPokemon);
                        _movingPokemon = sPokemon;
                        Core.Player.Pokemons.RemoveAt((int)_cursorPosition.Y + 1);
                    }
                    else
                    {
                        _movingPokemon = Core.Player.Pokemons[(int)_cursorPosition.Y];
                        Core.Player.Pokemons.RemoveAt((int)_cursorPosition.Y);
                        _pickupBox = 0;
                        _pickupPlace = new Vector2(6, 0);
                    }
                }
            }
            else if (_movingPokemon != null)
            {
                _movingPokemon.FullRestore();
                Core.Player.Pokemons.Add(_movingPokemon);
                _movingPokemon = null;
            }
        }
        else
        {
            Box box = GetBox(_currentBox);
            int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
            bool pokemonExists = box.pokemon.ContainsKey(id);

            if (pokemonExists == true)
            {
                if (_movingPokemon == null)
                {
                    _movingPokemon = box.pokemon[id].pokemon;
                    box.pokemon.Remove(id);
                    _pickupBox = _currentBox;
                    _pickupPlace = _cursorPosition;
                    RearrangeBattleBox(box);
                }
                else
                {
                    _movingPokemon.FullRestore();
                    Pokemon sPokemon = box.pokemon[id].pokemon;
                    box.pokemon[id] = new PokemonWrapper(_movingPokemon);
                    _movingPokemon = sPokemon;
                }
            }
            else if (_movingPokemon != null)
            {
                _movingPokemon.FullRestore();
                int index = box.IsBattleBox == true ? box.pokemon.Count : id;
                box.pokemon.Add(index, new PokemonWrapper(_movingPokemon));
                _movingPokemon = null;
            }
        }
    }

    private void WithdrawPokemon()
    {
        Box box = GetBox(_currentBox);
        int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);

        if (Core.Player.Pokemons.Count > 5)
        {
            SetupMenu([new MenuEntry(3, Localization.GetString("global_ok", "OK"), true, null)], Localization.GetString("storage_screen_pokemon_PartyIsFull", "Party is full!"));
        }
        else if (box.pokemon.ContainsKey(id) == true)
        {
            Core.Player.Pokemons.Add(box.pokemon[id].pokemon);
            box.pokemon.Remove(id);
        }
        RearrangeBattleBox(box);
    }

    private void DepositPokemonFromParty()
    {
        Box box = GetBox(_currentBox);
        if (box.pokemon.Count > 29) return;
        if (Core.Player.Pokemons.Count - 1 < (int)_cursorPosition.Y) return;

        List<Pokemon> l = new List<Pokemon>(Core.Player.Pokemons.ToArray());
        l.RemoveAt((int)_cursorPosition.Y);
        bool hasPokemon = l.Any(p => p.IsEgg == false && p.Status != Pokemon.StatusProblems.Fainted && p.HP > 0);

        if (hasPokemon == false)
        {
            SetupMenu([new MenuEntry(3, Localization.GetString("global_ok", "OK"), true, null)], Localization.GetString("storage_screen_pokemon_CannotRemoveLastPokemon", "Can't remove last Pokémon from party."));
        }
        else
        {
            int nextIndex = 0;
            while (box.pokemon.ContainsKey(nextIndex) == true)
                nextIndex += 1;
            Core.Player.Pokemons[(int)_cursorPosition.Y].FullRestore();
            box.pokemon.Add(nextIndex, new PokemonWrapper(Core.Player.Pokemons[(int)_cursorPosition.Y]));
            Core.Player.Pokemons.RemoveAt((int)_cursorPosition.Y);
        }
    }

    private void SummaryPokemon()
    {
        if (_cursorPosition.X == 6)
        {
            Core.SetScreen(new SummaryScreen(this, Core.Player.Pokemons.ToArray(), (int)_cursorPosition.Y));
            return;
        }
        Box box = GetBox(_currentBox);
        int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
        List<Pokemon> pokemonList = box.GetPokemonList();
        int partyIndex = pokemonList.IndexOf(box.pokemon[id].pokemon);
        Core.SetScreen(new SummaryScreen(this, pokemonList.ToArray(), partyIndex));
    }

    private String TakeItemPokemon(bool logImmediate = true)
    {
        Box box = GetBox(_currentBox);
        int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
        Pokemon pokemon = _cursorPosition.X == 6 ? Core.Player.Pokemons[(int)_cursorPosition.Y] : box.pokemon[id].pokemon;
        String message = String.Empty;
        if (pokemon.Item == null) return message;
        if (pokemon.Item.IsMail == true && pokemon.Item.AdditionalData != String.Empty)
        {
            message = Localization.GetString("storage_screen_pokemon_item_MailWasTaken", "The Mail was taken to your~inbox on your PC.");
            Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(pokemon.Item.AdditionalData));
        }
        else
        {
            message = Localization.GetString("storage_screen_pokemon_item_TakenItemFromPokemon", "Taken [ITEM]~from [POKEMONNAME].").Replace("[ITEM]", pokemon.Item.OneLineName()).Replace("[POKEMONNAME]", pokemon.GetDisplayName());
            String itemID = pokemon.Item.IsGameModeItem == true ? pokemon.Item.gmID : pokemon.Item.ID.ToString();
            Core.Player.Inventory.AddItem(itemID, 1);
        }
        if (logImmediate == true) Screen.TextBox.Show(message);
        pokemon.Item = null;
        return message;
    }

    private void ReleasePokemon()
    {
        bool hasPokemon = false;

        if (_cursorPosition.X != 6)
        {
            hasPokemon = true;
        }
        else
        {
            List<Pokemon> l = new List<Pokemon>(Core.Player.Pokemons.ToArray());
            l.RemoveAt((int)_cursorPosition.Y);
            hasPokemon = l.Any(p => p.IsEgg == false && p.Status != Pokemon.StatusProblems.Fainted && p.HP > 0);
        }

        if (hasPokemon == true)
        {
            Box box = GetBox(_currentBox);
            int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
            Pokemon p = _cursorPosition.X == 6 ? Core.Player.Pokemons[(int)_cursorPosition.Y] : box.pokemon[id].pokemon;

            if (p.IsEgg == false)
            {
                MenuEntry e1 = new MenuEntry(3, Localization.GetString("global_no", "No"), true, SelectPokemon);
                MenuEntry e = new MenuEntry(4, Localization.GetString("global_yes", "Yes"), false, ConfirmRelease);
                SetupMenu([e1, e], Localization.GetString("storage_screen_pokemon_release_Question", "Release [POKEMONNAME]?").Replace("[POKEMONNAME]", p.GetDisplayName()));
            }
            else
            {
                SetupMenu([new MenuEntry(3, Localization.GetString("global_ok", "OK"), true, null)], Localization.GetString("storage_screen_pokemon_release_CannotReleaseEgg", "Cannot release an Egg."));
            }
        }
        else
        {
            SetupMenu([new MenuEntry(3, Localization.GetString("global_ok", "OK"), true, null)], Localization.GetString("storage_screen_pokemon_release_CannotReleaseLastPokemon", "Cannot release the last Pokémon."));
        }
    }

    private void ConfirmRelease()
    {
        int id = (int)_cursorPosition.X + (int)((_cursorPosition.Y - 1) * 6);
        Box box = GetBox(_currentBox);
        Pokemon pokemon = _cursorPosition.X == 6 ? Core.Player.Pokemons[(int)_cursorPosition.Y] : box.pokemon[id].pokemon;
        String text = String.Empty;
        if (pokemon.Item != null) text += TakeItemPokemon(false);
        if (text != String.Empty) text += "*";
        text += Localization.GetString("storage_screen_pokemon_release_Goodbye", "Goodbye, [POKEMONNAME]!").Replace("[POKEMONNAME]", pokemon.GetDisplayName());
        Screen.TextBox.Show(text);

        if (_cursorPosition.X == 6)
            Core.Player.Pokemons.RemoveAt((int)_cursorPosition.Y);
        else
            box.pokemon.Remove(id);
    }

    private static void RearrangeBattleBox(Box b)
    {
        if (b.IsBattleBox == false) return;
        List<Pokemon> p = b.GetPokemonList();
        b.pokemon.Clear();
        for (int i = 0; i <= p.Count - 1; i++)
            b.pokemon.Add(i, new PokemonWrapper(p[i]));
    }

    public override void Draw()
    {
        DrawMainWindow();
        DrawPokemonStatus();
        DrawTopBar();
        DrawTeamWindow();

        Action draw = _menuVisible == true ? DrawMenuEntries : DrawCursor;
        draw();
        TextBox.Draw();
    }

    private void DrawTopBar()
    {
        int boxIndex = _currentBox;
        if (_boxChooseMode == true)
            boxIndex = _cursorPosition.X < 6 && _cursorPosition.Y > 0 ? (int)(_cursorPosition.X + (_cursorPosition.Y - 1) * 6) : _currentBox;

        Box b = GetBox(boxIndex);
        if (b == null) return;

        String texturePath = b.IsBattleBox == true ? "GUI\\Box\\BattleBox" : $"GUI\\Box\\{b.background}";
        Core.SpriteBatch.Draw(TextureManager.GetTexture(texturePath), new Rectangle(80, 50, 600, 100), Color.White);

        Color[] cArr = new Color[1];
        TextureManager.GetTexture(texturePath, new Rectangle(0, 0, 1, 1), String.Empty).GetData(cArr);
        Canvas.DrawScrollBar(new Vector2(80, 36), _boxes.Count, 1, boxIndex, new Size(600, 14), true, Color.Transparent, cArr[0]);

        SpriteFont font = FontManager.MainFont;
        float textWidth = font.MeasureString(b.name).X;
        Vector2 labelPosition = new Vector2(380 - textWidth, 76);
        Vector2 labelShadowPosition = labelPosition + new Vector2(4);

        Core.SpriteBatch.DrawString(font, b.name, labelShadowPosition, Color.Black, 0.0F, Vector2.Zero, 2, SpriteEffects.None, 0.0F);
        Core.SpriteBatch.DrawString(font, b.name, labelPosition, Color.White, 0.0F, Vector2.Zero, 2, SpriteEffects.None, 0.0F);

        Rectangle textureArea = new Rectangle(0, 16, 16, 16);
        Rectangle leftArrowArea = new Rectangle(10, 52, 96, 96);
        Rectangle rightArrowArea = new Rectangle(655, 52, 96, 96);
        Core.SpriteBatch.Draw(_menuTexture, leftArrowArea, textureArea, Color.White);
        Core.SpriteBatch.Draw(_menuTexture, rightArrowArea, textureArea, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);
    }

    private void DrawPokemonIcon(Rectangle area, Pokemon pokemon, bool drawItem = false, bool drawShadow = false)
    {
        Color light = IsLit(pokemon) == true ? Color.White : new Color(65, 65, 65, 255);
        Texture2D texture = pokemon.GetMenuTexture();
        if (drawShadow == true)
        {
            Rectangle shadowArea = new Rectangle(area.Location + new Point(10, 10), area.Size);
            Core.SpriteBatch.Draw(texture, shadowArea, new Color(0, 0, 0, 150));
        }
        Core.SpriteBatch.Draw(texture, area, light);
        if (pokemon.IsEgg == true || drawItem == false || pokemon.Item == null) return;
        Rectangle itemArea = new Rectangle(area.Location + new Point(32), new Point(24));
        Core.SpriteBatch.Draw(pokemon.Item.Texture, itemArea, Color.White);
    }

    private void DrawMainWindow()
    {
        Color backgroundColor = new Color(220, 220, 220);

        if (_boxChooseMode == true)
        {
            Canvas.DrawRectangle(Core.windowSize, backgroundColor);

            for (int x = 0; x <= 5; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    int id = y * 6 + x;
                    if (_boxes.Count - 1 < id) continue;

                    Box pBox = GetBox(id);
                    int pCount = pBox == null ? 0 : pBox.pokemon.Values.Where(p => IsLit(p.pokemon) == true).Count();

                    Point empty = new Point(64, 32);
                    Point notEmpty = new Point(64, 0);
                    Point full = new Point(32, 32);
                    Point tCoord = notEmpty;
                    if (pCount == 0) tCoord = empty;
                    if (pCount == 30) tCoord = full;

                    Rectangle tileArea = new Rectangle(new Point(x, y) * new Point(100, 84) + new Point(50, 200), new Point(64));
                    Rectangle textureArea = new Rectangle(tCoord, new Point(32));
                    Core.SpriteBatch.Draw(_texture, tileArea, textureArea, Color.White);
                }
            }
            return;
        }

        Box box = GetBox(_currentBox);
        PokemonWrapper? wrapper = null;
        Color[] cArr = new Color[1];
        String background = box.IsBattleBox == true ? "GUI\\Box\\Battlebox" : $"GUI\\Box\\{box.background}";
        TextureManager.GetTexture(background, new Rectangle(0, 0, 1, 1), String.Empty).GetData(cArr, 0, 1);
        backgroundColor = new Color(cArr[0].R, cArr[0].G, cArr[0].B, (byte)150);

        if (box.IsBattleBox == true)
        {
            Canvas.DrawGradient(Core.windowSize, new Color(203, 40, 41), new Color(238, 128, 128), false, -1);

            for (int i = 0; i <= 5; i++)
            {
                int bx = i + 2;
                int by = 0;
                while (bx > 3) { bx -= 2; by += 1; }
                Rectangle area = new Rectangle(new Point(bx, by) * new Point(100, 84) + new Point(50, 200), new Point(64));
                Canvas.DrawRectangle(area, backgroundColor);
                if (box.pokemon.TryGetValue(i, out wrapper) == false) continue;
                DrawPokemonIcon(area, wrapper.pokemon, true);
            }
        }
        else
        {
            int xt = box.background;
            int yt = 0;
            while (xt > 7) { xt -= 8; yt += 1; }

            for (int x = 0; x <= Core.windowSize.Width; x += 64)
            {
                for (int y = 0; y <= Core.windowSize.Height; y += 64)
                    Core.SpriteBatch.Draw(_texture, new Rectangle(x, y, 64, 64), new Rectangle(xt * 16, yt * 16 + 64, 16, 16), Color.White);
            }

            for (int x = 0; x <= 5; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    int id = y * 6 + x;
                    Rectangle area = new Rectangle(new Point(x, y) * new Point(100, 84) + new Point(50, 200), new Point(64));
                    Canvas.DrawRectangle(area, backgroundColor);
                    if (box.pokemon.TryGetValue(id, out wrapper) == false) continue;
                    DrawPokemonIcon(area, wrapper.pokemon, true);
                }
            }

            String text = Localization.GetString("storage_screen_filter_Hint", "Press <system.button(special)> on the keyboard to filter.");
            Vector2 textPosition = new Vector2(44, 200 + 5 * 84);
            Vector2 shadowPosition = textPosition + new Vector2(2);
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, shadowPosition, Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, textPosition, Color.White);
        }
    }

    private void DrawPokemonStatus()
    {
        if (_boxChooseMode == true && _cursorPosition.X < 6 && _cursorPosition.Y > 0)
        {
            Box box = GetBox((int)(_cursorPosition.X + (_cursorPosition.Y - 1) * 6));
            if (box == null) return;

            Rectangle overviewArea = new Rectangle(660, 200, 200, 200);
            Rectangle detailsArea = new Rectangle(660, 410, 200, 210);
            Canvas.DrawRectangle(overviewArea, new Color(84, 198, 216, 150));

            int minLevel = int.MaxValue;
            int maxLevel = int.MinValue;

            for (int x = 0; x <= 5; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    int id = y * 6 + x;
                    if (box.pokemon.TryGetValue(id, out PokemonWrapper? wrapper) == false) continue;
                    Point position = new Point(x, y) * new Point(32) + new Point(664, 215);
                    Pokemon pokemon = wrapper.pokemon;
                    DrawPokemonIcon(new Rectangle(position, new Point(32)), pokemon);
                    minLevel = Math.Min(minLevel, pokemon.Level);
                    maxLevel = Math.Max(maxLevel, pokemon.Level);
                }
            }

            Canvas.DrawRectangle(detailsArea, new Color(84, 198, 216, 150));

            String levelString = (minLevel == int.MaxValue || maxLevel == int.MinValue) ? Localization.GetString("global_none", "None") : $"{minLevel} - {maxLevel}";
            int maxPokemon = box.IsBattleBox == true ? 6 : 30;

            String t = Localization.GetString("storage_screen_pokemon_summary_Box", "Box") + $":  {box.name}{Environment.NewLine}";
            t += Localization.GetString("storage_screen_pokemon_summary_Pokemon", "Pokémon") + $":  {box.pokemon.Count} / {maxPokemon}{Environment.NewLine}";
            t += Localization.GetString("storage_screen_pokemon_summary_Level", "Level") + $":  {levelString}";

            Vector2 infoPosition = new Vector2(665, 415);
            Vector2 shadowPos = infoPosition + new Vector2(2);
            Core.SpriteBatch.DrawString(FontManager.MainFont, t, shadowPos, Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, t, infoPosition, Color.White);
        }
        else
        {
            Box box = GetBox(_currentBox);
            Pokemon? p = _movingPokemon;

            if (p == null)
            {
                if (_cursorPosition.X == 6)
                {
                    if (Core.Player.Pokemons.Count - 1 >= _cursorPosition.Y)
                        p = Core.Player.Pokemons[(int)_cursorPosition.Y];
                }
                else
                {
                    int id = box.IsBattleBox == true ? GetBattleBoxID() : (int)(_cursorPosition.X + (_cursorPosition.Y - 1) * 6);
                    if (box.pokemon.ContainsKey(id) == true)
                        p = box.pokemon[id].pokemon;
                }
            }

            if (p == null) return;

            Color[] cArr = new Color[1];
            String texturePath = box.IsBattleBox == true ? "GUI\\Box\\BattleBox" : $"GUI\\Box\\{box.background}";
            TextureManager.GetTexture(texturePath, new Rectangle(0, 0, 1, 1), String.Empty).GetData(cArr, 0, 1);

            Rectangle overviewArea = new Rectangle(660, 200, 256, 256);
            Rectangle detailsArea = new Rectangle(660, 472, 320, 240);
            Color backgroundColor = _boxChooseMode == true ? new Color(84, 198, 216, 150) : new Color(cArr[0].R, cArr[0].G, cArr[0].B, (byte)150);

            Canvas.DrawRectangle(overviewArea, backgroundColor);

            String modelName = p.AnimationName;
            String shinyString = p.IsShiny == true ? Localization.GetString("storage_screen_pokemon_summary_Shiny", "Shiny") : Localization.GetString("storage_screen_pokemon_summary_Normal", "Normal");

            if (Core.Player.ShowModelsInBattle == true && ModelManager.ModelExist($"Models\\Pokemon\\{modelName}\\{shinyString}") == true && p.IsEgg == false)
            {
                Draw3DModel(p, $"Models\\Pokemon\\{modelName}\\{shinyString}");
            }
            else
            {
                GetYOffset(p);
                Texture2D texture = p.GetTexture(true);
                Vector2 sizeVec = Vector2.Min(new Vector2(texture.Width, texture.Height) * 3, new Vector2(288, 288));
                Point size = new Point((int)sizeVec.X, (int)sizeVec.Y);
                Point position = new Point(792 - size.X / 2, 192 - _yOffset);
                Core.SpriteBatch.Draw(texture, new Rectangle(position, size), Color.White);
            }

            Canvas.DrawRectangle(detailsArea, backgroundColor);

            String text = String.Empty;
            if (p.IsEgg == true)
            {
                text = Localization.GetString("storage_screen_pokemon_summary_Egg", "Egg");
            }
            else
            {
                String itemString = p.Item == null ? Localization.GetString("global_none", "None") : p.Item.OneLineName();
                String nameString = p.NickName == String.Empty ? p.GetDisplayName() : $"{p.GetDisplayName()}/{p.GetName()}";

                text = $"{nameString}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_DexNo", "DEX NO.") + $"  {p.Number}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_LEVEL", "LEVEL") + $"  {p.Level}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_HP", "HP") + $"  {p.HP} / {p.MaxHP}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_ATTACK", "ATTACK") + $"  {p.Attack}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_DEFENSE", "DEFENSE") + $"  {p.Defense}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_SpATK", "SP. ATK") + $"  {p.SpAttack}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_SpDEF", "SP. DEF") + $"  {p.SpDefense}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_SPEED", "SPEED") + $"  {p.Speed}{Environment.NewLine}";
                text += Localization.GetString("storage_screen_pokemon_summary_ITEM", "ITEM") + $"  {itemString}";
            }

            Vector2 textPosition = new Vector2(665, 477);
            Vector2 shadowPosition = textPosition + new Vector2(2);
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, shadowPosition, Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, text, textPosition, Color.White);
        }
    }

    private void Draw3DModel(Pokemon p, String modelName)
    {
        Tuple<float, float, float, float, float> propList = p.GetModelProperties();
        float scale = propList.Item1 * 10 * ModelManager.PokeModelScale(modelName);
        Vector3 position = new Vector3(propList.Item2, propList.Item3, propList.Item4);
        float roll = propList.Item5;
        Vector3 rotation = new Vector3(ModelManager.PokeModelRotation(modelName).Z + roll + _modelRoll, ModelManager.PokeModelRotation(modelName).X, ModelManager.PokeModelRotation(modelName).Y);
        Texture2D t = ModelManager.DrawModelToTexture(modelName, _renderTarget, position, new Vector3(0, 10, 50), rotation, scale, true);
        Core.SpriteBatch.Draw(t, new Rectangle(192, 72, 1200, 680), Color.White);
    }

    private void DrawTeamWindow()
    {
        Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width - 310, 0, 400, Core.windowSize.Height), new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
        {
            Rectangle scrollArea = new Rectangle(Core.windowSize.Width - 128, y + StorageSystemScreen.tileOffset, 128, 64);
            Rectangle scrollTextureArea = new Rectangle(48, 0, 16, 16);
            Core.SpriteBatch.Draw(_menuTexture, scrollArea, scrollTextureArea, Color.White);
        }

        int halfHeight = Core.windowSize.Height / 2;
        Rectangle cutoutArea = new Rectangle(Core.windowSize.Width - 430, 0, 128, halfHeight);
        Rectangle cutoutTextureArea = new Rectangle(96, 0, 32, 64);

        Core.SpriteBatch.Draw(_texture, cutoutArea, cutoutTextureArea, Color.White);
        cutoutArea.Location += new Point(0, halfHeight);
        Core.SpriteBatch.Draw(_texture, cutoutArea, cutoutTextureArea, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipVertically, 0.0F);

        for (int i = 0; i <= 5; i++)
        {
            Rectangle outlineArea = new Rectangle(Core.windowSize.Width - 260, i * 100 + 50, 128, 80);
            Canvas.DrawBorder(2, outlineArea, new Color(42, 167, 198));
            if (Core.Player.Pokemons.Count - 1 < i) continue;
            Pokemon pokemon = Core.Player.Pokemons[i];
            Rectangle spriteArea = new Rectangle(Core.windowSize.Width - 228, i * 100 + 60, 64, 64);
            DrawPokemonIcon(spriteArea, pokemon, true);
        }
    }

    private void DrawCursor()
    {
        Point cPosition = (_cursorMoving == true ? _cursorMovePosition : GetAbsoluteCursorPosition(_cursorPosition)).ToPoint();

        if (_movingPokemon != null)
        {
            Rectangle spriteArea = new Rectangle(cPosition + new Point(-20, 34), new Point(64));
            DrawPokemonIcon(spriteArea, _movingPokemon, true, true);
        }

        Texture2D? cursorTexture = GetCursorTexture();
        if (cursorTexture != null)
            Core.SpriteBatch.Draw(cursorTexture, new Rectangle(cPosition, new Point(64)), Color.White);
    }

    private void DrawMenuEntries()
    {
        if (_menuHeader != String.Empty)
        {
            SpriteFont font = FontManager.MainFont;
            Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width - 370, 100, 356, 64), new Color(0, 0, 0, 180));
            Core.SpriteBatch.DrawString(font, _menuHeader, new Vector2(Core.windowSize.Width - 192 - font.MeasureString(_menuHeader).X / 2, 120), Color.White);
        }

        Texture2D? cursorTexture = GetCursorTexture();
        _menuEntries.ForEach(x => x.Draw(_menuCursor, cursorTexture));
    }

    private Texture2D? GetCursorTexture()
    {
        Dictionary<SelectionModes, Rectangle> rectangles = [];
        rectangles.Add(SelectionModes.SingleMove, new Rectangle(0, 0, 16, 16));
        rectangles.Add(SelectionModes.EasyMove, new Rectangle(16, 0, 16, 16));
        rectangles.Add(SelectionModes.Deposit, new Rectangle(32, 0, 16, 16));
        rectangles.Add(SelectionModes.Withdraw, new Rectangle(0, 32, 16, 16));

        return rectangles.ContainsKey(selectionMode) == true ? TextureManager.GetTexture("GUI\\Menus\\General", rectangles[selectionMode], String.Empty) : null;
    }

    private bool IsLit(Pokemon p)
    {
        if (filters.Count < 1) return true;
        if (p.IsEgg == true) return false;

        Dictionary<FilterTypes, Func<Filter, bool>> criteria = [];
        criteria.Add(FilterTypes.Ability, f => Localization.GetString("ability_name_" + p.Ability.ID).ToLower() == f.FilterValue.ToLower());
        criteria.Add(FilterTypes.Gender, f => Localization.GetString("global_" + p.Gender.ToString().ToLower()).ToLower() == f.FilterValue.ToLower());
        criteria.Add(FilterTypes.Nature, f => Localization.GetString("nature_name_" + p.Nature.ToString()).ToLower() == f.FilterValue.ToLower());
        criteria.Add(FilterTypes.Pokémon, f => p.GetName().ToLower() == f.FilterValue.ToLower());
        criteria.Add(FilterTypes.Move, f => p.attacks.Any(a => a.Name.ToLower() == f.FilterValue.ToLower()));
        criteria.Add(FilterTypes.Type1, f => p.Type1.ToString().ToLower() == f.FilterValue.ToLower());
        criteria.Add(FilterTypes.Type2, f => p.Type2.ToString().ToLower() == f.FilterValue.ToLower());

        foreach (Filter f in filters)
        {
            if (criteria.TryGetValue(f.FilterType, out Func<Filter, bool>? check) == true && check(f) == false) return false;
            if (f.FilterType == FilterTypes.HeldItem)
            {
                if (f.FilterValue == Localization.GetString("storage_screen_filter_HeldItem_No", "Has no Held Item") && p.Item != null) return false;
                if (f.FilterValue == Localization.GetString("storage_screen_filter_HeldItem_Yes", "Has a Held Item") && p.Item == null) return false;
            }
        }
        return true;
    }

    public static int DepositPokemon(Pokemon p, int boxIndex = -1)
    {
        p.FullRestore();
        List<Box> boxes = LoadBoxes();
        int startIndex = boxIndex > -1 ? boxIndex : 0;

        for (int i = startIndex; i <= boxes.Count - 1; i++)
        {
            Dictionary<int, PokemonWrapper> pokemons = GetBox(i, boxes).pokemon;
            if (pokemons.Count > 29) continue;
            for (int l = 0; l <= 29; l++)
            {
                if (pokemons.ContainsKey(l) == true) continue;
                pokemons.Add(l, new PokemonWrapper(p));
                break;
            }
            Core.Player.BoxData = GetBoxSaveData(boxes);
            return i;
        }

        if (startIndex == 0) return -1;
        for (int i = 0; i <= startIndex - 1; i++)
        {
            Dictionary<int, PokemonWrapper> pokemons = GetBox(i, boxes).pokemon;
            if (pokemons.Count > 29) continue;
            for (int l = 0; l <= 29; l++)
            {
                if (pokemons.ContainsKey(l) == true) continue;
                pokemons.Add(l, new PokemonWrapper(p));
                break;
            }
            Core.Player.BoxData = GetBoxSaveData(boxes);
            return i;
        }

        return -1;
    }

    public static String GetBoxName(int boxIndex) => GetBox(boxIndex, LoadBoxes()).name;

    private static Box GetBox(int index, List<Box> boxes) =>
        boxes.FirstOrDefault(x => x.index == index)!;

    private Box GetBox(int index) => GetBox(index, _boxes);

    private void SetupMenu(MenuEntry[] entries, String header)
    {
        _menuEntries.Clear();
        _menuEntries.AddRange(entries);
        _menuVisible = true;
        _menuCursor = _menuEntries[0].index;
        _menuHeader = header;
    }

    public static List<Pokemon> GetAllBoxPokemon()
    {
        List<Pokemon> pokemons = [];
        foreach (String line in Core.Player.BoxData.SplitAtNewline())
        {
            if (line.StartsWith("BOX|") == false && line != String.Empty)
            {
                String pokeData = line.Remove(0, line.IndexOf('{'));
                pokemons.Add(Pokemon.GetPokemonByData(pokeData));
            }
        }
        return pokemons;
    }

    public List<Pokemon> GetPokemonList(bool includeTeam, bool lit)
    {
        List<Pokemon> l = [];
        foreach (Box b in _boxes)
        {
            if (b.HasPokemon == false) continue;
            IEnumerable<Pokemon> boxPokemons = b.pokemon.Values.Select(x => x.pokemon);
            l.AddRange(boxPokemons.Where(pokemon => (lit == true && IsLit(pokemon) == true) || lit == false));
        }

        if (includeTeam == true)
            l.AddRange(Core.Player.Pokemons.Where(pokemon => (lit == true && IsLit(pokemon) == true) || lit == false));

        return l;
    }

    public static List<Pokemon> GetBattleBoxPokemon()
    {
        int battleBoxID = 0;
        String[] data = Core.Player.BoxData.SplitAtNewline();
        List<Pokemon> pokemonList = [];

        foreach (String line in data)
        {
            if (line.StartsWith("BOX|") == false) continue;
            String[] boxData = line.Split('|');
            battleBoxID = Math.Max(int.Parse(boxData[1]), battleBoxID);
        }
        foreach (String line in data)
        {
            if (line.StartsWith(battleBoxID.ToString() + ",") == false || line.EndsWith("}") == false) continue;
            String pokemonData = line.Remove(0, line.IndexOf('{'));
            pokemonList.Add(Pokemon.GetPokemonByData(pokemonData));
        }

        if (pokemonList.Count > 6)
            pokemonList.RemoveRange(5, pokemonList.Count - 6);

        return pokemonList;
    }

    public static void PokedexRegisterBoxPokemon()
    {
        foreach (String line in Core.Player.BoxData.SplitAtNewline())
        {
            if (line.StartsWith("BOX") == false && line != String.Empty)
            {
                String pokemonData = line.Remove(0, line.IndexOf('{'));
                Pokemon p = Pokemon.GetPokemonByData(pokemonData);
                String dexID = PokemonForms.GetPokemonDataFileName(p.Number, p.AdditionalData, true);

                if (p.IsEgg == false)
                {
                    if (p.IsShiny == true)
                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 3);
                    else
                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 2);
                }
            }
        }
    }

    public class PokemonWrapper
    {
        private Pokemon? _pokemon = null;
        private readonly String _pokemonData;
        private bool _loaded = false;

        public PokemonWrapper(String pokemonData)
        {
            _pokemonData = pokemonData;
        }

        public PokemonWrapper(Pokemon p)
        {
            _loaded = true;
            _pokemon = p;
            _pokemonData = p.GetSaveData();
        }

        public Pokemon pokemon
        {
            get
            {
                if (_loaded == true) return _pokemon!;
                _loaded = true;
                _pokemon = Pokemon.GetPokemonByData(_pokemonData);
                return _pokemon;
            }
        }

        public String PokemonData => _loaded == true ? _pokemon!.GetSaveData() : _pokemonData;
    }

    public class Box
    {
        public int index = 0;
        public String name = "BOX 0";
        public Dictionary<int, PokemonWrapper> pokemon = [];
        public int background = 0;
        public bool isSelected = false;

        private bool _isBattleBox = false;

        public Box(int index)
        {
            this.index = index;
            name = Localization.GetString("storage_screen_box_DefaultName", "BOX [NUMBER]").Replace("[NUMBER]", $"{index + 1}");
            background = index;
        }

        public bool HasPokemon => pokemon.Count > 0;

        public bool IsFull => pokemon.Count >= 30;

        public List<Pokemon> GetPokemonList() => pokemon.Values.Select(x => x.pokemon).ToList();

        public bool IsBattleBox
        {
            get => _isBattleBox;
            set
            {
                _isBattleBox = value;
                if (_isBattleBox == true)
                    name = Localization.GetString("storage_screen_box_BattleBoxName", "BATTLE BOX");
            }
        }
    }

    public class MenuEntry
    {
        public int index = 0;
        public String text = "Menu";
        public bool isBack = false;
        public Action? clickHandler = null;

        private Texture2D _t1 = null!;
        private Texture2D _t2 = null!;

        public MenuEntry(int index, String text, bool isBack, Action? clickHandler)
        {
            this.index = index;
            this.text = text;
            this.isBack = isBack;
            this.clickHandler = clickHandler;

            _t1 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(16, 16, 16, 16), String.Empty);
            _t2 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(32, 16, 16, 16), String.Empty);
        }

        public void Update(StorageSystemScreen s)
        {
            bool hovering = new Rectangle(Core.windowSize.Width - 270, 66 * index, 256, 64).Contains(MouseHandler.MousePosition);
            bool acceptPointer = Controls.Accept(true, false, false) == true && hovering == true;
            bool acceptButtons = Controls.Accept(false, true, true) == true;
            bool dismiss = Controls.Dismiss(true, true, true) == true && isBack == true;
            if (((acceptPointer == true || acceptButtons == true) && s._menuCursor == index) || dismiss == true)
            {
                s._menuVisible = false;
                clickHandler?.Invoke();
            }
            if (acceptPointer == true)
                s._menuCursor = index;
        }

        public void Draw(int cursorIndex, Texture2D? cursorTexture)
        {
            Point startPos = new Point(Core.windowSize.Width - 270, 66 * index);

            Core.SpriteBatch.Draw(_t1, new Rectangle(startPos.X, startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t2, new Rectangle(startPos.X + 64, startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t2, new Rectangle(startPos.X + 128, startPos.Y, 64, 64), Color.White);
            Core.SpriteBatch.Draw(_t1, new Rectangle(startPos.X + 192, startPos.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

            SpriteFont font = FontManager.MainFont;
            float textSize = font.MeasureString(text).X * 0.7F;
            Vector2 position = new Vector2(startPos.X + 128 - textSize, startPos.Y + 15);
            Core.SpriteBatch.DrawString(font, text, position, Color.Black, 0.0F, Vector2.Zero, 1.4F, SpriteEffects.None, 0.0F);

            if (index != cursorIndex) return;
            Point cPosition = startPos + new Point(128, -40);
            if (cursorTexture != null)
                Core.SpriteBatch.Draw(cursorTexture, new Rectangle(cPosition, new Point(64)), Color.White);
        }
    }
}

public class StorageSystemFilterScreen : Screen
{
    private class SelectMenu
    {
        private List<String> _items = [];
        private int _index = 0;
        private Action<SelectMenu>? _clickHandler = null;
        private int _backIndex = 0;
        public bool Visible = true;
        public int Scroll = 0;

        private Texture2D _t1 = null!;
        private Texture2D _t2 = null!;

        public SelectMenu(List<String> items, int index, Action<SelectMenu>? clickHandler, int backIndex)
        {
            _items = items;
            _index = index;
            _clickHandler = clickHandler;
            _backIndex = backIndex;
            if (_backIndex < 0)
                _backIndex = _items.Count + _backIndex;
            Visible = true;

            _t1 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(16, 16, 16, 16), String.Empty);
            _t2 = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(32, 16, 16, 16), String.Empty);
        }

        public void Update()
        {
            if (Visible == false) return;
            if (Controls.Up(true, true, true, true, true, true) == true) _index -= 1;
            if (Controls.Down(true, true, true, true, true, true) == true) _index += 1;
            _index = Math.Clamp(_index, 0, _items.Count - 1);

            for (int i = Scroll; i <= Scroll + 8; i++)
            {
                if (i > _items.Count - 1) continue;
                bool hovering = new Rectangle(Core.windowSize.Width - 398, 66 * (i + 1 - Scroll), 320, 64).Contains(MouseHandler.MousePosition);
                bool acceptPointer = Controls.Accept(true, false, false) == true && hovering == true;
                bool acceptButtons = Controls.Accept(false, true, true) == true;
                bool dismiss = Controls.Dismiss(true, true, true) == true;
                bool shouldAct = ((acceptPointer == true || acceptButtons == true) && i == _index) || (dismiss == true && _backIndex == _index);
                if (shouldAct == true)
                {
                    _clickHandler?.Invoke(this);
                    SoundManager.PlaySound("select");
                    Visible = false;
                }
                if (dismiss == true) _index = _backIndex;
                if (acceptPointer == true) _index = i;
            }

            if (_index - Scroll > 8) Scroll = _index - 8;
            if (_index - Scroll < 0) Scroll = _index;
        }

        public void Draw()
        {
            if (Visible == false) return;
            for (int i = Scroll; i <= Scroll + 8; i++)
            {
                if (i > _items.Count - 1) continue;
                String text = _items[i];
                Point startPos = new Point(Core.windowSize.Width - 398, 66 * ((i + 1) - Scroll));

                Core.SpriteBatch.Draw(_t1, new Rectangle(startPos.X, startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t2, new Rectangle(startPos.X + 64, startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t2, new Rectangle(startPos.X + 128, startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t2, new Rectangle(startPos.X + 192, startPos.Y, 64, 64), Color.White);
                Core.SpriteBatch.Draw(_t1, new Rectangle(startPos.X + 256, startPos.Y, 64, 64), null, Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

                SpriteFont font = FontManager.MainFont;
                Size textSize = new Size((int)(font.MeasureString(text).X * 1.3F / 2), (int)(font.MeasureString(text).Y * 1.3F / 2));
                Vector2 position = new Vector2(startPos.X + 160 - textSize.Width, startPos.Y + 32 - textSize.Height);

                Core.SpriteBatch.DrawString(font, text, position, Color.Black, 0.0F, Vector2.Zero, 1.3F, SpriteEffects.None, 0.0F);

                if (_index != i) continue;
                Point cPosition = startPos + new Point(160, -40);
                Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
                Core.SpriteBatch.Draw(t, new Rectangle(cPosition, new Point(64)), Color.White);
            }
        }

        public String SelectedItem => _items[_index];
    }

    private readonly StorageSystemScreen _storageSystemScreen;
    private Texture2D _texture = null!;

    private List<StorageSystemScreen.Filter> _filters = [];
    private SelectMenu _menu = null!;
    private List<String> _mainMenuItems = [];
    private int _cursor = 0;
    private int _scroll = 0;

    private int _results = 0;
    private List<StorageSystemScreen.Filter> _calculatedFilters = [];

    public StorageSystemFilterScreen(StorageSystemScreen currentScreen)
    {
        Identification = Identifications.StorageSystemFilterScreen;
        _storageSystemScreen = currentScreen;
        _texture = TextureManager.GetTexture("GUI\\Menus\\General");
        _filters.AddRange(currentScreen.filters);
        MouseVisible = true;
        CanMuteAudio = true;
        CanBePaused = true;

        _mainMenuItems =
        [
            Localization.GetString("storage_screen_filter_Pokemon", "Pokémon"),
            Localization.GetString("storage_screen_filter_Type1", "Type 1"),
            Localization.GetString("storage_screen_filter_Type2", "Type 2"),
            Localization.GetString("storage_screen_filter_Move", "Move"),
            Localization.GetString("storage_screen_filter_Ability", "Ability"),
            Localization.GetString("storage_screen_filter_Nature", "Nature"),
            Localization.GetString("storage_screen_filter_Gender", "Gender"),
            Localization.GetString("storage_screen_filter_HeldItem", "Held Item")
        ];

        _menu = new SelectMenu([""], 0, null, 0);
        _menu.Visible = false;
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
        {
            Rectangle source = new Rectangle(48, 0, 16, 16);
            Rectangle destination = new Rectangle(Core.windowSize.Width - 128, y + StorageSystemScreen.tileOffset, 128, 64);
            Core.SpriteBatch.Draw(_texture, destination, source, Color.White);
        }

        (Color A, Color B) tones = (new Color(42, 167, 198), new Color(42, 167, 198, 0));
        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 200), tones.A, tones.B, false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 200, Core.windowSize.Width, 200), tones.B, tones.A, false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("storage_screen_filter_Title", "Configure the filters:"), new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);

        for (int i = _scroll; i <= _scroll + 5; i++)
        {
            if (i > _mainMenuItems.Count - 1) continue;
            int p = i - _scroll;

            Core.SpriteBatch.Draw(_texture, new Rectangle(100, 100 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White);
            Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64, 100 + p * 96, 64 * 8, 64), new Rectangle(32, 16, 16, 16), Color.White);
            Core.SpriteBatch.Draw(_texture, new Rectangle(100 + 64 * 9, 100 + p * 96, 64, 64), new Rectangle(16, 16, 16, 16), Color.White, 0.0F, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0F);

            String filterText = GetFilterText(_mainMenuItems[i]);
            String s = filterText != String.Empty ? $"{_mainMenuItems[i]} ({filterText})" : _mainMenuItems[i];
            Rectangle sourceRectangle = filterText != String.Empty ? new Rectangle(16, 48, 16, 16) : new Rectangle(16, 32, 16, 16);
            Core.SpriteBatch.Draw(_texture, new Rectangle(120, 116 + p * 96, 32, 32), sourceRectangle, Color.White);
            Core.SpriteBatch.DrawString(FontManager.MainFont, s, new Vector2(160, 116 + p * 96), Color.Black, 0.0F, Vector2.Zero, 1.25F, SpriteEffects.None, 0.0F);
        }

        if (_filters.Count > 0)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, $"{Localization.GetString("storage_screen_filter_Results", "Results:")} {Environment.NewLine}{Environment.NewLine}{Localization.GetString("storage_screen_filter_Filters", "Filters:")} ", new Vector2(90 + 64 * 11, 119), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, $"{_results}{Environment.NewLine}{Environment.NewLine}{_filters.Count}", new Vector2(288 + 64 * 11, 119), Color.White);
        }

        Action draw = _menu.Visible == true ? _menu.Draw : DrawCursor;
        draw();
    }

    private String GetFilterText(String filterTypeString)
    {
        Func<StorageSystemScreen.Filter, bool> equals = f => $"{f.FilterType}".ToLower() == filterTypeString.ToLower();
        StorageSystemScreen.Filter? filter = _filters.Cast<StorageSystemScreen.Filter?>().FirstOrDefault(f => equals(f!.Value));
        return filter.HasValue == true ? filter.Value.FilterValue : String.Empty;
    }

    private void DrawCursor()
    {
        Point cPosition = new Point(520, 100 + _cursor * 96 - 42);
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle(cPosition, new Point(64)), Color.White);
    }

    private void ApplyFilters()
    {
        _storageSystemScreen.filters.Clear();
        _storageSystemScreen.filters.AddRange(_filters);
    }

    public override void Update()
    {
        if (_menu.Visible == true)
        {
            _menu.Update();
        }
        else
        {
            int direction = 0;
            if (Controls.Down(true, true, true, true, true, true) == true) direction = 1;
            if (Controls.Up(true, true, true, true, true, true) == true) direction = -1;
            _cursor += direction;
            if (Controls.ShiftDown() == true) _cursor += direction * 4;

            while (_cursor > 5) { _cursor -= 1; _scroll += 1; }
            while (_cursor < 0) { _cursor += 1; _scroll -= 1; }

            _scroll = _mainMenuItems.Count < 7 ? 0 : Math.Clamp(_scroll, 0, _mainMenuItems.Count - 6);
            _cursor = _mainMenuItems.Count < 6 ? Math.Clamp(_cursor, 0, _mainMenuItems.Count - 1) : Math.Clamp(_cursor, 0, 5);

            if (_mainMenuItems.Count > 0)
            {
                if (Controls.Accept(true, false, false) == true)
                {
                    for (int i = _scroll; i <= _scroll + 5; i++)
                    {
                        bool hovering = new Rectangle(100, 100 + (i - _scroll) * 96, 640, 64).Contains(MouseHandler.MousePosition);
                        if (i > _mainMenuItems.Count - 1 || hovering == false) continue;
                        if (i != _cursor + _scroll)
                        {
                            _cursor = i - _scroll;
                            continue;
                        }
                        SelectFilter();
                        SoundManager.PlaySound("select");
                    }
                }

                if (Controls.Accept(false, true, true) == true)
                {
                    SelectFilter();
                    SoundManager.PlaySound("select");
                }
            }

            if (Controls.Dismiss(true, true, true) == true)
            {
                ApplyFilters();
                Core.SetScreen(_storageSystemScreen);
                SoundManager.PlaySound("select");
            }
        }

        CalculateResults();
        StorageSystemScreen.tileOffset = (StorageSystemScreen.tileOffset + 1) % 64;
    }

    private void CalculateResults()
    {
        String s = String.Empty;
        String s1 = String.Empty;
        _calculatedFilters.ForEach(f => s += $"{f.FilterType}|{f.FilterValue}");
        _filters.ForEach(f => s1 += $"{f.FilterType}|{f.FilterValue}");

        if (s1 == s) return;
        _calculatedFilters.Clear();
        _calculatedFilters.AddRange(_filters);
        ApplyFilters();
        _results = _storageSystemScreen.GetPokemonList(true, true).Count;
    }

    private void SelectFilter()
    {
        String filterType = _mainMenuItems[_scroll + _cursor].ToLower();
        Dictionary<String, Action> menus = [];
        menus.Add(Localization.GetString("storage_screen_filter_Pokemon", "Pokémon").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Pokémon, true));
        menus.Add(Localization.GetString("storage_screen_filter_Type1", "Type 1").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Type1));
        menus.Add(Localization.GetString("storage_screen_filter_Type2", "Type 2").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Type2));
        menus.Add(Localization.GetString("storage_screen_filter_Move", "Move").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Move, true));
        menus.Add(Localization.GetString("storage_screen_filter_Ability", "Ability").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Ability, true));
        menus.Add(Localization.GetString("storage_screen_filter_Nature", "Nature").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Nature));
        menus.Add(Localization.GetString("storage_screen_filter_Gender", "Gender").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.Gender));
        menus.Add(Localization.GetString("storage_screen_filter_HeldItem", "Held Item").ToLower(), () => OpenMenu(StorageSystemScreen.FilterTypes.HeldItem));
        if (menus.ContainsKey(filterType) == true) menus[filterType]();
    }

    private void OpenMenu(StorageSystemScreen.FilterTypes filterType, bool letterFiltering = false)
    {
        List<Pokemon> l = _storageSystemScreen.GetPokemonList(true, false);
        Dictionary<StorageSystemScreen.FilterTypes, Func<IEnumerable<String>>> getNames = [];
        getNames.Add(StorageSystemScreen.FilterTypes.Pokémon, () => l.Select(pokemon => pokemon.GetName()));
        getNames.Add(StorageSystemScreen.FilterTypes.Type1, () => l.Select(p => $"{p.Type1}"));
        getNames.Add(StorageSystemScreen.FilterTypes.Type2, () => l.Select(p => $"{p.Type2}"));
        getNames.Add(StorageSystemScreen.FilterTypes.Move, () => l.SelectMany(p => p.attacks).Select(a => a.Name));
        getNames.Add(StorageSystemScreen.FilterTypes.Ability, () => l.Select(p => Localization.GetString("ability_name_" + p.Ability.ID)));
        getNames.Add(StorageSystemScreen.FilterTypes.Nature, () => l.Select(p => Localization.GetString("nature_name_" + p.Nature.ToString())));
        getNames.Add(StorageSystemScreen.FilterTypes.Gender, () => l.Select(p => Localization.GetString("global_" + p.Gender.ToString().ToLower())));
        getNames.Add(StorageSystemScreen.FilterTypes.HeldItem, () => [Localization.GetString("storage_screen_filter_HeldItem_Yes", "Has a Held Item"), Localization.GetString("storage_screen_filter_HeldItem_No", "Has no Held Item")]);

        List<String> names = getNames[filterType]().Distinct().ToList();
        names.Sort();

        List<String>? letters = letterFiltering == true ? names.Select(name => $"{name[0]}".ToUpper()).Distinct().ToList() : null;
        List<String> items = letterFiltering == true ? letters! : names;

        Action<SelectMenu> onClick = letterFiltering == true
            ? s => SelectLetter(s, names, filterType)
            : s => SelectType(s, filterType);

        items.Add(Localization.GetString("global_back", "Back"));
        if (GetFilterText($"{filterType}") != String.Empty) items.Insert(0, Localization.GetString("global_clear", "Clear"));
        _menu = new SelectMenu(items, 0, onClick, -1);
    }

    private void SelectLetter(SelectMenu s, List<String> names, StorageSystemScreen.FilterTypes filterType)
    {
        if (s.SelectedItem == Localization.GetString("global_back", "Back")) return;
        if (s.SelectedItem == Localization.GetString("global_clear", "Clear"))
        {
            _filters.RemoveAll(filter => filter.FilterType == filterType);
            return;
        }
        String chosenLetter = s.SelectedItem;
        List<String> buttonNames = names.Where(x => x.ToUpper().StartsWith(chosenLetter) == true).Distinct().ToList();
        buttonNames.Sort();
        buttonNames.Add(Localization.GetString("global_back", "Back"));
        _menu = new SelectMenu(buttonNames, 0, x => SelectType(x, filterType, false), -1);
    }

    private void SelectType(SelectMenu s, StorageSystemScreen.FilterTypes filterType, bool backIsRoot = true)
    {
        if (s.SelectedItem == Localization.GetString("global_back", "Back"))
        {
            if (backIsRoot == false) OpenMenu(filterType, true);
            return;
        }
        _filters.RemoveAll(filter => filter.FilterType == filterType);
        if (s.SelectedItem == Localization.GetString("global_clear", "Clear")) return;
        _filters.Add(new StorageSystemScreen.Filter { FilterType = filterType, FilterValue = s.SelectedItem });
    }
}
