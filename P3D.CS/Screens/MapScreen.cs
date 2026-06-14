using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class MapScreen : Screen
{
    public enum VisibleMode
    {
        Always,
        Temporary,
        Unlock,
        Register
    }

    public const int RASTER_SIZE = 32;
    private const int MAP_MOVE_SPEED = 3;

    public static int mapOffsetX = 100;
    public static int mapOffsetY = 75;

    private Object[] _flag = [];

    private List<String> _regions = [];
    private int _regionPointer = 0;
    private String _currentRegion = String.Empty;

    private List<City> _cities = [];
    private List<Route> _routes = [];
    private List<Place> _places = [];
    private List<Roaming> _roamingPoke = [];

    private Texture2D _playerSkinTexture = null!;
    private Texture2D _objectsTexture = null!;
    private Texture2D _mapTexture = null!;
    private Texture2D _texture = null!;

    private String _hoverText = String.Empty;
    private String _pokehoverText = String.Empty;
    private bool[] _drawObjects = new bool[4];
    private float _backgroundOffset = 0.0F;

    private Vector2 _cursorPosition = Vector2.Zero;
    private Vector2 _lastMousePosition = Vector2.Zero;

    public MapScreen(Screen currentScreen, List<String> regions, int startIndex, Object[] flag)
    {
        Identification = Identifications.MapScreen;

        PreScreen = currentScreen;
        _flag = flag;
        _currentRegion = regions[startIndex];
        _regions = regions;
        _regionPointer = startIndex;

        _drawObjects = Player.Temp.MapSwitch;

        if (Core.Player.IsGameJoltSave == true && GameJolt.API.LoggedIn == true)
        {
            GameJolt.Emblem ownE = new GameJolt.Emblem(Core.Player.Name, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem);
            _playerSkinTexture = ownE.SpriteTexture;
        }
        else
        {
            if (Screen.Level.Surfing == true)
                _playerSkinTexture = TextureManager.GetTexture("Textures\\NPC\\" + Core.Player.TempSurfSkin);
            else if (Screen.Level.Riding == true)
                _playerSkinTexture = TextureManager.GetTexture("Textures\\NPC\\" + Core.Player.TempRideSkin);
            else
                _playerSkinTexture = TextureManager.GetTexture("Textures\\NPC\\" + Core.Player.Skin);
        }

        MouseVisible = false;

        _objectsTexture = TextureManager.GetTexture("GUI\\Map\\map_objects");
        LoadMapTexture();

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");

        FillMap();

        Vector2 v = GetCursorPosition();
        if (v.X != 0 || v.Y != 0)
            _cursorPosition = GetCursorPosition() + new Vector2(mapOffsetX, mapOffsetY);
        else
            _cursorPosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);

        _lastMousePosition = _cursorPosition;
        Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
    }

    public MapScreen(Screen currentScreen, String startRegion, Object[] flag)
        : this(currentScreen, new List<String> { startRegion }, 0, flag)
    {
    }

    private void LoadMapTexture()
    {
        _mapTexture = TextureManager.GetTexture("GUI\\Map\\" + _currentRegion + "_map");
    }

    private void FillMap()
    {
        mapOffsetX = 100;
        mapOffsetY = 75;

        _routes.Clear();
        _places.Clear();
        _cities.Clear();
        _roamingPoke.Clear();

        List<Roaming> tempPoke = [];
        List<String> roamingPokeName = [];

        String path = GameModeManager.GetScriptPath("worldmap\\" + _currentRegion + ".dat");
        Security.FileValidation.CheckFileValid(path, false, "MapScreen.cs");

        String[] inputData = System.IO.File.ReadAllLines(path);

        foreach (String line in inputData)
        {
            if (line != String.Empty && line.StartsWith("{\"") == true)
            {
                Dictionary<String, String> tags = [];
                String[] data = line.Split('}');
                foreach (String tag in data)
                {
                    if (tag.Contains('{') == true && tag.Contains('[') == true)
                    {
                        String tagName = tag.Remove(0, 2);
                        tagName = tagName.Remove(tagName.IndexOf('"'));

                        String tagContent = tag.Remove(0, tag.IndexOf('[') + 1);
                        tagContent = tagContent.Remove(tagContent.IndexOf(']'));

                        tags[tagName.ToLower()] = tagContent;
                    }
                }

                switch (tags["placetype"].ToLower())
                {
                    case "city":
                    {
                        String name = tags["name"];
                        String[] mapFiles = tags["mapfiles"].Split(',');
                        List<String> positionList = tags["position"].Split(',').ToList();
                        String size = tags["size"];

                        City.CitySize citySize = City.CitySize.Small;
                        switch (size.ToLower())
                        {
                            case "small": case "0": citySize = City.CitySize.Small; break;
                            case "vertical": case "1": citySize = City.CitySize.Vertical; break;
                            case "horizontal": case "2": citySize = City.CitySize.Horizontal; break;
                            case "big": case "3": citySize = City.CitySize.Big; break;
                            case "large": case "4": citySize = City.CitySize.Large; break;
                        }

                        List<String> playerPositionList = "-1,-1".Split(',').ToList();
                        if (tags.ContainsKey("playerposition") == true)
                            playerPositionList = tags["playerposition"].Split(',').ToList();

                        int visible = (int)VisibleMode.Always;
                        if (tags.ContainsKey("visible") == true)
                        {
                            switch (tags["visible"].ToLower())
                            {
                                case "0": case "always": visible = (int)VisibleMode.Always; break;
                                case "1": case "temporary": visible = (int)VisibleMode.Temporary; break;
                                case "2": case "unlock": visible = (int)VisibleMode.Unlock; break;
                                case "3": case "register": visible = (int)VisibleMode.Register; break;
                            }
                        }

                        String register = String.Empty;
                        if (tags.ContainsKey("register") == true)
                            register = tags["register"];

                        if (tags.ContainsKey("flyto") == true)
                        {
                            List<String> flyTo = tags["flyto"].Split(',').ToList();
                            _cities.Add(new City(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]), citySize,
                                flyTo[0], new Vector3(float.Parse(flyTo[1]), float.Parse(flyTo[2]), float.Parse(flyTo[3])),
                                int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        else
                        {
                            _cities.Add(new City(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]), citySize,
                                String.Empty, Vector3.Zero, int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        break;
                    }
                    case "route":
                    {
                        String name = tags["name"];
                        String[] mapFiles = tags["mapfiles"].Split(',');
                        List<String> positionList = tags["position"].Split(',').ToList();

                        Route.RouteDirections routeDirection = Route.RouteDirections.Horizontal;
                        switch (tags["direction"].ToLower())
                        {
                            case "horizontal": case "0": routeDirection = Route.RouteDirections.Horizontal; break;
                            case "vertical": case "1": routeDirection = Route.RouteDirections.Vertical; break;
                            case "horizontalendright": case "2": routeDirection = Route.RouteDirections.HorizontalEndRight; break;
                            case "horizontalendleft": case "3": routeDirection = Route.RouteDirections.HorizontalEndLeft; break;
                            case "verticalendup": case "4": routeDirection = Route.RouteDirections.VerticalEndUp; break;
                            case "verticalenddown": case "5": routeDirection = Route.RouteDirections.VerticalEndDown; break;
                            case "curvedownright": case "6": routeDirection = Route.RouteDirections.CurveDownRight; break;
                            case "curvedownleft": case "7": routeDirection = Route.RouteDirections.CurveDownLeft; break;
                            case "curveupleft": case "8": routeDirection = Route.RouteDirections.CurveUpLeft; break;
                            case "curveupright": case "9": routeDirection = Route.RouteDirections.CurveUpRight; break;
                            case "tup": case "10": routeDirection = Route.RouteDirections.TUp; break;
                            case "tdown": case "13": routeDirection = Route.RouteDirections.TDown; break;
                            case "tleft": case "14": routeDirection = Route.RouteDirections.TLeft; break;
                            case "tright": case "15": routeDirection = Route.RouteDirections.TRight; break;
                            case "horizontalconnection": case "11": routeDirection = Route.RouteDirections.HorizontalConnection; break;
                            case "verticalconnection": case "12": routeDirection = Route.RouteDirections.VerticalConnection; break;
                        }

                        Route.RouteTypes routeType = Route.RouteTypes.Land;
                        switch (tags["routetype"].ToLower())
                        {
                            case "land": case "0": routeType = Route.RouteTypes.Land; break;
                            case "water": case "1": routeType = Route.RouteTypes.Water; break;
                        }

                        List<String> playerPositionList = "-1,-1".Split(',').ToList();
                        if (tags.ContainsKey("playerposition") == true)
                            playerPositionList = tags["playerposition"].Split(',').ToList();

                        int visible = (int)VisibleMode.Always;
                        if (tags.ContainsKey("visible") == true)
                        {
                            switch (tags["visible"].ToLower())
                            {
                                case "0": case "always": visible = (int)VisibleMode.Always; break;
                                case "1": case "temporary": visible = (int)VisibleMode.Temporary; break;
                                case "2": case "unlock": visible = (int)VisibleMode.Unlock; break;
                                case "3": case "register": visible = (int)VisibleMode.Register; break;
                            }
                        }

                        String register = String.Empty;
                        if (tags.ContainsKey("register") == true)
                            register = tags["register"];

                        if (tags.ContainsKey("flyto") == true)
                        {
                            List<String> flyTo = tags["flyto"].Split(',').ToList();
                            _routes.Add(new Route(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]),
                                routeDirection, routeType, flyTo[0], new Vector3(float.Parse(flyTo[1]), float.Parse(flyTo[2]), float.Parse(flyTo[3])),
                                int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        else
                        {
                            _routes.Add(new Route(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]),
                                routeDirection, routeType, String.Empty, Vector3.Zero,
                                int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        break;
                    }
                    case "place":
                    {
                        String name = tags["name"];
                        String[] mapFiles = tags["mapfiles"].Split(',');
                        List<String> positionList = tags["position"].Split(',').ToList();
                        String size = tags["size"];

                        Place.PlaceSizes placeSize = Place.PlaceSizes.Small;
                        switch (size.ToLower())
                        {
                            case "small": case "0": placeSize = Place.PlaceSizes.Small; break;
                            case "vertical": case "1": placeSize = Place.PlaceSizes.Vertical; break;
                            case "round": case "2": placeSize = Place.PlaceSizes.Round; break;
                            case "square": case "3": placeSize = Place.PlaceSizes.Square; break;
                            case "verticalbig": case "4": placeSize = Place.PlaceSizes.VerticalBig; break;
                            case "large": case "5": placeSize = Place.PlaceSizes.Large; break;
                        }

                        List<String> playerPositionList = "-1,-1".Split(',').ToList();
                        if (tags.ContainsKey("playerposition") == true)
                            playerPositionList = tags["playerposition"].Split(',').ToList();

                        int visible = (int)VisibleMode.Always;
                        if (tags.ContainsKey("visible") == true)
                        {
                            switch (tags["visible"].ToLower())
                            {
                                case "0": case "always": visible = (int)VisibleMode.Always; break;
                                case "1": case "temporary": visible = (int)VisibleMode.Temporary; break;
                                case "2": case "unlock": visible = (int)VisibleMode.Unlock; break;
                                case "3": case "register": visible = (int)VisibleMode.Register; break;
                            }
                        }

                        String register = String.Empty;
                        if (tags.ContainsKey("register") == true)
                            register = tags["register"];

                        if (tags.ContainsKey("flyto") == true)
                        {
                            List<String> flyTo = tags["flyto"].Split(',').ToList();
                            _places.Add(new Place(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]), placeSize,
                                flyTo[0], new Vector3(float.Parse(flyTo[1]), float.Parse(flyTo[2]), float.Parse(flyTo[3])),
                                int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        else
                        {
                            _places.Add(new Place(name, mapFiles, int.Parse(positionList[0]), int.Parse(positionList[1]), placeSize,
                                String.Empty, Vector3.Zero, int.Parse(playerPositionList[0]), int.Parse(playerPositionList[1]), visible, register));
                        }
                        break;
                    }
                }

                if (String.IsNullOrWhiteSpace(Core.Player.RoamingPokemonData) == false)
                {
                    if (Core.Player.RoamingPokemonData.Length > 0 && Core.Player.RoamingPokemonData.Contains('|') == true)
                    {
                        foreach (String pokes in Core.Player.RoamingPokemonData.SplitAtNewline())
                        {
                            String[] tempData = pokes.Split('|');
                            String[] mapFiles = tags["mapfiles"].Split(',');
                            String pokeCurrentLocation = tempData[4];
                            if (mapFiles.Contains(pokeCurrentLocation) == true)
                                tempPoke.Add(new Roaming(tempData[1], int.Parse(tags["position"].Split(',')[0]), int.Parse(tags["position"].Split(',')[1]), tags["name"]));

                            String pokemonID = tempData[1];
                            String pokemonAddition = "xXx";
                            if (pokemonID.Contains('_') == true)
                            {
                                pokemonAddition = PokemonForms.GetAdditionalValueFromDataFile(tempData[1]);
                                pokemonID = tempData[1].GetSplit(0, "_");
                            }
                            if (pokemonID.Contains(';') == true)
                            {
                                pokemonAddition = tempData[1].GetSplit(1, ";");
                                pokemonID = tempData[1].GetSplit(0, ";");
                            }
                            String pokeName = Pokemon.GetPokemonByID(int.Parse(pokemonID), pokemonAddition).GetName();
                            if (roamingPokeName.Contains(pokeName) == false)
                                roamingPokeName.Add(pokeName);
                        }
                    }
                }
            }
        }

        if (tempPoke.Count > 0 && roamingPokeName.Count > 0)
        {
            foreach (String pokes in roamingPokeName)
            {
                List<Roaming> mapObject = tempPoke.Where(p => p.Name == pokes).OrderBy(p => p.Distance).ToList();
                if (mapObject != null && mapObject.Count != 0)
                    _roamingPoke.Add(mapObject.ElementAt(mapObject[0].GetSkipIndex()));
            }
        }
    }

    public override void Update()
    {
        if (_lastMousePosition != new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y))
        {
            _cursorPosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
            _lastMousePosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
        }

        if (Controls.Dismiss() == true)
        {
            SoundManager.PlaySound("select");
            Player.Temp.MapSwitch = _drawObjects;
            Core.SetScreen(new TransitionScreen(this, PreScreen, Color.Black, false));
        }

        if (Controls.Up(false, true, false, true, true, true) == true)
        {
            _cursorPosition.Y -= MAP_MOVE_SPEED * 2.0F;
            Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
            _lastMousePosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
        }
        if (Controls.Down(false, true, false, true, true, true) == true)
        {
            _cursorPosition.Y += MAP_MOVE_SPEED * 2.0F;
            Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
            _lastMousePosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
        }
        if (Controls.Left(false, true, false, true, true, true) == true)
        {
            _cursorPosition.X -= MAP_MOVE_SPEED * 2.0F;
            Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
            _lastMousePosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
        }
        if (Controls.Right(false, true, false, true, true, true) == true)
        {
            _cursorPosition.X += MAP_MOVE_SPEED * 2.0F;
            Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
            _lastMousePosition = new Vector2(MouseHandler.MousePosition.X, MouseHandler.MousePosition.Y);
        }

        Vector2 mapOffset = new Vector2(MapScreen.mapOffsetX, MapScreen.mapOffsetY);
        Point cursorPoint = new Point((int)_cursorPosition.X, (int)_cursorPosition.Y);

        _hoverText = String.Empty;
        _pokehoverText = String.Empty;

        if (_hoverText == String.Empty && _pokehoverText == String.Empty && _drawObjects[3] == true)
        {
            foreach (Roaming poke in _roamingPoke)
            {
                if (poke.GetRectangle(mapOffset).Contains(cursorPoint) == true)
                {
                    _pokehoverText = poke.Name;
                    _hoverText = poke.Location;
                    break;
                }
            }
        }
        if (_hoverText == String.Empty && _pokehoverText == String.Empty && _drawObjects[2] == true)
        {
            foreach (Place place in _places)
            {
                if (place.GetRectangle(mapOffset).Contains(cursorPoint) == true)
                {
                    bool doUpdate = false;
                    if (place.Visible == (int)VisibleMode.Always || (place.Visible == (int)VisibleMode.Temporary && place.ContainFiles.Contains(Level.LevelFile.ToLower()) == true))
                        doUpdate = true;
                    else if (place.Visible == (int)VisibleMode.Unlock)
                    {
                        foreach (String p in place.ContainFiles)
                        {
                            if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(p.ToLower()) == true)
                                doUpdate = true;
                            break;
                        }
                    }
                    else if (place.Visible == (int)VisibleMode.Register)
                    {
                        if (ActionScript.IsRegistered(place.Register) == true)
                            doUpdate = true;
                    }
                    if (doUpdate == true)
                    {
                        if (Controls.Accept(true, true, true) == true)
                            place.Click(_flag);
                        _hoverText = place.Name;
                    }
                    break;
                }
            }
        }
        if (_hoverText == String.Empty && _pokehoverText == String.Empty && _drawObjects[0] == true)
        {
            foreach (City city in _cities)
            {
                if (city.GetRectangle(mapOffset).Contains(cursorPoint) == true)
                {
                    bool doUpdate = false;
                    if (city.Visible == (int)VisibleMode.Always || (city.Visible == (int)VisibleMode.Temporary && city.ContainFiles.Contains(Level.LevelFile.ToLower()) == true))
                        doUpdate = true;
                    else if (city.Visible == (int)VisibleMode.Unlock)
                    {
                        foreach (String p in city.ContainFiles)
                        {
                            if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(p.ToLower()) == true)
                                doUpdate = true;
                            break;
                        }
                    }
                    else if (city.Visible == (int)VisibleMode.Register)
                    {
                        if (ActionScript.IsRegistered(city.Register) == true)
                            doUpdate = true;
                    }
                    if (doUpdate == true)
                    {
                        if (Controls.Accept(true, true, true) == true)
                            city.Click(_flag);
                        _hoverText = city.Name;
                    }
                    break;
                }
            }
        }
        if (_hoverText == String.Empty && _pokehoverText == String.Empty && _drawObjects[1] == true)
        {
            foreach (Route route in _routes)
            {
                if (route.GetRectangle(mapOffset).Contains(cursorPoint) == true)
                {
                    bool doUpdate = false;
                    if (route.Visible == (int)VisibleMode.Always || (route.Visible == (int)VisibleMode.Temporary && route.ContainFiles.Contains(Level.LevelFile.ToLower()) == true))
                        doUpdate = true;
                    else if (route.Visible == (int)VisibleMode.Unlock)
                    {
                        foreach (String p in route.ContainFiles)
                        {
                            if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(p.ToLower()) == true)
                                doUpdate = true;
                            break;
                        }
                    }
                    else if (route.Visible == (int)VisibleMode.Register)
                    {
                        if (ActionScript.IsRegistered(route.Register) == true)
                            doUpdate = true;
                    }
                    if (doUpdate == true)
                    {
                        if (Controls.Accept(true, true, true) == true)
                            route.Click(_flag);
                        _hoverText = route.Name;
                    }
                    break;
                }
            }
        }

        _backgroundOffset += 1.0F;
        if (_backgroundOffset >= 64.0F)
            _backgroundOffset = 0.0F;

        UpdateSwitch();

        int cPointer = _regionPointer;
        if (KeyBoardHandler.KeyPressed(Keys.LeftShift) == true || ControllerHandler.ButtonPressed(Buttons.LeftTrigger) == true)
            _regionPointer -= 1;
        if (KeyBoardHandler.KeyPressed(Keys.RightShift) == true || ControllerHandler.ButtonPressed(Buttons.RightTrigger) == true)
            _regionPointer += 1;

        if (_regionPointer < 0)
            _regionPointer = _regions.Count - 1;
        else if (_regionPointer > _regions.Count - 1)
            _regionPointer = 0;

        if (_regionPointer != cPointer)
        {
            _currentRegion = _regions[_regionPointer];
            LoadMapTexture();
            FillMap();
        }
    }

    private void UpdateSwitch()
    {
        for (int i = 0; i <= 3; i++)
        {
            Rectangle r = new Rectangle(Core.windowSize.Width - 240, 100 + i * 30, 240, 30);
            if (Controls.Accept(true, true, true) == true)
            {
                if (r.Contains(new Point((int)MouseHandler.MousePosition.X, (int)MouseHandler.MousePosition.Y)) == true)
                    _drawObjects[i] = !_drawObjects[i];
            }
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int y = 0; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle(mapOffsetX + 15, mapOffsetY + 15, (int)(_mapTexture.Width / 16) * RASTER_SIZE * 2, (int)(_mapTexture.Height / 16) * RASTER_SIZE * 2), new Rectangle(96, 40, 16, 16), new Color(0, 0, 0, 100));

        Vector2 mapOffset = new Vector2(mapOffsetX, mapOffsetY);

        for (int x = 0; x <= _mapTexture.Width / 16; x++)
        {
            if (x * 16 <= _mapTexture.Width - 16)
            {
                for (int y = 0; y <= _mapTexture.Height / 16; y++)
                {
                    if (y * 16 <= _mapTexture.Height - 16)
                        Core.SpriteBatch.Draw(_mapTexture, new Rectangle((int)(x * RASTER_SIZE * 2 + mapOffset.X), (int)(y * RASTER_SIZE * 2 + mapOffset.Y), RASTER_SIZE * 2, RASTER_SIZE * 2), new Rectangle(x * 16, y * 16, 16, 16), Color.White);
                }
            }
        }

        if (_drawObjects[1] == true)
        {
            foreach (Route route in _routes)
            {
                bool isSelected = route.ContainFiles.Contains(Level.LevelFile.ToLower()) == true;
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && route.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(route.Visible, route.ContainFiles, route.Register) == true)
                    Core.SpriteBatch.Draw(route.GetTexture(_objectsTexture, isSelected), route.GetRectangle(mapOffset), c);
            }
        }

        if (_drawObjects[0] == true)
        {
            foreach (City city in _cities)
            {
                bool isSelected = city.ContainFiles.Contains(Level.LevelFile.ToLower()) == true;
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && city.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(city.Visible, city.ContainFiles, city.Register) == true)
                    Core.SpriteBatch.Draw(city.GetTexture(_objectsTexture, isSelected), city.GetRectangle(mapOffset), c);
            }
        }

        if (_drawObjects[2] == true)
        {
            foreach (Place place in _places)
            {
                bool isSelected = place.ContainFiles.Contains(Level.LevelFile.ToLower()) == true;
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && place.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(place.Visible, place.ContainFiles, place.Register) == true)
                    Core.SpriteBatch.Draw(place.GetTexture(_objectsTexture, isSelected), place.GetRectangle(mapOffset), c);
            }
        }

        // Draw fly icons
        if (_drawObjects[1] == true)
        {
            foreach (Route route in _routes)
            {
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && route.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(route.Visible, route.ContainFiles, route.Register) == true)
                {
                    if (_flag[0].ToString()!.ToLower() == "fly" && route.CanFlyTo(_flag) == true)
                    {
                        Rectangle iconRect = route.GetRectangle(mapOffset);
                        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle((int)(iconRect.X + (iconRect.Width / 2) - 16), (int)(iconRect.Y + (iconRect.Height / 2) - 16), 32, 32), new Rectangle(72, 16, 16, 16), c);
                    }
                }
            }
        }
        if (_drawObjects[0] == true)
        {
            foreach (City city in _cities)
            {
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && city.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(city.Visible, city.ContainFiles, city.Register) == true)
                {
                    if (_flag[0].ToString()!.ToLower() == "fly" && city.CanFlyTo(_flag) == true)
                    {
                        Rectangle iconRect = city.GetRectangle(mapOffset);
                        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle((int)(iconRect.X + (iconRect.Width / 2) - 16), (int)(iconRect.Y + (iconRect.Height / 2) - 16), 32, 32), new Rectangle(72, 0, 16, 16), c);
                    }
                }
            }
        }
        if (_drawObjects[2] == true)
        {
            foreach (Place place in _places)
            {
                Color c = Color.White;
                if (_flag[0].ToString()!.ToLower() == "fly" && place.CanFlyTo(_flag) == false)
                    c = Color.Gray;

                if (ShouldDraw(place.Visible, place.ContainFiles, place.Register) == true)
                {
                    if (_flag[0].ToString()!.ToLower() == "fly" && place.CanFlyTo(_flag) == true)
                    {
                        Rectangle iconRect = place.GetRectangle(mapOffset);
                        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle((int)(iconRect.X + (iconRect.Width / 2) - 16), (int)(iconRect.Y + (iconRect.Height / 2) - 16), 32, 32), new Rectangle(72, 32, 16, 16), c);
                    }
                }
            }
        }

        if (_drawObjects[3] == true)
        {
            foreach (Roaming pokes in _roamingPoke)
                Core.SpriteBatch.Draw(pokes.GetTexture(), pokes.GetRectangle(mapOffset), Color.White);
        }

        int playerSkinWidth = _playerSkinTexture.Width / 3;
        int playerSkinHeight = _playerSkinTexture.Height / 4;
        float playerSkinScale = 1.0F;
        if (_playerSkinTexture.Width == _playerSkinTexture.Height / 2)
            playerSkinWidth = _playerSkinTexture.Width / 2;
        else if (_playerSkinTexture.Width == _playerSkinTexture.Height)
            playerSkinWidth = _playerSkinTexture.Width / 4;
        if (playerSkinWidth > 32)
            playerSkinScale = 0.5F;

        Rectangle playerTextureRectangle = new Rectangle(0, playerSkinHeight * 2, playerSkinWidth, playerSkinHeight);

        Vector2 v = GetPlayerPosition();
        if (v.X != 0 || v.Y != 0)
        {
            Core.SpriteBatch.Draw(_playerSkinTexture,
                new Rectangle(
                    (int)(GetPlayerPosition().X + mapOffsetX - playerSkinWidth * playerSkinScale),
                    (int)(GetPlayerPosition().Y + mapOffsetY - playerSkinHeight * playerSkinScale),
                    (int)(playerSkinWidth * 2 * playerSkinScale),
                    (int)(playerSkinHeight * 2 * playerSkinScale)),
                playerTextureRectangle, Color.White);
        }

        if (_hoverText != String.Empty && _pokehoverText != String.Empty)
        {
            String hoverString = Localization.GetString("map_screen_PokemonAtPlace", "[NAME] at [PLACE]")
                .Replace("[NAME]", Localization.GetString("pokemon_name_" + _pokehoverText))
                .Replace("[PLACE]", Localization.GetString("Places_" + _hoverText));
            Core.SpriteBatch.DrawRectangle(
                new Rectangle((int)(_cursorPosition.X + 32 - 2), (int)(_cursorPosition.Y - 32 + 2), (int)(FontManager.MainFont.MeasureString(hoverString).X + 2 + 4), (int)FontManager.MainFont.MeasureString(hoverString).Y),
                Color.FromNonPremultiplied(0, 0, 0, 153));
            Core.SpriteBatch.DrawString(FontManager.MainFont, hoverString, new Vector2((int)(_cursorPosition.X + 32 + 2), (int)(_cursorPosition.Y - 32 + 2)), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, hoverString, new Vector2((int)(_cursorPosition.X + 32), (int)(_cursorPosition.Y - 32)), Color.White);
        }
        else if (_hoverText != String.Empty && _pokehoverText == String.Empty)
        {
            String hoverString = Localization.GetString("Places_" + _hoverText);
            Core.SpriteBatch.DrawRectangle(
                new Rectangle((int)(_cursorPosition.X + 32 - 2), (int)(_cursorPosition.Y - 32 + 2), (int)(FontManager.MainFont.MeasureString(hoverString).X + 2 + 4), (int)FontManager.MainFont.MeasureString(hoverString).Y),
                Color.FromNonPremultiplied(0, 0, 0, 153));
            Core.SpriteBatch.DrawString(FontManager.MainFont, hoverString, new Vector2(_cursorPosition.X + 32 + 2, _cursorPosition.Y - 32 + 2), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MainFont, hoverString, new Vector2(_cursorPosition.X + 32, _cursorPosition.Y - 32), Color.White);
        }

        String firstChar = _currentRegion[0].ToString().ToUpper();
        String regionString = Localization.GetString(firstChar + _currentRegion.Remove(0, 1));
        if (_regions.Count > 1)
            regionString += " " + Localization.GetString("map_screen_RegionSwitchHint", "(Press the Shift Key/Shoulder Triggers to switch between regions.)");

        Core.SpriteBatch.DrawString(FontManager.InGameFont, regionString, new Vector2(MapScreen.mapOffsetX + 2, MapScreen.mapOffsetY - 31), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.InGameFont, regionString, new Vector2(MapScreen.mapOffsetX, MapScreen.mapOffsetY - 33), Color.White);

        DrawSwitch();
        DrawCursor();
    }

    private bool ShouldDraw(int visible, List<String> containFiles, String register)
    {
        if (visible == (int)VisibleMode.Always)
            return true;
        if (visible == (int)VisibleMode.Temporary && containFiles.Contains(Level.LevelFile.ToLower()) == true)
            return true;
        if (visible == (int)VisibleMode.Unlock)
        {
            foreach (String p in containFiles)
            {
                if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(p.ToLower()) == true)
                    return true;
                break;
            }
        }
        if (visible == (int)VisibleMode.Register && ActionScript.IsRegistered(register) == true)
            return true;
        return false;
    }

    private void DrawSwitch()
    {
        // Cities:
        Rectangle r = _drawObjects[0] == true ? new Rectangle(104, 0, 12, 12) : new Rectangle(116, 0, 12, 12);
        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle(Core.windowSize.Width - 256 - 48, 100, 24, 24), r, new Color(255, 255, 255, 220));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_cities"), new Vector2(Core.windowSize.Width - 256 + 2, 100 + 2), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_cities"), new Vector2(Core.windowSize.Width - 256, 100), Color.White);

        // Routes:
        r = _drawObjects[1] == true ? new Rectangle(104, 12, 12, 12) : new Rectangle(116, 12, 12, 12);
        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle(Core.windowSize.Width - 256 - 48, 130, 24, 24), r, new Color(255, 255, 255, 220));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_routes"), new Vector2(Core.windowSize.Width - 256 + 2, 130 + 2), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_routes"), new Vector2(Core.windowSize.Width - 256, 130), Color.White);

        // Places:
        r = _drawObjects[2] == true ? new Rectangle(104, 24, 12, 12) : new Rectangle(116, 24, 12, 12);
        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle(Core.windowSize.Width - 256 - 48, 160, 24, 24), r, new Color(255, 255, 255, 220));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_places"), new Vector2(Core.windowSize.Width - 256 + 2, 160 + 2), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_places"), new Vector2(Core.windowSize.Width - 256, 160), Color.White);

        // Roaming:
        r = _drawObjects[3] == true ? new Rectangle(113, 65, 14, 14) : new Rectangle(113, 81, 14, 14);
        Core.SpriteBatch.Draw(_objectsTexture, new Rectangle(Core.windowSize.Width - 256 - 48, 187, 28, 28), r, new Color(255, 255, 255, 220));
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_roaming"), new Vector2(Core.windowSize.Width - 256 + 2, 190 + 2), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("map_screen_roaming"), new Vector2(Core.windowSize.Width - 256, 190), Color.White);
    }

    private void DrawCursor()
    {
        Texture2D t = TextureManager.GetTexture("GUI\\Menus\\General", new Rectangle(0, 0, 16, 16), String.Empty);
        Core.SpriteBatch.Draw(t, new Rectangle((int)_cursorPosition.X, (int)_cursorPosition.Y - 30, 32, 32), Color.White);
    }

    public static void UseFly(String flyToFile, Vector3 flyToPosition, Object[] flag)
    {
        Screen.Camera.PlannedMovement = new Vector3(0, 2, 0);

        Pokemon p = (Pokemon)flag[1];
        String skinName = Screen.Level.OwnPlayer.SkinName;

        if (Screen.Level.Surfing == true)
        {
            skinName = Core.Player.TempSurfSkin;
            Screen.Level.Surfing = false;
            Screen.Level.OverworldPokemon.Visible = false;
        }
        if (Screen.Level.Riding == true)
        {
            skinName = Core.Player.TempRideSkin;
            Screen.Level.Riding = false;
        }

        String isShiny = "N";
        if (p != null && p.IsShiny == true)
            isShiny = "S";

        String s = "version=2" + Environment.NewLine +
            "@text.show(" + p.GetDisplayName() + " " + Localization.GetString("fieldmove_fly_used", "used~Fly!") + ")" + Environment.NewLine;

        if (((OverworldCamera)Screen.Camera).ThirdPerson == false)
            s += "@camera.activateThirdPerson" + Environment.NewLine;

        s += "@camera.setposition(0,0.9,3)" + Environment.NewLine +
            "@level.wait(30)" + Environment.NewLine +
            "@pokemon.cry(" + p.Number + ")" + Environment.NewLine +
            "@player.wearskin([POKEMON|" + isShiny + "]" + p.Number + PokemonForms.GetOverworldAddition(p) + ")" + Environment.NewLine +
            "@player.turnto(2)" + Environment.NewLine +
            "@player.move(2)" + Environment.NewLine +
            "@sound.play(FieldMove_Fly)" + Environment.NewLine +
            "@camera.fix" + Environment.NewLine +
            "@player.setmovement(0,2,3)" + Environment.NewLine +
            "@player.move(3)" + Environment.NewLine +
            "@screen.fadeout(10)" + Environment.NewLine +
            "@camera.defix" + Environment.NewLine +
            "@camera.reset" + Environment.NewLine +
            "@player.turnto(0)" + Environment.NewLine +
            "@player.warp(" + flyToFile + "," + flyToPosition.X.ToString().ReplaceDecSeparator() + "," + (flyToPosition.Y - 4 + 0.1F).ToString().ReplaceDecSeparator() + "," + (flyToPosition.Z + 6).ToString().ReplaceDecSeparator() + ",0)" + Environment.NewLine +
            "@camera.setyaw(0)" + Environment.NewLine +
            "@camera.setposition(0,-3.7,-4.5)" + Environment.NewLine +
            "@level.update" + Environment.NewLine +
            "@player.setmovement(0,-2,-3)" + Environment.NewLine +
            "@screen.fadein(10)" + Environment.NewLine +
            "@camera.fix" + Environment.NewLine +
            "@player.move(2)" + Environment.NewLine +
            "@camera.reset" + Environment.NewLine +
            "@camera.defix(1)" + Environment.NewLine +
            "@player.setmovement(0,-2,0)" + Environment.NewLine +
            "@player.move(2)" + Environment.NewLine +
            "@player.turnto(2)" + Environment.NewLine +
            "@player.wearskin(" + skinName + ")" + Environment.NewLine;

        while (Core.CurrentScreen.Identification != Identifications.OverworldScreen)
        {
            if (Core.CurrentScreen.PreScreen?.Identification == Identifications.OverworldScreen)
            {
                Core.SetScreen(new TransitionScreen(Core.CurrentScreen, Core.CurrentScreen.PreScreen, Color.White, false));
                break;
            }
            else
            {
                Core.SetScreen(Core.CurrentScreen.PreScreen!);
            }
        }

        if (((OverworldCamera)Screen.Camera).ThirdPerson == false)
            s += "@camera.deactivatethirdperson" + Environment.NewLine;

        s += "@level.wait(1)" + Environment.NewLine +
             ":end";

        PlayerStatistics.Track("Fly used", 1);
        Core.Player.IsFlying = true;
        ((OverworldScreen)((TransitionScreen)Core.CurrentScreen).NewScreen!).ActionScript.StartScript(s, 2, false);
    }

    private Vector2 GetCursorPosition()
    {
        Vector2 v = Vector2.Zero;
        Rectangle r = new Rectangle(0, 0, 0, 0);
        Vector2 mapOffset = new Vector2(mapOffsetX, mapOffsetY);

        foreach (City city in _cities)
        {
            if (city.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
            {
                v = city.GetPosition();
                r = city.GetRectangle(mapOffset);
            }
        }
        foreach (Place place in _places)
        {
            if (place.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
            {
                v = place.GetPosition();
                r = place.GetRectangle(mapOffset);
            }
        }
        foreach (Route route in _routes)
        {
            if (route.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
            {
                v = route.GetPosition();
                r = route.GetRectangle(mapOffset);
            }
        }

        return v + new Vector2(r.Width / 2, r.Height / 2);
    }

    private Vector2 GetPlayerPosition()
    {
        Vector2 v = Vector2.Zero;
        Rectangle r = new Rectangle(0, 0, 0, 0);
        Vector2 mapOffset = new Vector2(mapOffsetX, mapOffsetY);
        int sizeX = 1 * RASTER_SIZE;
        int sizeY = 1 * RASTER_SIZE;

        foreach (City city in _cities)
        {
            if (city.Visible != (int)VisibleMode.Register || ActionScript.IsRegistered(city.Register) == true)
            {
                if (city.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
                {
                    if (city.PlayerPositionX != -1 && city.PlayerPositionY != -1)
                    {
                        v = city.GetPlayerPosition();
                        r = new Rectangle((int)(city.GetPlayerPosition().X + mapOffset.X), (int)(city.GetPosition().Y + mapOffset.Y), sizeX, sizeY);
                    }
                    else
                    {
                        v = city.GetPosition();
                        r = city.GetRectangle(mapOffset);
                    }
                }
            }
        }
        foreach (Place place in _places)
        {
            if (place.Visible != (int)VisibleMode.Register || ActionScript.IsRegistered(place.Register) == true)
            {
                if (place.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
                {
                    if (place.PlayerPositionX != -1 && place.PlayerPositionY != -1)
                    {
                        v = place.GetPlayerPosition();
                        r = new Rectangle((int)(place.GetPlayerPosition().X + mapOffset.X), (int)(place.GetPosition().Y + mapOffset.Y), sizeX, sizeY);
                    }
                    else
                    {
                        v = place.GetPosition();
                        r = place.GetRectangle(mapOffset);
                    }
                }
            }
        }
        foreach (Route route in _routes)
        {
            if (route.Visible != (int)VisibleMode.Register || ActionScript.IsRegistered(route.Register) == true)
            {
                if (route.ContainFiles.Contains(Level.LevelFile.ToLower()) == true)
                {
                    if (route.PlayerPositionX != -1 && route.PlayerPositionY != -1)
                    {
                        v = route.GetPlayerPosition();
                        r = new Rectangle((int)(route.GetPlayerPosition().X + mapOffset.X), (int)(route.GetPosition().Y + mapOffset.Y), sizeX, sizeY);
                    }
                    else
                    {
                        v = route.GetPosition();
                        r = route.GetRectangle(mapOffset);
                    }
                }
            }
        }

        return v + new Vector2(r.Width / 2, r.Height / 2);
    }

    public class City
    {
        public enum CitySize { Small, Vertical, Horizontal, Big, Large }

        public String Name = "???";
        public List<String> ContainFiles = [];
        public int PlayerPositionX = -1;
        public int PlayerPositionY = -1;
        public int PositionX = 0;
        public int PositionY = 0;
        public String FlyToFile = String.Empty;
        public Vector3 FlyToPosition = Vector3.Zero;
        public CitySize Size = CitySize.Small;
        public int Visible = (int)VisibleMode.Always;
        public String Register = String.Empty;

        private Texture2D? _t;

        public City(String name, String[] containFiles, int positionX, int positionY, CitySize size,
                    String flyToFile = "", Vector3 flyToPosition = default,
                    int playerPositionX = -1, int playerPositionY = -1,
                    int visible = (int)VisibleMode.Always, String register = "")
        {
            Name = name;
            foreach (String file in containFiles)
                ContainFiles.Add(file.ToLower());
            PositionX = positionX;
            PositionY = positionY;
            PlayerPositionX = playerPositionX != -1 ? playerPositionX : positionX;
            PlayerPositionY = playerPositionY != -1 ? playerPositionY : positionY;
            Size = size;
            FlyToFile = flyToFile;
            FlyToPosition = flyToPosition;
            Visible = visible;
            Register = register;
        }

        public Vector2 GetPosition() => new Vector2(PositionX * MapScreen.RASTER_SIZE, PositionY * MapScreen.RASTER_SIZE);

        public Vector2 GetPlayerPosition() => new Vector2(PlayerPositionX * MapScreen.RASTER_SIZE, PlayerPositionY * MapScreen.RASTER_SIZE);

        public Rectangle GetRectangle(Vector2 offset)
        {
            int sizeX = 0;
            int sizeY = 0;
            switch (Size)
            {
                case CitySize.Small: sizeX = 1; sizeY = 1; break;
                case CitySize.Horizontal: sizeX = 2; sizeY = 1; break;
                case CitySize.Vertical: sizeX = 1; sizeY = 2; break;
                case CitySize.Big: sizeX = 2; sizeY = 2; break;
                case CitySize.Large: sizeX = 3; sizeY = 2; break;
            }
            sizeX *= MapScreen.RASTER_SIZE;
            sizeY *= MapScreen.RASTER_SIZE;
            return new Rectangle((int)(GetPosition().X + offset.X), (int)(GetPosition().Y + offset.Y), sizeX, sizeY);
        }

        public Texture2D GetTexture(Texture2D fullTexture, bool isSelected)
        {
            if (_t == null || isSelected == true)
            {
                int modX = isSelected == true ? 36 : 0;
                Rectangle r;
                switch (Size)
                {
                    case CitySize.Small: r = new Rectangle(0 + modX, 0, 12, 12); break;
                    case CitySize.Vertical: r = new Rectangle(0 + modX, 12, 12, 24); break;
                    case CitySize.Horizontal: r = new Rectangle(12 + modX, 0, 24, 12); break;
                    case CitySize.Big: r = new Rectangle(12 + modX, 12, 24, 24); break;
                    case CitySize.Large: r = new Rectangle(0 + modX, 36, 36, 24); break;
                    default: r = new Rectangle(0, 0, 12, 12); break;
                }
                _t = TextureManager.GetTexture(fullTexture, r);
            }
            return _t;
        }

        public void Click(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly" && CanFlyTo(flag) == true)
                MapScreen.UseFly(FlyToFile, FlyToPosition, flag);
        }

        public bool CanFlyTo(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly")
            {
                if (FlyToPosition != Vector3.Zero && FlyToFile != String.Empty)
                {
                    bool flytomap = false;
                    foreach (String map in ContainFiles)
                    {
                        if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(map.ToLower()) == true)
                        {
                            flytomap = true;
                            break;
                        }
                    }
                    if (flytomap == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
                        return true;
                }
            }
            return false;
        }
    }

    public class Route
    {
        public enum RouteTypes { Land, Water }

        public enum RouteDirections
        {
            Horizontal, Vertical,
            HorizontalEndRight, HorizontalEndLeft,
            VerticalEndUp, VerticalEndDown,
            CurveDownRight, CurveDownLeft, CurveUpLeft, CurveUpRight,
            TUp, TDown, TRight, TLeft,
            HorizontalConnection, VerticalConnection
        }

        public String Name = String.Empty;
        public int PositionX = 0;
        public int PositionY = 0;
        public int PlayerPositionX = -1;
        public int PlayerPositionY = -1;
        public List<String> ContainFiles = [];
        public String FlyToFile = String.Empty;
        public Vector3 FlyToPosition = Vector3.Zero;
        public RouteDirections RouteDirection = RouteDirections.Horizontal;
        public RouteTypes RouteType = RouteTypes.Land;
        public int Visible = (int)VisibleMode.Always;
        public String Register = String.Empty;

        private Texture2D? _t;

        public Route(String name, String[] containFiles, int positionX, int positionY,
                     RouteDirections routeDirection, RouteTypes routeType,
                     String flyToFile = "", Vector3 flyToPosition = default,
                     int playerPositionX = -1, int playerPositionY = -1,
                     int visible = (int)VisibleMode.Always, String register = "")
        {
            Name = name;
            PositionX = positionX;
            PositionY = positionY;
            RouteDirection = routeDirection;
            RouteType = routeType;
            foreach (String file in containFiles)
                ContainFiles.Add(file.ToLower());
            PlayerPositionX = playerPositionX != -1 ? playerPositionX : positionX;
            PlayerPositionY = playerPositionY != -1 ? playerPositionY : positionY;
            FlyToFile = flyToFile;
            FlyToPosition = flyToPosition;
            Visible = visible;
            Register = register;
        }

        public Vector2 GetPosition() => new Vector2(PositionX * MapScreen.RASTER_SIZE, PositionY * MapScreen.RASTER_SIZE);

        public Vector2 GetPlayerPosition() => new Vector2(PlayerPositionX * MapScreen.RASTER_SIZE, PlayerPositionY * MapScreen.RASTER_SIZE);

        public Rectangle GetRectangle(Vector2 offset)
        {
            float sizeX = 1.0F;
            float sizeY = 1.0F;
            switch (RouteDirection)
            {
                case RouteDirections.Horizontal: sizeX = 1.25F; sizeY = 0.75F; break;
                case RouteDirections.Vertical: sizeX = 0.75F; sizeY = 1.25F; break;
                case RouteDirections.CurveDownLeft: case RouteDirections.CurveDownRight:
                case RouteDirections.CurveUpLeft: case RouteDirections.CurveUpRight:
                case RouteDirections.TUp: case RouteDirections.TDown:
                case RouteDirections.TRight: case RouteDirections.TLeft:
                    sizeX = 0.75F; sizeY = 0.75F; break;
                case RouteDirections.HorizontalConnection: sizeX = 1.0F; sizeY = 0.75F; break;
                case RouteDirections.VerticalConnection: sizeX = 0.75F; sizeY = 1.0F; break;
                case RouteDirections.HorizontalEndRight: case RouteDirections.HorizontalEndLeft:
                case RouteDirections.VerticalEndDown: case RouteDirections.VerticalEndUp:
                    sizeX = 0.75F; sizeY = 0.75F; break;
            }

            Vector2 positionOffset = new Vector2(((1 - sizeX) * MapScreen.RASTER_SIZE) / 2, ((1 - sizeY) * MapScreen.RASTER_SIZE) / 2);

            if (RouteDirection == RouteDirections.HorizontalConnection)
                positionOffset.X += 0.5F * MapScreen.RASTER_SIZE;
            if (RouteDirection == RouteDirections.VerticalConnection)
                positionOffset.Y += 0.5F * MapScreen.RASTER_SIZE;

            sizeX *= MapScreen.RASTER_SIZE;
            sizeY *= MapScreen.RASTER_SIZE;

            return new Rectangle((int)(GetPosition().X + positionOffset.X + offset.X), (int)(GetPosition().Y + positionOffset.Y + offset.Y), (int)sizeX, (int)sizeY);
        }

        public Texture2D GetTexture(Texture2D fullTexture, bool isSelected)
        {
            if (_t == null || isSelected == true)
            {
                int modX = RouteType == RouteTypes.Water ? 32 : 0;
                if (isSelected == true)
                    modX = 64;
                int y = 64;
                Rectangle r;
                switch (RouteDirection)
                {
                    case RouteDirections.Horizontal: case RouteDirections.HorizontalConnection: r = new Rectangle(0 + modX, 0 + y, 8, 8); break;
                    case RouteDirections.TUp: r = new Rectangle(8 + modX, 0 + y, 8, 8); break;
                    case RouteDirections.TDown: r = new Rectangle(8 + modX, 24 + y, 8, 8); break;
                    case RouteDirections.TRight: r = new Rectangle(0 + modX, 24 + y, 8, 8); break;
                    case RouteDirections.TLeft: r = new Rectangle(24 + modX, 16 + y, 8, 8); break;
                    case RouteDirections.Vertical: case RouteDirections.VerticalConnection: r = new Rectangle(16 + modX, 0 + y, 8, 8); break;
                    case RouteDirections.HorizontalEndRight: r = new Rectangle(24 + modX, 0 + y, 8, 8); break;
                    case RouteDirections.CurveUpLeft: r = new Rectangle(0 + modX, 8 + y, 8, 8); break;
                    case RouteDirections.CurveDownRight: r = new Rectangle(8 + modX, 8 + y, 8, 8); break;
                    case RouteDirections.CurveUpRight: r = new Rectangle(16 + modX, 8 + y, 8, 8); break;
                    case RouteDirections.CurveDownLeft: r = new Rectangle(24 + modX, 8 + y, 8, 8); break;
                    case RouteDirections.HorizontalEndLeft: r = new Rectangle(0 + modX, 16 + y, 8, 8); break;
                    case RouteDirections.VerticalEndDown: r = new Rectangle(8 + modX, 16 + y, 8, 8); break;
                    case RouteDirections.VerticalEndUp: r = new Rectangle(16 + modX, 16 + y, 8, 8); break;
                    default: r = new Rectangle(0 + modX, 0 + y, 8, 8); break;
                }
                _t = TextureManager.GetTexture(fullTexture, r);
            }
            return _t;
        }

        public void Click(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly" && CanFlyTo(flag) == true)
                MapScreen.UseFly(FlyToFile, FlyToPosition, flag);
        }

        public bool CanFlyTo(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly")
            {
                if (FlyToPosition != Vector3.Zero && FlyToFile != String.Empty)
                {
                    if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(FlyToFile.ToLower()) == true
                        || GameController.IS_DEBUG_ACTIVE == true
                        || Core.Player.SandBoxMode == true)
                        return true;
                }
            }
            return false;
        }
    }

    public class Place
    {
        public enum PlaceSizes { Small, Vertical, Round, Square, VerticalBig, Large }

        public String Name = "???";
        public List<String> ContainFiles = [];
        public int PositionX;
        public int PositionY;
        public int PlayerPositionX = -1;
        public int PlayerPositionY = -1;
        public PlaceSizes PlaceSize;
        public int Visible = (int)VisibleMode.Always;
        public String Register = String.Empty;
        public String FlyToFile = String.Empty;
        public Vector3 FlyToPosition = Vector3.Zero;

        private Texture2D? _t;

        public Place(String name, String[] containFiles, int positionX, int positionY, PlaceSizes placeSize,
                     String flyToFile = "", Vector3 flyToPosition = default,
                     int playerPositionX = -1, int playerPositionY = -1,
                     int visible = (int)VisibleMode.Always, String register = "")
        {
            Name = name;
            PositionX = positionX;
            PositionY = positionY;
            PlaceSize = placeSize;
            foreach (String file in containFiles)
                ContainFiles.Add(file.ToLower());
            PlayerPositionX = playerPositionX != -1 ? playerPositionX : positionX;
            PlayerPositionY = playerPositionY != -1 ? playerPositionY : positionY;
            FlyToFile = flyToFile;
            FlyToPosition = flyToPosition;
            Visible = visible;
            Register = register;
        }

        public Vector2 GetPosition() => new Vector2(PositionX * MapScreen.RASTER_SIZE, PositionY * MapScreen.RASTER_SIZE);

        public Vector2 GetPlayerPosition() => new Vector2(PlayerPositionX * MapScreen.RASTER_SIZE, PlayerPositionY * MapScreen.RASTER_SIZE);

        public Rectangle GetRectangle(Vector2 offset)
        {
            float sizeX = 1.0F;
            float sizeY = 1.0F;
            switch (PlaceSize)
            {
                case PlaceSizes.Small: sizeX = 1.0F; sizeY = 1.0F; break;
                case PlaceSizes.Vertical: sizeX = 1.0F; sizeY = 2.0F; break;
                case PlaceSizes.Round: sizeX = 1.5F; sizeY = 1.5F; break;
                case PlaceSizes.Square: sizeX = 2.0F; sizeY = 2.0F; break;
                case PlaceSizes.VerticalBig: sizeX = 1.5F; sizeY = 2.5F; break;
                case PlaceSizes.Large: sizeX = 3.5F; sizeY = 2.5F; break;
            }

            Vector2 positionOffset = new Vector2(((1 - sizeX) * MapScreen.RASTER_SIZE) / 2, ((1 - sizeY) * MapScreen.RASTER_SIZE) / 2);

            if (PlaceSize == PlaceSizes.Vertical)
                positionOffset.Y += 0.5F * MapScreen.RASTER_SIZE;

            sizeX *= MapScreen.RASTER_SIZE;
            sizeY *= MapScreen.RASTER_SIZE;

            return new Rectangle((int)(GetPosition().X + positionOffset.X + offset.X), (int)(GetPosition().Y + positionOffset.Y + offset.Y), (int)sizeX, (int)sizeY);
        }

        public Texture2D GetTexture(Texture2D fullTexture, bool isSelected)
        {
            if (_t == null || isSelected == true)
            {
                int modX = isSelected == true ? 56 : 0;
                int y = 96;
                Rectangle r;
                switch (PlaceSize)
                {
                    case PlaceSizes.Small: r = new Rectangle(12 + modX, 20 + y, 8, 8); break;
                    case PlaceSizes.Vertical: r = new Rectangle(40 + modX, 16 + y, 8, 16); break;
                    case PlaceSizes.Round: r = new Rectangle(0 + modX, 20 + y, 12, 12); break;
                    case PlaceSizes.Square: r = new Rectangle(40 + modX, 0 + y, 16, 16); break;
                    case PlaceSizes.VerticalBig: r = new Rectangle(0 + modX, 0 + y, 12, 20); break;
                    case PlaceSizes.Large: r = new Rectangle(12 + modX, 0 + y, 28, 20); break;
                    default: r = new Rectangle(12 + modX, 20 + y, 8, 8); break;
                }
                _t = TextureManager.GetTexture(fullTexture, r);
            }
            return _t;
        }

        public void Click(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly" && CanFlyTo(flag) == true)
                MapScreen.UseFly(FlyToFile, FlyToPosition, flag);
        }

        public bool CanFlyTo(Object[] flag)
        {
            if (flag[0].ToString()!.ToLower() == "fly")
            {
                if (FlyToPosition != Vector3.Zero && FlyToFile != String.Empty)
                {
                    bool flytomap = false;
                    foreach (String map in ContainFiles)
                    {
                        if (Core.Player.VisitedMaps.ToLower().Split(',').Contains(map.ToLower()) == true)
                        {
                            flytomap = true;
                            break;
                        }
                    }
                    if (flytomap == true || GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
                        return true;
                }
            }
            return false;
        }
    }

    public class Roaming
    {
        public String ID = String.Empty;
        public String Name = String.Empty;
        public String Location = String.Empty;
        public int PositionX;
        public int PositionY;
        public double Distance;
        public Pokemon Species = null!;

        private Texture2D? _t;

        public Roaming(String id, int positionX, int positionY, String location)
        {
            ID = id;
            String pokemonID = id;
            String pokemonAddition = "xXx";
            if (pokemonID.Contains('_') == true)
            {
                pokemonAddition = PokemonForms.GetAdditionalValueFromDataFile(id);
                pokemonID = id.GetSplit(0, "_");
            }
            if (pokemonID.Contains(';') == true)
            {
                pokemonAddition = id.GetSplit(1, ";");
                pokemonID = id.GetSplit(0, ";");
            }
            Name = Pokemon.GetPokemonByID(int.Parse(pokemonID), pokemonAddition).GetName();
            Species = Pokemon.GetPokemonByID(int.Parse(pokemonID), pokemonAddition);
            PositionX = positionX;
            PositionY = positionY;
            Location = location;
            Distance = Math.Pow(Math.Pow(positionX, 2) + Math.Pow(positionY, 2), 0.5);
        }

        public Vector2 GetPosition() => new Vector2(PositionX * MapScreen.RASTER_SIZE, PositionY * MapScreen.RASTER_SIZE);

        public Rectangle GetRectangle(Vector2 offset)
        {
            int sizeX = 1 * MapScreen.RASTER_SIZE;
            int sizeY = 1 * MapScreen.RASTER_SIZE;
            return new Rectangle((int)(GetPosition().X + offset.X), (int)(GetPosition().Y + offset.Y), sizeX, sizeY);
        }

        public Texture2D GetTexture()
        {
            Vector2 v = PokemonForms.GetMenuImagePositionVec(Species);
            Size s = PokemonForms.GetMenuImageSize(Species);
            String sheet = PokemonForms.GetSheetName(Species);
            return TextureManager.GetTexture("GUI\\PokemonMenu\\" + sheet, new Rectangle((int)v.X * 32, (int)v.Y * 32, s.Width, s.Height), String.Empty);
        }

        public int GetSkipIndex()
        {
            switch (Location)
            {
                case "Route 31": case "Route 37": case "Route 42": return 0;
                case "Route 29": case "Route 30": case "Route 33": case "Route 34":
                case "Route 35": case "Route 36": case "Route 38": case "Route 39": case "Route 44": return 1;
                case "Route 32": case "Route 45": return 2;
                default: return 0;
            }
        }
    }
}
